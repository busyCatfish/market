using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Interfaces;
using Business.Models;
using Business.Validation;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService customerService;

		public CustomersController(ICustomerService customerService)
		{
			this.customerService = customerService;
		}


		// GET: api/customers
		[HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerModel>>> Get()
        {
            IEnumerable<CustomerModel> customers;
            try
            {
                customers = await customerService.GetAllAsync();
            }
            catch(MarketException)
            {
                return BadRequest();
            }

            if (customers is null)
            {
                return NotFound();
            }

            return Ok(customers);
        }

        //GET: api/customers/1
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerModel>> GetById(int id)
        {
			CustomerModel customer;
			try
			{
				customer = await customerService.GetByIdAsync(id);
			}
			catch (MarketException)
			{
				return BadRequest();
			}

			if (customer is null)
            {
                return NotFound();
            }

            return Ok(customer);
        }
        
        //GET: api/customers/products/1
        [HttpGet("products/{id}")]
        public async Task<ActionResult<CustomerModel>> GetByProductId(int id)
        {
			IEnumerable<CustomerModel> customers;
			try
			{
				customers = await customerService.GetCustomersByProductIdAsync(id);
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

        // POST: api/customers
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] CustomerModel value)
        {
			try
			{
				await customerService.AddAsync(value);
			}
			catch (MarketException)
			{
				return BadRequest();
			}

			return Ok(value);
        }

        // PUT: api/customers/1
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int Id, [FromBody] CustomerModel value)
        {
			try
			{
				if(Id != value.Id)
				{
					return BadRequest();
				}

				await customerService.UpdateAsync(value);
			}
			catch (MarketException)
			{
				return BadRequest();
			}

			return Ok(value);
		}

        // DELETE: api/customers/1
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
			try
			{
				await customerService.DeleteAsync(id);
			}
			catch (MarketException)
			{
				return BadRequest();
			}

			return Ok();
        }
    }
}
