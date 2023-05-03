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
	public class PersonRepository : IPersonRepository
	{
		private readonly TradeMarketDbContext tradeMarketDbContext;

		public PersonRepository(TradeMarketDbContext tradeMarketDbContext)
		{
			this.tradeMarketDbContext = tradeMarketDbContext;
		}
		public async Task AddAsync(Person entity)
		{
			if (entity == null) return;

			if (await tradeMarketDbContext.Persons.FirstOrDefaultAsync(c => c.Id == entity.Id) == null)
			{
				await tradeMarketDbContext.Persons.AddAsync(entity);
				await tradeMarketDbContext.SaveChangesAsync();
			}
		}

		public void Delete(Person entity)
		{
			tradeMarketDbContext.Persons.Remove(entity);
			tradeMarketDbContext.SaveChanges();
		}

		public async Task DeleteByIdAsync(int id)
		{
			var person = await tradeMarketDbContext.Persons.FirstOrDefaultAsync(c => c.Id == id);

			tradeMarketDbContext.Persons.Remove(person);
			await tradeMarketDbContext.SaveChangesAsync();
		}

		public async Task<IEnumerable<Person>> GetAllAsync()
		{
			var persons = await tradeMarketDbContext.Persons.ToListAsync();

			return persons;
		}

		public async Task<Person> GetByIdAsync(int id)
		{
			var person = await tradeMarketDbContext.Persons.FindAsync(id);

			return person;
		}

		public void Update(Person entity)
		{
			if (entity == null)
			{
				return;
			}

			tradeMarketDbContext.Persons.Update(entity);

			tradeMarketDbContext.SaveChanges();
		}
	}
}
