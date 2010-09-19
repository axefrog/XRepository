using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace XRepository
{
	public class StorageOperation : IDisposable
	{
		private readonly bool _atomic;
		internal OperationContext Context { get; private set; }

		private bool _disposed;

		public StorageOperation(ConnectionSource source = null) : this(false, source)
		{
		}

		protected StorageOperation(bool atomic, ConnectionSource source = null, IsolationLevel? isolationLevel = null)
		{
			_atomic = atomic;
			if(atomic && isolationLevel.HasValue)
				Context = ConnectionStateManager.AcquireContext(isolationLevel, source);
			else
				Context = ConnectionStateManager.AcquireContext(atomic, source);
			if(Context.OperationCount == 1)
				IsInitiatingOperation = true;
			Depth = Context.OperationCount;
		}

		public bool IsInitiatingOperation { get; private set; }
		public int Depth { get; private set; }

		private const string DisposedExceptionMessage = "StorageOperation object should not be accessed after it is disposed";
		public ConnectionSource ConnectionSource
		{
			get
			{
				if(_disposed)
					throw new Exception(DisposedExceptionMessage);
				return Context.Source;
			}
		}

		public DbConnection Connection
		{
			get
			{
				if(_disposed)
					throw new Exception(DisposedExceptionMessage);
				return Context.Connection;
			}
		}

		public void Release()
		{
			if(_disposed)
				throw new Exception(DisposedExceptionMessage);
			ConnectionStateManager.ReleaseContext(Context);
			_disposed = true;
			Context = null;
		}

		public virtual void Dispose()
		{
			if(_disposed)
				return;
			Release();
		}
	}
}
