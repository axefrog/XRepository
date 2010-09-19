using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;

namespace XRepository
{
	public static class ConnectionStateManager
	{
		private static ConcurrentDictionary<long, OperationContext> _connections = new ConcurrentDictionary<long, OperationContext>();
		private static ConnectionSource _defaultConnectionSource;

		public static ConnectionSource DefaultConnectionSource
		{
			get
			{
				lock(_connections)
					if(_defaultConnectionSource == null)
						_defaultConnectionSource = new ConnectionSource();
				return _defaultConnectionSource;
			}
			set
			{
				lock(_connections)
					_defaultConnectionSource = value;
			}
		}

		/// <summary>
		/// Acquires a OperationContext object for the current thread using the default connection source, optionally
		/// in an atomic context. If an operation using the default connection source is in progress, it will be
		/// subscribed to, otherwise a new one will be created. If the isolation level is specified, it will be used
		/// to start a new transaction, if none exists. If an existing transaction is already active in the current
		/// context, the specified isolation level will be ignored.
		/// </summary>
		/// <param name="isolationLevel">The isolationLevel that will be used to create a new transaction, if no
		/// transaction is already active in the current context. If null, no new transaction will be started.</param>
		/// <param name="source">The connection source to use, or null to use the default connection source</param>
		/// <returns>The current operation context</returns>
		internal static OperationContext AcquireContext(IsolationLevel? isolationLevel = null, ConnectionSource source = null)
		{
			return AcquireContext(source ?? DefaultConnectionSource, isolationLevel.HasValue, isolationLevel);
		}

		/// <summary>
		/// Acquires a OperationContext object for the current thread using the default connection source, optionally
		/// in an atomic context. If an operation using the default connection source is in progress, it will be
		/// subscribed to, otherwise a new one will be created. If the isolation level is specified, it will be used
		/// to start a new transaction, if none exists. If an existing transaction is already active in the current
		/// context, the specified isolation level will be ignored.
		/// </summary>
		/// <param name="atomic">Specifies that a new transaction should be started if no transaction is already active
		/// in the current context. If false, no new transaction will be started.</param>
		/// <param name="source">The connection source to use. If null, the default connection source will be used</param>
		/// <returns>The current operation context</returns>
		internal static OperationContext AcquireContext(bool atomic, ConnectionSource source = null)
		{
			return AcquireContext(DefaultConnectionSource, atomic);
		}

		private static OperationContext AcquireContext(ConnectionSource source, bool atomic, IsolationLevel? isolationLevel = null)
		{
			OperationContext context;
			var hash = source.GetHashForThread();
			// get the existing context for the current thread
			if(!_connections.TryGetValue(Thread.CurrentThread.ManagedThreadId, out context))
			{
				// start a new context
				context = new OperationContext
				{
					Source = source,
					Hash = hash,
				};
				// try and add the new context (should work every time unless there is some kind of unpredictable race condition in play)
				if(!_connections.TryAdd(hash, context))
					context = _connections[hash]; // this line should never be reached, but if it is and it fails, some rethinking of this code is probably in order...
			}
			context.OperationCount++;
			if(atomic)
			{
				if(context.AtomicOperationCount == 0)
					context.Transaction = isolationLevel.HasValue ? context.Connection.BeginTransaction(isolationLevel.Value) : context.Connection.BeginTransaction();
				context.AtomicOperationCount++;
			}
			return context;
		}

		internal static void ReleaseContext(OperationContext context, bool atomic = false)
		{
			// if this is an atomic operation (a subset of the overall operation), handle the release of the subscriber from the operation
			if(atomic)
			{
				if(context.Transaction == null)
					throw new InvalidOperationException("Cannot call ReleaseContext in an atomic context when there is no attached transaction");
				if(context.AtomicOperationCount <= 0)
					throw new InvalidOperationException("Unexpected error: ReleaseContext was called in an atomic context and there was an attached transaction, but the AtomicOperationCount was " + context.AtomicOperationCount);
				
				// decrement the atomic counter to keep track of how many subscribers are still making use of the transaction
				context.AtomicOperationCount--;
				
				// if this was the last subscriber to the transaction, we can finalise the transaction, depending on how the operation went
				if(context.AtomicOperationCount == 0)
				{
					try
					{
						if(context.RollbackRequested)
							context.Transaction.Rollback();
						else
							context.Transaction.Commit();
					}
					catch(Exception ex)
					{
						throw new Exception("Unable to finalise the transaction for the current storage operation: " + ex.Message, ex);
					}
					context.Transaction = null;
				}
			}

			// decrement the overall operation count for the current context
			context.OperationCount--;
			if(context.OperationCount == 0)
			{
				if(context.IsConnectionInitialised && context.Connection.State == ConnectionState.Open)
					context.Connection.Close();
				OperationContext temp;
				_connections.TryRemove(context.Hash, out temp);
			}
		}
	}
}
