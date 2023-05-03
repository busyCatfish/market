using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Validation
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
	public class MarketException : Exception
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
	{
		public MarketException() { }

		public MarketException(string message) : base(message) { }

		public MarketException(string message, Exception innerException) : base(message, innerException) { }
	}
}
