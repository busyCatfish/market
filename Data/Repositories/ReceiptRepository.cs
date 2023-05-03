using Data.Data;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
	public class ReceiptRepository : IReceiptRepository
	{
		private readonly TradeMarketDbContext tradeMarketDbContext;

		public ReceiptRepository(TradeMarketDbContext tradeMarketDbContext)
		{
			this.tradeMarketDbContext = tradeMarketDbContext;
		}
		public async Task AddAsync(Receipt entity)
		{
			if (entity == null) return;

			if (await tradeMarketDbContext.Receipts.FirstOrDefaultAsync(c => c.Id == entity.Id) == null)
			{
				await tradeMarketDbContext.Receipts.AddAsync(entity);
				await tradeMarketDbContext.SaveChangesAsync();
			}
		}

		public void Delete(Receipt entity)
		{
			tradeMarketDbContext.Receipts.Remove(entity);
			tradeMarketDbContext.SaveChanges();
		}

		public async Task DeleteByIdAsync(int id)
		{
			var receipt = await tradeMarketDbContext.Receipts.FirstOrDefaultAsync(c => c.Id == id);

			tradeMarketDbContext.Receipts.Remove(receipt);
			await tradeMarketDbContext.SaveChangesAsync();
		}

		public async Task<IEnumerable<Receipt>> GetAllAsync()
		{
			var receipts = await tradeMarketDbContext.Receipts.ToListAsync();

			return receipts;
		}

		public async Task<IEnumerable<Receipt>> GetAllWithDetailsAsync()
		{
			var receipts = await tradeMarketDbContext.Receipts.ToListAsync();

            foreach (var receipt in receipts)
            {
				var receiptsDetails = await tradeMarketDbContext.ReceiptsDetails.Where(rd => rd.ReceiptId == receipt.Id).ToListAsync();

				foreach (var rd in receiptsDetails)
				{
					var product = await tradeMarketDbContext.Products.FindAsync(rd.ProductId);
					product.Category = await tradeMarketDbContext.ProductCategories.FindAsync(product.ProductCategoryId);
					rd.Product = product;
				}

				receipt.ReceiptDetails = receiptsDetails;

				receipt.Customer = await tradeMarketDbContext.Customers.FindAsync(receipt.CustomerId);

				receipt.Customer.Person = await tradeMarketDbContext.Persons.FindAsync(receipt.Customer.PersonId);
			}

            return receipts;
		}

		public async Task<Receipt> GetByIdAsync(int id)
		{
			var receipt = await tradeMarketDbContext.Receipts.FindAsync(id);

			return receipt;
		}

		public async Task<Receipt> GetByIdWithDetailsAsync(int id)
		{
			var receipt = await tradeMarketDbContext.Receipts.FindAsync(id);

			if(receipt == null)
			{
				return null;
			}

			var receiptsDetails = await tradeMarketDbContext.ReceiptsDetails.Where(rd => rd.ReceiptId == id).ToListAsync();

            foreach (var rd in receiptsDetails)
            {
				var product = await tradeMarketDbContext.Products.FindAsync(rd.ProductId);
				product.Category = await tradeMarketDbContext.ProductCategories.FindAsync(product.ProductCategoryId);
				rd.Product = product;
			}

            receipt.ReceiptDetails = receiptsDetails;

			receipt.Customer = await tradeMarketDbContext.Customers.FindAsync(receipt.CustomerId);			

			return receipt;
		}

		public void Update(Receipt entity)
		{
			if (entity == null)
			{
				return;
			}

			tradeMarketDbContext.Receipts.Update(entity);

			tradeMarketDbContext.SaveChanges();
		}
	}
}
