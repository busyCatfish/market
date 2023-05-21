﻿using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Business.Models;
using System.Linq;
using Stripe.Checkout;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Models;

namespace WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[ApiExplorerSettings(IgnoreApi = true)]
	public class CheckoutController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		private static string s_wasmClientURL = string.Empty;

		public CheckoutController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpPost]
		public async Task<ActionResult> CheckoutOrder([FromBody] ProductModel product, [FromServices] IServiceProvider sp)
		{
			var referer = Request.Headers["Referer"];
			s_wasmClientURL = referer[0];

			// Build the URL to which the customer will be redirected after paying.
			var server = sp.GetRequiredService<IServer>();

			var serverAddressesFeature = server.Features.Get<IServerAddressesFeature>();

			string? thisApiUrl = null;

			if (serverAddressesFeature != null)
			{
				thisApiUrl = serverAddressesFeature.Addresses.FirstOrDefault();
			}

			if (thisApiUrl != null)
			{
				var sessionId = await CheckOut(product, thisApiUrl);
				var pubKey = _configuration["Stripe:PubKey"];

				var checkoutOrderResponse = new CheckoutOrderResponse()
				{
					SessionId = sessionId,
					PubKey = pubKey
				};

				return Ok(checkoutOrderResponse);
			}
			else
			{
				return StatusCode(500);
			}
		}

		[NonAction]
		public async Task<string> CheckOut(ProductModel product, string thisApiUrl)
		{
			// Create a payment flow from the items in the cart.
			// Gets sent to Stripe API.
			var options = new SessionCreateOptions
			{
				// Stripe calls the URLs below when certain checkout events happen such as success and failure.
				SuccessUrl = $"{thisApiUrl}/checkout/success?sessionId=" + "{CHECKOUT_SESSION_ID}", // Customer paid.
				CancelUrl = s_wasmClientURL + "failed",  // Checkout cancelled.
				PaymentMethodTypes = new List<string> // Only card available in test mode?
            {
				"card"
			},
				LineItems = new List<SessionLineItemOptions>
			{
				new SessionLineItemOptions()
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)product.Price, // Price is in USD cents.
                        Currency = "USD",
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = product.ProductName,
							Description = product.CategoryName
						},
					},
					Quantity = 1,
				},
			},
				Mode = "payment" // One-time payment. Stripe supports recurring 'subscription' payments.
			};

			var service = new SessionService();
			var session = await service.CreateAsync(options);

			return session.Id;
		}

		[HttpGet("success")]
		// Automatic query parameter handling from ASP.NET.
		// Example URL: https://localhost:7051/checkout/success?sessionId=si_123123123123
		public ActionResult CheckoutSuccess(string sessionId)
		{
			var sessionService = new SessionService();
			var session = sessionService.Get(sessionId);

			// Here you can save order and customer details to your database.
			var total = session.AmountTotal.Value;
			var customerEmail = session.CustomerDetails.Email;

			return Redirect(s_wasmClientURL + "success");
		}
	}
}
