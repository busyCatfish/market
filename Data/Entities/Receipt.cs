using System;
using System.Collections;
using System.Collections.Generic;

namespace Data.Entities
{
	public class Receipt : BaseEntity
	{
        public int CustomerId { get; set; }

		public DateTime OperationDate { get; set; }

		public bool IsCheckedOut { get; set; }

		public Customer Customer { get; set; }

		public ICollection<ReceiptDetail> ReceiptDetails { get; set; }
    }
}