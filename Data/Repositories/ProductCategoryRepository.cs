using Data.Data;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
	public class ProductCategoryRepository : IProductCategoryRepository
	{
		private readonly TradeMarketDbContext tradeMarketDbContext;

		public ProductCategoryRepository(TradeMarketDbContext tradeMarketDbContext)
		{
			this.tradeMarketDbContext = tradeMarketDbContext;
		}
		public async Task AddAsync(ProductCategory entity)
		{
			if (entity == null) return;

			if (await tradeMarketDbContext.ProductCategories.FirstOrDefaultAsync(c => c.Id == entity.Id) == null)
			{
				await tradeMarketDbContext.ProductCategories.AddAsync(entity);
				await tradeMarketDbContext.SaveChangesAsync();
			}
		}

		public void Delete(ProductCategory entity)
		{
			tradeMarketDbContext.ProductCategories.Remove(entity);
			tradeMarketDbContext.SaveChanges();
		}

		public async Task DeleteByIdAsync(int id)
		{
			var category = await tradeMarketDbContext.ProductCategories.FirstOrDefaultAsync(c => c.Id == id);

			tradeMarketDbContext.ProductCategories.Remove(category);
			await tradeMarketDbContext.SaveChangesAsync();
		}

		public async Task<IEnumerable<ProductCategory>> GetAllAsync()
		{
			var categories = await tradeMarketDbContext.ProductCategories.ToListAsync();

			return categories;
		}

		public async Task<ProductCategory> GetByIdAsync(int id)
		{
			var category = await tradeMarketDbContext.ProductCategories.FindAsync(id);

			return category;
		}

		public void Update(ProductCategory entity)
		{
			if (entity == null)
			{
				return;
			}

			tradeMarketDbContext.ProductCategories.Update(entity);

			tradeMarketDbContext.SaveChanges();
		}
	}
}
