using Business.Interfaces;
using Business.Models;
using Business.Services;
using Business.Validation;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Data.Entities;
using System.Linq;

namespace WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductsController : Controller
	{
		private readonly IProductService productService;

		public ProductsController(IProductService productService)
		{
			this.productService = productService;
		}

		// GET: /api/products
		[HttpGet]
		[Route("all")]
		public async Task<ActionResult<IEnumerable<ProductModel>>> Get()
		{
			IEnumerable<ProductModel> products;
			try
			{
				products = await productService.GetAllAsync();
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

		//GET: /api/products/1
		[HttpGet("{id}")]
		public async Task<ActionResult<ProductModel>> GetById(int id)
		{
			ProductModel product;
			try
			{
				product = await productService.GetByIdAsync(id);
			}
			catch (MarketException)
			{
				return BadRequest();
			}

			if (product is null)
			{
				return NotFound();
			}

			return Ok(product);
		}

		//GET: /api/products?categoryId=1&minPrice=20&maxPrice=50
		[HttpGet]
		public async Task<ActionResult<IEnumerable<ProductModel>>> GetByFilter([FromQuery] FilterSearchModel request)
		{
			IEnumerable<ProductModel> products;
			try
			{
				products = await productService.GetByFilterAsync(request);
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

		//GET: /api/products/categories
		[HttpGet]
		[Route("categories")]
		public async Task<ActionResult<IEnumerable<ProductCategoryModel>>> GetAllCategories()
		{
			IEnumerable<ProductCategoryModel> categories;
			try
			{
				categories = await productService.GetAllProductCategoriesAsync();
			}
			catch (MarketException)
			{
				return BadRequest();
			}

			if (categories is null)
			{
				return NotFound();
			}

			return Ok(categories);
		}

		//POST: /api/products
		[HttpPost]
		public async Task<ActionResult> Add([FromBody] ProductModel value)
		{
			try
			{
				await productService.AddAsync(value);
			}
			catch (MarketException)
			{
				return BadRequest();
			}

			return Ok(value);
		}

		//POST: /api/products/categories
		[HttpPost]
		[Route("categories")]
		public async Task<ActionResult> AddCategory([FromBody] ProductCategoryModel value)
		{
			try
			{
				await productService.AddCategoryAsync(value);
			}
			catch (MarketException)
			{
				return BadRequest();
			}

			return Ok(value);
		}

		//PUT: /api/products/{id}
		[HttpPut("{id}")]
		public async Task<ActionResult> Update(int id, [FromBody] ProductModel value)
		{
			try
			{

				if (id != value.Id)
				{
					return BadRequest();
				}

				await productService.UpdateAsync(value);
			}
			catch (MarketException)
			{
				return BadRequest();
			}

			return Ok(value);
		}

		//PUT: /api/products/categories{id}
		//[Route("categories")]
		[HttpPut("categories/{id}")]
		public async Task<ActionResult> UpdateCategory(int id, [FromBody] ProductCategoryModel value)
		{
			try
			{

				if (id != value.Id)
				{
					return BadRequest();
				}

				await productService.UpdateCategoryAsync(value);
			}
			catch (MarketException)
			{
				return BadRequest();
			}

			return Ok(value);
		}

		//DELETE: /api/products/{id}
		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(int id)
		{
			try
			{
				await productService.DeleteAsync(id);
			}
			catch (MarketException)
			{
				return BadRequest();
			}

			return Ok();
		}

		//DELETE: /api/products/categories/{id}
		//[Route("categories")]
		[HttpDelete("categories/{id}")]
		public async Task<ActionResult> DeleteCategory(int id)
		{
			try
			{
				await productService.RemoveCategoryAsync(id);
			}
			catch (MarketException)
			{
				return BadRequest();
			}

			return Ok();
		}
	}
}
