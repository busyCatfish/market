using Data.Data;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
	public class CustomerRepository : ICustomerRepository
	{
		private readonly TradeMarketDbContext tradeMarketDbContext;

		public CustomerRepository(TradeMarketDbContext tradeMarketDbContext)
		{
			this.tradeMarketDbContext = tradeMarketDbContext;
		}

		public async Task AddAsync(Customer entity)
		{
			if (entity == null) return;
			if (await tradeMarketDbContext.Customers.FirstOrDefaultAsync(c => c.Id == entity.Id) == null)
			{
				await tradeMarketDbContext.Customers.AddAsync(entity);
				await tradeMarketDbContext.SaveChangesAsync();
			}
		}

		public void Delete(Customer entity)
		{
			tradeMarketDbContext.Customers.Remove(entity);
			tradeMarketDbContext.SaveChanges();
		}

		public async Task DeleteByIdAsync(int id)
		{
			var customer = await tradeMarketDbContext.Customers.FirstOrDefaultAsync(c => c.Id == id);

			tradeMarketDbContext.Customers.Remove(customer);
			await tradeMarketDbContext.SaveChangesAsync();
		}

		public async Task<IEnumerable<Customer>> GetAllAsync()
		{
			var customers = await tradeMarketDbContext.Customers.ToListAsync();

			return customers;
		}

		public async Task<IEnumerable<Customer>> GetAllWithDetailsAsync()
		{
			var customers = await tradeMarketDbContext.Customers.ToListAsync();

			foreach (var customer in customers)
			{
				var receipts = await tradeMarketDbContext.Receipts.Where(r => r.CustomerId == customer.Id).ToArrayAsync();

				foreach (var r in receipts)
				{
					r.ReceiptDetails = await tradeMarketDbContext.ReceiptsDetails.Where(rd => rd.ReceiptId == r.Id).ToArrayAsync();
				}

				customer.Receipts = receipts;

				customer.Person = await tradeMarketDbContext.Persons.FindAsync(customer.PersonId);
			}

			return customers;
		}

		public async Task<Customer> GetByIdAsync(int id)
		{
			var customer = await tradeMarketDbContext.Customers.FindAsync(id).AsTask();

			return customer;
		}

		public async Task<Customer> GetByIdWithDetailsAsync(int id)
		{
			var customer = await tradeMarketDbContext.Customers.FindAsync(id);

			if (customer is null)
			{
				return null;
			}

			var receipts = await tradeMarketDbContext.Receipts.Where(r => r.CustomerId == id).ToArrayAsync();

			foreach (var r in receipts)
			{
				r.ReceiptDetails = await tradeMarketDbContext.ReceiptsDetails.Where(rd => rd.ReceiptId == r.Id).ToArrayAsync();
			}

			customer.Receipts = receipts;

			customer.Person = await tradeMarketDbContext.Persons.FindAsync(customer.PersonId);

			return customer;
		}

		public void Update(Customer entity)
		{
			if (entity == null)
			{
				return;
			}

			tradeMarketDbContext.Customers.Update(entity);

			tradeMarketDbContext.SaveChanges();
		}
	}
}
