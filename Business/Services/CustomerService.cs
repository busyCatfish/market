using Business.Interfaces;
using Business.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Data;
using Business.Validation;
using Data.Interfaces;
using AutoMapper;
using Data.Entities;
using System.Reflection;
using System.Linq;

namespace Business.Services
{
	public class CustomerService : ICustomerService
	{
		public CustomerService(IUnitOfWork @object, IMapper mapper)
		{
			Object = @object;
			Mapper = mapper;
		}

		public IUnitOfWork Object { get; }
		public IMapper Mapper { get; }

		public async Task AddAsync(CustomerModel model)
		{
			CheckPropertyOfCustomerModel(model);


			Customer customer = Mapper.Map<Customer>(model);

			await Object.CustomerRepository.AddAsync(customer);

			await Object.SaveAsync();
		}

		public async Task DeleteAsync(int modelId)
		{
			await Object.CustomerRepository.DeleteByIdAsync(modelId);

			await Object.SaveAsync();
		}

		public async Task<IEnumerable<CustomerModel>> GetAllAsync()
		{
			var customers = await Object.CustomerRepository.GetAllWithDetailsAsync();

			var customersModel = Mapper.Map<IEnumerable<CustomerModel>>(customers);

			return customersModel;
		}

		public async Task<CustomerModel> GetByIdAsync(int id)
		{
			var customer = await Object.CustomerRepository.GetByIdWithDetailsAsync(id);

			var customerModel = Mapper.Map<CustomerModel>(customer);

			return customerModel;
		}

		public async Task<IEnumerable<CustomerModel>> GetCustomersByProductIdAsync(int productId)
		{
			var customers = await Object.CustomerRepository.GetAllWithDetailsAsync();

			List<Customer> result = new List<Customer>();
			foreach (var c in customers)
			{
				foreach (var r in c.Receipts)
				{
					foreach (var rd in r.ReceiptDetails)
					{
						if (rd.ProductId == productId)
						{
							result.Add(c);
							break;
						}
					}
				}
			}

			var resultModel = Mapper.Map<IEnumerable<CustomerModel>>(result.Distinct());

			return resultModel;
		}

		public async Task UpdateAsync(CustomerModel model)
		{
			CheckPropertyOfCustomerModel(model);

			Customer customer = Mapper.Map<Customer>(model);

			Object.PersonRepository.Update(customer.Person);

			Object.CustomerRepository.Update(customer);

			await Object.SaveAsync();
		}

		private void CheckPropertyOfCustomerModel(CustomerModel model)
		{
			if (model == null) throw new MarketException("Customer cannot be null");

			if (string.IsNullOrEmpty(model.Name)) throw new MarketException("Name cannot be null or empty");

			if (string.IsNullOrEmpty(model.Surname)) throw new MarketException("Name cannot be null or empty");

			if (model.BirthDate > DateTime.Now) throw new MarketException("Incorrect Birth date");

			if (model.BirthDate < DateTime.Now.AddYears(-80)) throw new MarketException("Incorrect Birth date");

			if (model.DiscountValue < 0) throw new MarketException("Discount cannot be less than 0");
		}
	}
}
