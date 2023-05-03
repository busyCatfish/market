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
	public class ProductRepository : IProductRepository
	{
		private readonly TradeMarketDbContext tradeMarketDbContext;

		public ProductRepository(TradeMarketDbContext tradeMarketDbContext)
		{
			this.tradeMarketDbContext = tradeMarketDbContext;
		}

		public async Task AddAsync(Product entity)
		{
			if (entity == null) return;
			if (await tradeMarketDbContext.Products.FirstOrDefaultAsync(c => c.Id == entity.Id) == null)
			{
				await tradeMarketDbContext.Products.AddAsync(entity);
				
				await tradeMarketDbContext.SaveChangesAsync();

			}
		}

		public void Delete(Product entity)
		{
			tradeMarketDbContext.Products.Remove(entity);
			tradeMarketDbContext.SaveChanges();
		}

		public async Task DeleteByIdAsync(int id)
		{
			var product = await tradeMarketDbContext.Products.FirstOrDefaultAsync(c => c.Id == id);

			tradeMarketDbContext.Products.Remove(product);
			await tradeMarketDbContext.SaveChangesAsync();
		}

		public async Task<IEnumerable<Product>> GetAllAsync()
		{
			var products = await tradeMarketDbContext.Products.ToListAsync();

			return products;
		}

		public async Task<IEnumerable<Product>> GetAllWithDetailsAsync()
		{
			var products = await tradeMarketDbContext.Products.ToListAsync();

			foreach (var product in products)
			{
				var receiptsDetails = await tradeMarketDbContext.ReceiptsDetails.Where(r => r.ProductId == product.Id).ToArrayAsync();

				product.ReceiptDetails = receiptsDetails;

				product.Category = await tradeMarketDbContext.ProductCategories.FindAsync(product.ProductCategoryId);
			}

			return products;
		}

		public async Task<Product> GetByIdAsync(int id)
		{
			var product = await tradeMarketDbContext.Products.FindAsync(id).AsTask();

			return product;
		}

		public async Task<Product> GetByIdWithDetailsAsync(int id)
		{
			var product = await tradeMarketDbContext.Products.FindAsync(id);

			if(product is null)
			{
				return null;
			}

			var receiptsDetails = await tradeMarketDbContext.ReceiptsDetails.Where(r => r.ProductId == id).ToArrayAsync();

			product.ReceiptDetails = receiptsDetails;

			product.Category = await tradeMarketDbContext.ProductCategories.FindAsync(product.ProductCategoryId);

			return product;
		}

		public void Update(Product entity)
		{
			if (entity == null)
			{
				return;
			}

			tradeMarketDbContext.Products.Update(entity);

			tradeMarketDbContext.SaveChanges();
		}
	}
}
