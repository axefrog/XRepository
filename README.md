XRepository
===========

XRepository is a data abstraction layer designed to simplify the process of managing connections and transactions
within any execution context, irrespective of the depth of nested methods participating in an operation and without
forcing any of those methods to need connection or transaction objects passed to them, or for them to assume that they
are participating in the context of a greater storage operation. Using XRepository, all methods that perform database
operations can be built in a way that allows them to act independently and without knowledge of whether their calling
methods have called them in the scope of another database operation, but still allow them to automatically participate
in those operations, transactional or not, if present.

XRepository is built with a code-first, convention-over-configuration approach. You can configure as much or as little
as you like, but "out of the box", you should be able to have your project's data layer up and running extremely
quickly with very few lines of code.

The two classes you will use are `StorageOperation` and `AtomicStorageOperation`. Both should be initialised with
`using` blocks to guarantee that when they are no longer required, they are disposed of immediately. Disposal allows
the storage operation to complete, closing connections, committing transactions, etc.

StorageOperation
----------------

`StorageOperation` represents an shared connection in the current execution context. A nested set of method calls can
each construct and dispose their `StorageContext` objects and each instance will automatically know whether it is part
of a greater storage operation (and thus subscribe to that operation) or whether it has been created independently and
thus should itself manage connection and disconnection from a given source. The `Connection` object is automatically
opened as soon as it is accessed the first time and closed when the initiating `StorageContext` object disposes.

`StorageOperation` should not be passed or referenced outside of the thread in which it was created. New instances of
`StorageOperation` automatically operate within the context of the current thread, which means if a secondary thread
starts a new storage operation while a second storage operation is in progress on a different thread, it will create
and manage its own connection, preventing multiple threads from interfering with each other. Thus, XRepository is
thread safe as long any given `StorageOperation` object is referenced only from within the scope and thread in which it
was created.

In summary, do not pass references to a `StorageOperation` object outside of the scope in which it was created. Nested
method calls should each construct and dispose their own `StorageOperation` object, and in doing so, XRepository will
automatically manage subscription and unsubscription of any execution context to a given connection. Also, unless you
have a very good reason for doing so, ALWAYS use `using` blocks to construct and dispose of `StorageOperation` objects.

AtomicStorageOperation
----------------------

`AtomicStorageOperation` inherits from `StorageOperation` and thus follows all of the same rules that govern the
base class. However, operations performed within the context of an `AtomicStorageOperation` instance are transactional.
To signal a successful atomic operation, call the `Complete()` method of the `AtomicStorageOperation` object in the
current scope and context. Just as with `StorageOperation`, if a given atomic operation is performed from a method that
is nested within the context of another `AtomicStorageOperation` object at a shallower level in the stack, then the
initiating `AtomicStorageOperation` instance will govern the final commit or rollback for the operation. if
`Complete()` is not called within any single `AtomicStorageOperation` instance participating in the current execution
context, then the initiating `AtomicStorageOperation` instance will cause all operations performed in that context to
roll back upon its disposal, including nested operations that are only being performed within a regular
`StorageOperation1` instance.

You can specify a particular `IsolationLevel` for an atomic operation, but it will be only be used if the
`AtomicStorageOperation` instance is the initiating instance within the current execution context. If the operation is
nested inside another atomic operation, the isolation level used will be that of the `AtomicStorageOperation` instance
that initiated the transaction.

Examples
--------
### Simple usage outside of repository to ensure contained method calls use the same connection

    using(new StorageOperation())
	{
		// work performed here ensures that calls to your repositories will all use the same connection
	}

### Usage inside of repository to obtain and use a connection

    using(var op = new StorageOperation())
	{
		// retaining a reference to the StorageOperation allows your repository to access the current Connection object
		var command = op.Connection.CreateCommand();
		
		// do work...
	}

### Using a storage operation to bring multiple methods calls into the same connection context

    public void IndependentMethod1()
	{
		using(new StorageOperation())
		{
			// do work here
		}
	}

	public void IndependentMethod2()
	{
		using(new StorageOperation())
		{
			// do work here
		}
	}

	public void PerformBulkOperation()
	{
		using(new StorageOperation())
		{
			IndependentMethod1();
			IndependentMethod2();
		}
	}

### Perform an atomic operation

    using(var op = new AtomicStorageOperation())
	{
		// do work, then if successful...

		if(successful)
			op.Complete();
	}

### Perform an atomic operation using a specific `IsolationLevel`

    using(var op = new AtomicStorageOperation(IsolationLevel.ReadUncommitted))
	{
		// do work, then if successful...

		if(successful)
			op.Complete();
	}

### Perform a bulk operation, with multiple methods participating in the same transaction

    public void IndependentMethod1()
	{
		using(var op = new AtomicStorageOperation())
		{
			// do work here
			if(successful)
				op.Complete();
		}
	}

	public void IndependentMethod2()
	{
		using(new StorageOperation())
		{
			// do work here
		}
	}

	public void PerformBulkOperation()
	{
		using(var op = new AtomicStorageOperation())
		{
			IndependentMethod1();
			IndependentMethod2();

			// if the call to IndependentMethod1 failed and op.Complete() was not called, then the transaction will
			// roll back, even though we are calling Complete() here.
			op.Complete();
		}
	}

### Perform a bulk operation, with only some parts of the operation participating in a transaction

    public void IndependentMethod1()
	{
		using(var op = new AtomicStorageOperation())
		{
			// do work here
			if(successful)
				op.Complete();
		}
	}

    public void IndependentMethod2()
	{
		using(var op = new AtomicStorageOperation())
		{
			// do work here
			if(successful)
				op.Complete();
		}
	}

	public void IndependentMethod3()
	{
		using(new StorageOperation())
		{
			// do work here
		}
	}

	public void PerformBulkOperation()
	{
		using(new StorageOperation())
		{
			// this call will complete irrespective of what comes after it, as there is no active atomic context
			IndependentMethod3();

			using(var op = new AtomicStorageOperation())
			{
				IndependentMethod1();
				IndependentMethod2();

				// if either method fails to call Complete(), then the transaction will roll back when this using
				// block exits and thus disposes
				op.Complete();
			}

			// this call will complete irrespective of the previous atomic transaction's success
			IndependentMethod3();
		}
	}
