using System;
using System.Data.Common;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XRepository.Tests
{
	/// <summary>
	/// Sorry, these tests are a bit basic and should probably be split into a comprehensive set of distinct tests.
	/// Also, I need to add tests to confirm that a transaction has or has not been committed, etc.
	/// </summary>

	[TestClass]
	public class StorageOperationTests
	{
		[TestMethod]
		public void New_storage_operation_with_default_settings_has_correct_values_at_each_depth()
		{
			using(var op = new StorageOperation())
			{
				Assert.IsNotNull(op.Connection);
				Assert.IsNotNull(op.ConnectionSource);
				Assert.IsNotNull(op.ConnectionSource.Provider);
				Assert.IsTrue(op.ConnectionSource.Provider.GetType() == typeof(FakeDbProviderFactory));
				Assert.AreEqual(1, op.Depth);

				using(var nested = new StorageOperation())
				{
					Assert.IsNotNull(nested.Connection);
					Assert.IsNotNull(nested.ConnectionSource);
					Assert.AreEqual(1, op.Depth);
					Assert.AreEqual(2, nested.Depth);
				}

				Assert.AreEqual(1, op.Depth);

				using(var trans = new AtomicStorageOperation())
				{
					Assert.IsNotNull(trans.Connection);
					Assert.IsNotNull(trans.ConnectionSource);
					Assert.AreEqual(1, op.Depth);
					Assert.AreEqual(2, trans.Depth);
					trans.Complete();
				}

				Assert.AreEqual(1, op.Depth);
			}
		}
	}
}
