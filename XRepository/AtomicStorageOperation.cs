using System.Data;
using System.Data.Common;

namespace XRepository
{
	public class AtomicStorageOperation : StorageOperation
	{
		private bool _completed;

		public AtomicStorageOperation(IsolationLevel? isolationLevel = null, ConnectionSource source = null) : base(true, source, isolationLevel)
		{
		}

		public DbTransaction Transaction
		{
			get { return Context.Transaction; }
		}

		public void Complete()
		{
			_completed = true;
		}

		public override void Dispose()
		{
			if(!_completed)
				Context.RequestRollback();
			base.Dispose();
		}
	}
}