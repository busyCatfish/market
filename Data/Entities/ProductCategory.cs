using System.Collections;
using System.Collections.Generic;

namespace Data.Entities
{
	public class ProductCategory : BaseEntity
	{
		public string CategoryName { get; set; }

		public ICollection<Product> Products { get; set; }
	}
}