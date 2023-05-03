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
	public class ReceiptDetailRepository : IReceiptDetailRepository
	{
		private readonly TradeMarketDbContext tradeMarketDbContext;

		public ReceiptDetailRepository(TradeMarketDbContext tradeMarketDbContext)
		{
			this.tradeMarketDbContext = tradeMarketDbContext;
		}
		public async Task AddAsync(ReceiptDetail entity)
		{
			if (entity == null) return;

			if (await tradeMarketDbContext.ReceiptsDetails.FirstOrDefaultAsync(c => c.Id == entity.Id) == null)
			{
				await tradeMarketDbContext.ReceiptsDetails.AddAsync(entity);
				await tradeMarketDbContext.SaveChangesAsync();
			}
		}

		public void Delete(ReceiptDetail entity)
		{
			tradeMarketDbContext.ReceiptsDetails.Remove(entity);
			tradeMarketDbContext.SaveChanges();
		}

		public async Task DeleteByIdAsync(int id)
		{
			var detail = await tradeMarketDbContext.ReceiptsDetails.FirstOrDefaultAsync(c => c.Id == id);

			tradeMarketDbContext.ReceiptsDetails.Remove(detail);
			await tradeMarketDbContext.SaveChangesAsync();
		}

		public async Task<IEnumerable<ReceiptDetail>> GetAllAsync()
		{
			var details = await tradeMarketDbContext.ReceiptsDetails.ToArrayAsync();

			return details;
		}

		public async Task<IEnumerable<ReceiptDetail>> GetAllWithDetailsAsync()
		{
			var details = await tradeMarketDbContext.ReceiptsDetails.ToListAsync();

			foreach (var detail in details)
			{
				var product = await tradeMarketDbContext.Products.FindAsync(detail.ProductId);
				
				product.Category = await tradeMarketDbContext.ProductCategories.FindAsync(product.ProductCategoryId);
				
				detail.Product = product;

				detail.Receipt = await tradeMarketDbContext.Receipts.FindAsync(detail.ReceiptId);
			}

			return details;
		}

		public async Task<ReceiptDetail> GetByIdAsync(int id)
		{
			var detail = await tradeMarketDbContext.ReceiptsDetails.FindAsync(id).AsTask();

			return detail;
		}

		public void Update(ReceiptDetail entity)
		{
			if (entity == null)
			{
				return;
			}

			tradeMarketDbContext.ReceiptsDetails.Update(entity);

			tradeMarketDbContext.SaveChanges();
		}
	}
}
