using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
	public class StatisticService : IStatisticService
	{
		public StatisticService(IUnitOfWork @object, IMapper mapper)
		{
			Object = @object;
			Mapper = mapper;
		}

		public IUnitOfWork Object { get; }
		public IMapper Mapper { get; }

		public async Task<IEnumerable<ProductModel>> GetCustomersMostPopularProductsAsync(int productCount, int customerId)
		{
			var receipts = await Object.ReceiptRepository.GetAllWithDetailsAsync();

			var receiptDetails = receipts.Where(r => r.CustomerId == customerId).Select(r => r.ReceiptDetails);

			List<ProductWithQuantity> products = new List<ProductWithQuantity>();

			foreach (var rdCollection in receiptDetails)
			{
				foreach (ReceiptDetail rd in rdCollection)
				{
					var p = products.FirstOrDefault(pr => pr.Product.Id == rd.ProductId);
					if (p != null)
					{
						p.Quantity += rd.Quantity;
					}
					else products.Add(new ProductWithQuantity(rd.Product, rd.Quantity));
				}
			}

			var productsModel = Mapper.Map<IEnumerable<ProductModel>>(products.OrderByDescending(p => p.Quantity).Select(p => p.Product));

			return productsModel.Take(productCount).ToList();
		}

		public async Task<decimal> GetIncomeOfCategoryInPeriod(int categoryId, DateTime startDate, DateTime endDate)
		{
			var receipts = await Object.ReceiptRepository.GetAllWithDetailsAsync();

			decimal income = 0m;

			foreach (var receipt in receipts.Where(r => r.OperationDate >= startDate && r.OperationDate <= endDate))
			{
				foreach (ReceiptDetail rd in receipt.ReceiptDetails)
				{
					if (rd.Product.ProductCategoryId == categoryId)
					{
						income += rd.DiscountUnitPrice * rd.Quantity;
					}
				}
			}

			return income;
		}

		public async Task<IEnumerable<ProductModel>> GetMostPopularProductsAsync(int productCount)
		{
			var receiptDetails = await Object.ReceiptDetailRepository.GetAllWithDetailsAsync();

			List<ProductWithQuantity> products = new List<ProductWithQuantity>();

			foreach (ReceiptDetail rd in receiptDetails)
			{
				var p = products.FirstOrDefault(pr => pr.Product.Id == rd.ProductId);
				if (p != null)
				{
					p.Quantity += rd.Quantity;
				}
				else products.Add(new ProductWithQuantity(rd.Product, rd.Quantity));
			}

			var productsModel = Mapper.Map<IEnumerable<ProductModel>>(products.OrderByDescending(p => p.Quantity).Select(p => p.Product));

			return productsModel.Take(productCount).ToList();
		}

		public async Task<IEnumerable<CustomerActivityModel>> GetMostValuableCustomersAsync(int customerCount, DateTime startDate, DateTime endDate)
		{
			var receipts = await Object.ReceiptRepository.GetAllWithDetailsAsync();

			Dictionary<Customer, decimal> customersWithSum = new Dictionary<Customer, decimal>();

			foreach (var receipt in receipts.Where(r => r.OperationDate >= startDate && r.OperationDate <= endDate).GroupBy(r => r.CustomerId))
			{
				Customer customer = receipt.Select(r => r.Customer).FirstOrDefault();

				foreach (var rdCollection in receipt.Select(r => r.ReceiptDetails))
				{
					decimal ReceiptSum = rdCollection.Sum(rd => rd.DiscountUnitPrice * rd.Quantity);

					if (customersWithSum.ContainsKey(customer))
					{
						customersWithSum[customer] += ReceiptSum;
					}
					else customersWithSum.Add(customer, ReceiptSum);
				}
			}
			var customersModel = new List<CustomerActivityModel>();

			foreach (var customer in customersWithSum.OrderByDescending(c => c.Value).Take(customerCount))
			{
				customersModel.Add(new CustomerActivityModel()
				{
					CustomerId = customer.Key.Id,
					CustomerName = $"{customer.Key.Person.Name} {customer.Key.Person.Surname}",
					ReceiptSum = customer.Value
				});
			}

			return customersModel.ToList();
		}
	}

	public class ProductWithQuantity
	{
		public Product Product { get; set; }
		public int Quantity { get; set; }

		public ProductWithQuantity(Product product, int quantity)
		{
			this.Product = product;
			this.Quantity = quantity;
		}
	}
}
