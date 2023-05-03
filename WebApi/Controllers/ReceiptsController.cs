using Business.Interfaces;
using Business.Models;
using Business.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ReceiptsController : Controller
	{
		private readonly IReceiptService receiptService;

		public ReceiptsController(IReceiptService receiptService)
		{
			this.receiptService = receiptService;
		}

		// GET: /api/receipts
		[HttpGet]
		public async Task<ActionResult<IEnumerable<ReceiptModel>>> GetAll()
		{
			IEnumerable<ReceiptModel> receipts;
			try
			{
				receipts = await receiptService.GetAllAsync();
			}
			catch (MarketException)
			{
				return BadRequest();
			}

			if (receipts is null)
			{
				return NotFound();
			}

			return Ok(receipts);
		}

		//GET: /api/receipts/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<ReceiptModel>> GetById(int id)
		{
			ReceiptModel receipt;
			try
			{
				receipt = await receiptService.GetByIdAsync(id);
			}
			catch (MarketException)
			{
				return BadRequest();
			}

			if (receipt is null)
			{
				return NotFound();
			}

			return Ok(receipt);
		}

		//GET: /api/receipts/period?startDate=2021-12-1&endDate=2020-12-31
		[HttpGet]
		[Route("period")]
		public async Task<ActionResult<IEnumerable<ReceiptModel>>> GetReceiptsByPeriod([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
		{
			IEnumerable<ReceiptModel> receipts;
			try
			{
				receipts = await receiptService.GetReceiptsByPeriodAsync(startDate, endDate);
			}
			catch (MarketException)
			{
				return BadRequest();
			}

			if (receipts is null)
			{
				return NotFound();
			}

			return Ok(receipts);
		}

		//GET: /api/receipts/{id}/sum
		[HttpGet("{id}/sum")]
		public async Task<ActionResult<decimal>> GetSum(int id)
		{
			decimal sum = 0;
			try
			{
				sum = await receiptService.ToPayAsync(id);
			}
			catch (MarketException)
			{
				return BadRequest();
			}

			return Ok(sum);
		}


		//GET: /api/receipts/{id}/details
		[HttpGet("{id}/details")]
		public async Task<ActionResult<IEnumerable<ReceiptDetailModel>>> GetReceiptDetails(int id)
		{
			IEnumerable<ReceiptDetailModel> receiptDetails;
			try
			{
				receiptDetails = await receiptService.GetReceiptDetailsAsync(id);
			}
			catch (MarketException)
			{
				return BadRequest();
			}

			if (receiptDetails is null)
			{
				return NotFound();
			}

			return Ok(receiptDetails);
		}
		//POST: /api/receipts
		[HttpPost]
		public async Task<ActionResult> Add([FromBody] ReceiptModel value)
		{
			try
			{
				await receiptService.AddAsync(value);
			}
			catch (MarketException)
			{
				return BadRequest();
			}

			return Ok(value);
		}

		//PUT: /api/receipts/{id}
		[HttpPut("{id}")]
		public async Task<ActionResult> Update(int id, [FromBody] ReceiptModel value)
		{
			try
			{
				if (id != value.Id)
				{
					return BadRequest();
				}

				await receiptService.UpdateAsync(value);
			}
			catch (MarketException)
			{
				return BadRequest();
			}

			return Ok(value);
		}

		//PUT: /api/receipts/{id}/products/add/{productId}/{quantity}
		[HttpPut("{id}/products/add/{productId}/{quantity}")]
		public async Task<ActionResult> AddProduct(int id, int productId, int quantity, [FromBody] ReceiptDetailModel value)
		{
			try
			{
				await receiptService.AddProductAsync(productId, id, quantity);	
			}
			catch (MarketException)
			{
				return BadRequest();
			}

			return Ok();
		}

		//PUT: /api/receipts/{id}/products/remove/{productId}/{quantity}
		[HttpPut("{id}/products/remove/{productId}/{quantity}")]
		public async Task<ActionResult> RemoveProduct(int id, int productId, int quantity)
		{
			try
			{
				await receiptService.RemoveProductAsync(productId, id, quantity);
			}
			catch (MarketException)
			{
				return BadRequest();
			}

			return Ok();
		}

		//PUT: /api/receipts/{id}/checkout
		[HttpPut("{id}/checkout")]
		public async Task<ActionResult> CheckOut(int id)
		{
			try
			{
				await receiptService.CheckOutAsync(id);
			}
			catch (MarketException)
			{
				return BadRequest();
			}

			return Ok();
		}

		//DELETE: /api/receipts/{id}
		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(int id)
		{
			try
			{
				await receiptService.DeleteAsync(id);
			}
			catch (MarketException)
			{
				return BadRequest();
			}

			return Ok();
		}
	}
}
