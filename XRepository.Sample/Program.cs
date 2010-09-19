using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRepository.Sample
{
	class Program
	{
		static void Main(string[] args)
		{
			using(new StorageOperation())
			{
			}
		}
	}

	class Customer
	{
		public int ID { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateTime DateCreated { get; set; }
	}
}
