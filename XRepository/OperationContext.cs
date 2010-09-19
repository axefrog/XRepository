using System;
using System.Data.Common;

namespace XRepository
{
	/// <summary>
	/// OperationContext is thread-dependent and must NOT be used outside of the thread in which it was created.
	/// </summary>
	internal class OperationContext
	{
		private DbConnection _connection;
		public DbConnection Connection
		{
			get
			{
				if(_connection == null)
				{
					_connection = Source.CreateConnection();
					try
					{
						_connection.Open();
					}
					catch(Exception ex)
					{
						throw new Exception("Unable to open a connection using the specified source connection string: " + ex.Message, ex);
					}
				}
				return _connection;
			}
		}

		internal bool IsConnectionInitialised
		{
			get { return _connection != null; }
		}

		public DbTransaction Transaction { get; set; }
		public long Hash { get; set; }
		public ConnectionSource Source { get; set; }
		public int OperationCount { get; set; }
		public int AtomicOperationCount { get; set; }
		
		private bool _rollbackRequested;
		public bool RollbackRequested
		{
			get { return _rollbackRequested; }
		}
		public void RequestRollback()
		{
			_rollbackRequested = true;
		}
	}
}