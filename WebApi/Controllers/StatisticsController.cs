using Business.Interfaces;
using Business.Models;
using Business.Services;
using Business.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class StatisticsController : Controller
	{
		private readonly IStatisticService statisticService;

		public StatisticsController(IStatisticService statisticService)
		{
			this.statisticService = statisticService;
		}

		//GET: /api/statistic/popularProducts?productCount=2
		[HttpGet]
		[Route("popularProducts")]
		public async Task<ActionResult<IEnumerable<ProductModel>>> GetMostPopularProducts([FromQuery]int productCount)
		{
			IEnumerable<ProductModel> products;
			try
			{
				products = await statisticService.GetMostPopularProductsAsync(productCount);
			}
			catch (MarketException)
			{
				return BadRequest();
			}

			if (products is null)
			{
				return NotFound();
			}

			return Ok(products);
		}

		//GET: /api/statisic/customer/{id}/{productCount}
		[HttpGet("customer/{id}/{productCount}")]
		public async Task<ActionResult<IEnumerable<ProductModel>>> GetCustomersMostPopularProducts(int id, int productCount)
		{
			IEnumerable<ProductModel> products;
			try
			{
				products = await statisticService.GetCustomersMostPopularProductsAsync(productCount, id);
			}
			catch (MarketException)
			{
				return BadRequest();
			}

			if (products is null)
			{
				return NotFound();
			}

			return Ok(products);
		}

		//GET: /api/statistic/activity/{customerCount}?startDate= 2020-7-21&endDate= 2020-7-22
		[HttpGet("activity/{customerCount}")]
		public async Task<ActionResult<IEnumerable<CustomerActivityModel>>> GetMostValuableCustomers(int customerCount, 
			[FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
		{
			IEnumerable<CustomerActivityModel> customers;
			try
			{
				customers = await statisticService.GetMostValuableCustomersAsync(customerCount, startDate, endDate);
			}
			catch (MarketException)
			{
				return BadRequest();
			}

			if (customers is null)
			{
				return NotFound();
			}

			return Ok(customers);
		}

		//GET: /api/statistic/income/{categoryId}?startDate= 2020-7-21&endDate= 2020-7-22
		[HttpGet("income/{categoryId}")]
		public async Task<ActionResult<decimal>> GetIncomeOfCategoryInPeriod(int categoryId,
			 [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
		{
			decimal income;
			try
			{
				income = await statisticService.GetIncomeOfCategoryInPeriod(categoryId, startDate, endDate);
			}
			catch (MarketException)
			{
				return BadRequest();
			}

			return Ok(income);
		}
	}
}
