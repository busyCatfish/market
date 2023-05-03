using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Data.Data
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly TradeMarketDbContext tradeMarketDbContext;

		public UnitOfWork(TradeMarketDbContext tradeMarketDbContext)
		{
			this.tradeMarketDbContext = tradeMarketDbContext;
		}

		public ICustomerRepository CustomerRepository => new CustomerRepository(tradeMarketDbContext);

		public IPersonRepository PersonRepository => new PersonRepository(tradeMarketDbContext);

		public IProductRepository ProductRepository => new ProductRepository(tradeMarketDbContext);

		public IProductCategoryRepository ProductCategoryRepository => new ProductCategoryRepository(tradeMarketDbContext);

		public IReceiptRepository ReceiptRepository => new ReceiptRepository(tradeMarketDbContext);

		public IReceiptDetailRepository ReceiptDetailRepository => new ReceiptDetailRepository(tradeMarketDbContext);

		public async Task SaveAsync()
		{
			await tradeMarketDbContext.SaveChangesAsync();
		}
	}
}
