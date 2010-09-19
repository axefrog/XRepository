using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace XRepository.Tests
{
	public class FakeDbConnection : DbConnection
	{
		private string _dataSource;
		private string _database;

		private string _serverVersion;

		private ConnectionState _state;

		/// <summary>
		/// Gets or sets the string used to open the connection.
		/// </summary>
		/// <returns>
		/// The connection string used to establish the initial connection. The exact contents of the connection string depend on the specific data source for this connection. The default value is an empty string.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override string ConnectionString { get; set; }

		/// <summary>
		/// Gets the name of the current database after a connection is opened, or the database name specified in the connection string before the connection is opened.
		/// </summary>
		/// <returns>
		/// The name of the current database or the name of the database to be used after a connection is opened. The default value is an empty string.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override string Database
		{
			get { return _database; }
		}

		/// <summary>
		/// Gets a string that describes the state of the connection.
		/// </summary>
		/// <returns>
		/// The state of the connection. The format of the string returned depends on the specific type of connection you are using.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override ConnectionState State
		{
			get { return _state; }
		}

		/// <summary>
		/// Gets the name of the database server to which to connect.
		/// </summary>
		/// <returns>
		/// The name of the database server to which to connect. The default value is an empty string.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override string DataSource
		{
			get { return _dataSource; }
		}

		/// <summary>
		/// Gets a string that represents the version of the server to which the object is connected.
		/// </summary>
		/// <returns>
		/// The version of the database. The format of the string returned depends on the specific type of connection you are using.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override string ServerVersion
		{
			get { return _serverVersion; }
		}

		/// <summary>
		/// Starts a database transaction.
		/// </summary>
		/// <returns>
		/// An object representing the new transaction.
		/// </returns>
		/// <param name="isolationLevel">Specifies the isolation level for the transaction.</param>
		protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
		{
			return new FakeDbTransaction(this, isolationLevel);
		}

		/// <summary>
		/// Closes the connection to the database. This is the preferred method of closing any open connection.
		/// </summary>
		/// <exception cref="T:System.Data.Common.DbException">The connection-level error that occurred while opening the connection. </exception><filterpriority>1</filterpriority>
		public override void Close()
		{
		}

		/// <summary>
		/// Changes the current database for an open connection.
		/// </summary>
		/// <param name="databaseName">Specifies the name of the database for the connection to use.</param><filterpriority>2</filterpriority>
		public override void ChangeDatabase(string databaseName)
		{
		}

		/// <summary>
		/// Opens a database connection with the settings specified by the <see cref="P:System.Data.Common.DbConnection.ConnectionString"/>.
		/// </summary>
		/// <filterpriority>1</filterpriority>
		public override void Open()
		{
		}

		/// <summary>
		/// Creates and returns a <see cref="T:System.Data.Common.DbCommand"/> object associated with the current connection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Data.Common.DbCommand"/> object.
		/// </returns>
		protected override DbCommand CreateDbCommand()
		{
			return new FakeDbCommand();
		}
	}

	public class FakeDbTransaction : DbTransaction
	{
		private readonly FakeDbConnection _conn;
		private readonly IsolationLevel _isolationLevel;

		public FakeDbTransaction(FakeDbConnection conn, IsolationLevel isolationLevel)
		{
			_conn = conn;
			_isolationLevel = isolationLevel;
		}

		/// <summary>
		/// Specifies the <see cref="T:System.Data.Common.DbConnection"/> object associated with the transaction.
		/// </summary>
		/// <returns>
		/// The <see cref="T:System.Data.Common.DbConnection"/> object associated with the transaction.
		/// </returns>
		protected override DbConnection DbConnection
		{
			get { return _conn; }
		}

		/// <summary>
		/// Specifies the <see cref="T:System.Data.IsolationLevel"/> for this transaction.
		/// </summary>
		/// <returns>
		/// The <see cref="T:System.Data.IsolationLevel"/> for this transaction.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override IsolationLevel IsolationLevel
		{
			get { return _isolationLevel; }
		}

		/// <summary>
		/// Commits the database transaction.
		/// </summary>
		/// <filterpriority>1</filterpriority>
		public override void Commit()
		{
		}

		/// <summary>
		/// Rolls back a transaction from a pending state.
		/// </summary>
		/// <filterpriority>1</filterpriority>
		public override void Rollback()
		{
		}
	}

	public class FakeDbCommand : DbCommand
	{
		private DbParameterCollection _dbParameterCollection;

		/// <summary>
		/// Gets or sets the text command to run against the data source.
		/// </summary>
		/// <returns>
		/// The text command to execute. The default value is an empty string ("").
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override string CommandText { get; set; }

		/// <summary>
		/// Gets or sets the wait time before terminating the attempt to execute a command and generating an error.
		/// </summary>
		/// <returns>
		/// The time in seconds to wait for the command to execute.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int CommandTimeout { get; set; }

		/// <summary>
		/// Indicates or specifies how the <see cref="P:System.Data.Common.DbCommand.CommandText"/> property is interpreted.
		/// </summary>
		/// <returns>
		/// One of the <see cref="T:System.Data.CommandType"/> values. The default is Text.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override CommandType CommandType { get; set; }

		/// <summary>
		/// Gets or sets how command results are applied to the <see cref="T:System.Data.DataRow"/> when used by the Update method of a <see cref="T:System.Data.Common.DbDataAdapter"/>.
		/// </summary>
		/// <returns>
		/// One of the <see cref="T:System.Data.UpdateRowSource"/> values. The default is Both unless the command is automatically generated. Then the default is None.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override UpdateRowSource UpdatedRowSource { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="T:System.Data.Common.DbConnection"/> used by this <see cref="T:System.Data.Common.DbCommand"/>.
		/// </summary>
		/// <returns>
		/// The connection to the data source.
		/// </returns>
		protected override DbConnection DbConnection { get; set; }

		/// <summary>
		/// Gets the collection of <see cref="T:System.Data.Common.DbParameter"/> objects.
		/// </summary>
		/// <returns>
		/// The parameters of the SQL statement or stored procedure.
		/// </returns>
		protected override DbParameterCollection DbParameterCollection
		{
			get { return _dbParameterCollection; }
		}

		/// <summary>
		/// Gets or sets the <see cref="P:System.Data.Common.DbCommand.DbTransaction"/> within which this <see cref="T:System.Data.Common.DbCommand"/> object executes.
		/// </summary>
		/// <returns>
		/// The transaction within which a Command object of a .NET Framework data provider executes. The default value is a null reference (Nothing in Visual Basic).
		/// </returns>
		protected override DbTransaction DbTransaction { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the command object should be visible in a customized interface control.
		/// </summary>
		/// <returns>
		/// true, if the command object should be visible in a control; otherwise false. The default is true.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override bool DesignTimeVisible { get; set; }

		/// <summary>
		/// Creates a prepared (or compiled) version of the command on the data source.
		/// </summary>
		/// <filterpriority>1</filterpriority>
		public override void Prepare()
		{
		}

		/// <summary>
		/// Attempts to cancels the execution of a <see cref="T:System.Data.Common.DbCommand"/>.
		/// </summary>
		/// <filterpriority>1</filterpriority>
		public override void Cancel()
		{
		}

		/// <summary>
		/// Creates a new instance of a <see cref="T:System.Data.Common.DbParameter"/> object.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Data.Common.DbParameter"/> object.
		/// </returns>
		protected override DbParameter CreateDbParameter()
		{
			return new FakeDbParameter();
		}

		/// <summary>
		/// Executes the command text against the connection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Data.Common.DbDataReader"/>.
		/// </returns>
		/// <param name="behavior">An instance of <see cref="T:System.Data.CommandBehavior"/>.</param>
		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			return new FakeDbDataReader();
		}

		/// <summary>
		/// Executes a SQL statement against a connection object.
		/// </summary>
		/// <returns>
		/// The number of rows affected.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override int ExecuteNonQuery()
		{
			return 0;
		}

		/// <summary>
		/// Executes the query and returns the first column of the first row in the result set returned by the query. All other columns and rows are ignored.
		/// </summary>
		/// <returns>
		/// The first column of the first row in the result set.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override object ExecuteScalar()
		{
			return null;
		}
	}

	public class FakeDbParameter : DbParameter
	{
		/// <summary>
		/// Gets or sets the <see cref="T:System.Data.DbType"/> of the parameter.
		/// </summary>
		/// <returns>
		/// One of the <see cref="T:System.Data.DbType"/> values. The default is <see cref="F:System.Data.DbType.String"/>.
		/// </returns>
		/// <exception cref="T:System.ArgumentException">The property is not set to a valid <see cref="T:System.Data.DbType"/>.</exception><filterpriority>1</filterpriority>
		public override DbType DbType { get; set; }

		/// <summary>
		/// Gets or sets a value that indicates whether the parameter is input-only, output-only, bidirectional, or a stored procedure return value parameter.
		/// </summary>
		/// <returns>
		/// One of the <see cref="T:System.Data.ParameterDirection"/> values. The default is Input.
		/// </returns>
		/// <exception cref="T:System.ArgumentException">The property is not set to one of the valid <see cref="T:System.Data.ParameterDirection"/> values.</exception><filterpriority>1</filterpriority>
		public override ParameterDirection Direction { get; set; }

		/// <summary>
		/// Gets or sets a value that indicates whether the parameter accepts null values.
		/// </summary>
		/// <returns>
		/// true if null values are accepted; otherwise false. The default is false.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override bool IsNullable { get; set; }

		/// <summary>
		/// Gets or sets the name of the <see cref="T:System.Data.Common.DbParameter"/>.
		/// </summary>
		/// <returns>
		/// The name of the <see cref="T:System.Data.Common.DbParameter"/>. The default is an empty string ("").
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override string ParameterName { get; set; }

		/// <summary>
		/// Gets or sets the name of the source column mapped to the <see cref="T:System.Data.DataSet"/> and used for loading or returning the <see cref="P:System.Data.Common.DbParameter.Value"/>.
		/// </summary>
		/// <returns>
		/// The name of the source column mapped to the <see cref="T:System.Data.DataSet"/>. The default is an empty string.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override string SourceColumn { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="T:System.Data.DataRowVersion"/> to use when you load <see cref="P:System.Data.Common.DbParameter.Value"/>.
		/// </summary>
		/// <returns>
		/// One of the <see cref="T:System.Data.DataRowVersion"/> values. The default is Current.
		/// </returns>
		/// <exception cref="T:System.ArgumentException">The property is not set to one of the <see cref="T:System.Data.DataRowVersion"/> values.</exception><filterpriority>1</filterpriority>
		public override DataRowVersion SourceVersion { get; set; }

		/// <summary>
		/// Gets or sets the value of the parameter.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Object"/> that is the value of the parameter. The default value is null.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override object Value { get; set; }

		/// <summary>
		/// Sets or gets a value which indicates whether the source column is nullable. This allows <see cref="T:System.Data.Common.DbCommandBuilder"/> to correctly generate Update statements for nullable columns.
		/// </summary>
		/// <returns>
		/// true if the source column is nullable; false if it is not.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override bool SourceColumnNullMapping { get; set; }

		/// <summary>
		/// Gets or sets the maximum size, in bytes, of the data within the column.
		/// </summary>
		/// <returns>
		/// The maximum size, in bytes, of the data within the column. The default value is inferred from the parameter value.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override int Size { get; set; }

		/// <summary>
		/// Resets the DbType property to its original settings.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public override void ResetDbType()
		{
		}
	}

	public class FakeDbDataReader : DbDataReader
	{
		private int _depth;

		private int _fieldCount;

		private bool _hasRows;

		private bool _isClosed;

		private int _recordsAffected;

		/// <summary>
		/// Gets a value indicating the depth of nesting for the current row.
		/// </summary>
		/// <returns>
		/// The depth of nesting for the current row.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override int Depth
		{
			get { return _depth; }
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Data.Common.DbDataReader"/> is closed.
		/// </summary>
		/// <returns>
		/// true if the <see cref="T:System.Data.Common.DbDataReader"/> is closed; otherwise false.
		/// </returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Data.SqlClient.SqlDataReader"/> is closed. </exception><filterpriority>1</filterpriority>
		public override bool IsClosed
		{
			get { return _isClosed; }
		}

		/// <summary>
		/// Gets the number of rows changed, inserted, or deleted by execution of the SQL statement. 
		/// </summary>
		/// <returns>
		/// The number of rows changed, inserted, or deleted. -1 for SELECT statements; 0 if no rows were affected or the statement failed.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override int RecordsAffected
		{
			get { return _recordsAffected; }
		}

		/// <summary>
		/// Gets the number of columns in the current row.
		/// </summary>
		/// <returns>
		/// The number of columns in the current row.
		/// </returns>
		/// <exception cref="T:System.NotSupportedException">There is no current connection to an instance of SQL Server. </exception><filterpriority>1</filterpriority>
		public override int FieldCount
		{
			get { return _fieldCount; }
		}

		/// <summary>
		/// Gets the value of the specified column as an instance of <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// The value of the specified column.
		/// </returns>
		/// <param name="ordinal">The zero-based column ordinal.</param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>1</filterpriority>
		public override object this[int ordinal]
		{
			get { return null; }
		}

		/// <summary>
		/// Gets the value of the specified column as an instance of <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// The value of the specified column.
		/// </returns>
		/// <param name="name">The name of the column.</param><exception cref="T:System.IndexOutOfRangeException">No column with the specified name was found. </exception><filterpriority>1</filterpriority>
		public override object this[string name]
		{
			get { return null; }
		}

		/// <summary>
		/// Gets a value that indicates whether this <see cref="T:System.Data.Common.DbDataReader"/> contains one or more rows.
		/// </summary>
		/// <returns>
		/// true if the <see cref="T:System.Data.Common.DbDataReader"/> contains one or more rows; otherwise false.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override bool HasRows
		{
			get { return _hasRows; }
		}

		/// <summary>
		/// Closes the <see cref="T:System.Data.Common.DbDataReader"/> object.
		/// </summary>
		/// <filterpriority>1</filterpriority>
		public override void Close()
		{
		}

		/// <summary>
		/// Returns a <see cref="T:System.Data.DataTable"/> that describes the column metadata of the <see cref="T:System.Data.Common.DbDataReader"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Data.DataTable"/> that describes the column metadata.
		/// </returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Data.SqlClient.SqlDataReader"/> is closed. </exception><filterpriority>1</filterpriority>
		public override DataTable GetSchemaTable()
		{
			return new DataTable();
		}

		/// <summary>
		/// Advances the reader to the next result when reading the results of a batch of statements.
		/// </summary>
		/// <returns>
		/// true if there are more result sets; otherwise false.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override bool NextResult()
		{
			return false;
		}

		/// <summary>
		/// Advances the reader to the next record in a result set.
		/// </summary>
		/// <returns>
		/// true if there are more rows; otherwise false.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override bool Read()
		{
			return false;
		}

		/// <summary>
		/// Gets the value of the specified column as a Boolean.
		/// </summary>
		/// <returns>
		/// The value of the specified column.
		/// </returns>
		/// <param name="ordinal">The zero-based column ordinal.</param><exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception><filterpriority>1</filterpriority>
		public override bool GetBoolean(int ordinal)
		{
			return false;
		}

		/// <summary>
		/// Gets the value of the specified column as a byte.
		/// </summary>
		/// <returns>
		/// The value of the specified column.
		/// </returns>
		/// <param name="ordinal">The zero-based column ordinal.</param><exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception><filterpriority>1</filterpriority>
		public override byte GetByte(int ordinal)
		{
			return 0;
		}

		/// <summary>
		/// Reads a stream of bytes from the specified column, starting at location indicated by <paramref name="dataOffset"/>, into the buffer, starting at the location indicated by <paramref name="bufferOffset"/>.
		/// </summary>
		/// <returns>
		/// The actual number of bytes read.
		/// </returns>
		/// <param name="ordinal">The zero-based column ordinal.</param><param name="dataOffset">The index within the row from which to begin the read operation.</param><param name="buffer">The buffer into which to copy the data.</param><param name="bufferOffset">The index with the buffer to which the data will be copied.</param><param name="length">The maximum number of characters to read.</param><exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception><filterpriority>1</filterpriority>
		public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
		{
			return 0;
		}

		/// <summary>
		/// Gets the value of the specified column as a single character.
		/// </summary>
		/// <returns>
		/// The value of the specified column.
		/// </returns>
		/// <param name="ordinal">The zero-based column ordinal.</param><exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception><filterpriority>1</filterpriority>
		public override char GetChar(int ordinal)
		{
			return '\0';
		}

		/// <summary>
		/// Reads a stream of characters from the specified column, starting at location indicated by <paramref name="dataIndex"/>, into the buffer, starting at the location indicated by <paramref name="bufferIndex"/>.
		/// </summary>
		/// <returns>
		/// The actual number of characters read.
		/// </returns>
		/// <param name="ordinal">The zero-based column ordinal.</param><param name="dataOffset">The index within the row from which to begin the read operation.</param><param name="buffer">The buffer into which to copy the data.</param><param name="bufferOffset">The index with the buffer to which the data will be copied.</param><param name="length">The maximum number of characters to read.</param><filterpriority>1</filterpriority>
		public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
		{
			return 0;
		}

		/// <summary>
		/// Gets the value of the specified column as a globally-unique identifier (GUID).
		/// </summary>
		/// <returns>
		/// The value of the specified column.
		/// </returns>
		/// <param name="ordinal">The zero-based column ordinal.</param><exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception><filterpriority>1</filterpriority>
		public override Guid GetGuid(int ordinal)
		{
			return Guid.Empty;
		}

		/// <summary>
		/// Gets the value of the specified column as a 16-bit signed integer.
		/// </summary>
		/// <returns>
		/// The value of the specified column.
		/// </returns>
		/// <param name="ordinal">The zero-based column ordinal.</param><exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception><filterpriority>2</filterpriority>
		public override short GetInt16(int ordinal)
		{
			return 0;
		}

		/// <summary>
		/// Gets the value of the specified column as a 32-bit signed integer.
		/// </summary>
		/// <returns>
		/// The value of the specified column.
		/// </returns>
		/// <param name="ordinal">The zero-based column ordinal.</param><exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception><filterpriority>1</filterpriority>
		public override int GetInt32(int ordinal)
		{
			return 0;
		}

		/// <summary>
		/// Gets the value of the specified column as a 64-bit signed integer.
		/// </summary>
		/// <returns>
		/// The value of the specified column.
		/// </returns>
		/// <param name="ordinal">The zero-based column ordinal.</param><exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception><filterpriority>2</filterpriority>
		public override long GetInt64(int ordinal)
		{
			return 0;
		}

		/// <summary>
		/// Gets the value of the specified column as a <see cref="T:System.DateTime"/> object.
		/// </summary>
		/// <returns>
		/// The value of the specified column.
		/// </returns>
		/// <param name="ordinal">The zero-based column ordinal.</param><exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception><filterpriority>1</filterpriority>
		public override DateTime GetDateTime(int ordinal)
		{
			return DateTime.MinValue;
		}

		/// <summary>
		/// Gets the value of the specified column as an instance of <see cref="T:System.String"/>.
		/// </summary>
		/// <returns>
		/// The value of the specified column.
		/// </returns>
		/// <param name="ordinal">The zero-based column ordinal.</param><exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception><filterpriority>1</filterpriority>
		public override string GetString(int ordinal)
		{
			return string.Empty;
		}

		/// <summary>
		/// Gets the value of the specified column as an instance of <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// The value of the specified column.
		/// </returns>
		/// <param name="ordinal">The zero-based column ordinal.</param><filterpriority>1</filterpriority>
		public override object GetValue(int ordinal)
		{
			return null;
		}

		/// <summary>
		/// Populates an array of objects with the column values of the current row.
		/// </summary>
		/// <returns>
		/// The number of instances of <see cref="T:System.Object"/> in the array.
		/// </returns>
		/// <param name="values">An array of <see cref="T:System.Object"/> into which to copy the attribute columns.</param><filterpriority>1</filterpriority>
		public override int GetValues(object[] values)
		{
			return 0;
		}

		/// <summary>
		/// Gets a value that indicates whether the column contains nonexistent or missing values.
		/// </summary>
		/// <returns>
		/// true if the specified column is equivalent to <see cref="T:System.DBNull"/>; otherwise false.
		/// </returns>
		/// <param name="ordinal">The zero-based column ordinal.</param><filterpriority>1</filterpriority>
		public override bool IsDBNull(int ordinal)
		{
			return false;
		}

		/// <summary>
		/// Gets the value of the specified column as a <see cref="T:System.Decimal"/> object.
		/// </summary>
		/// <returns>
		/// The value of the specified column.
		/// </returns>
		/// <param name="ordinal">The zero-based column ordinal.</param><exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception><filterpriority>1</filterpriority>
		public override decimal GetDecimal(int ordinal)
		{
			return 0;
		}

		/// <summary>
		/// Gets the value of the specified column as a double-precision floating point number.
		/// </summary>
		/// <returns>
		/// The value of the specified column.
		/// </returns>
		/// <param name="ordinal">The zero-based column ordinal.</param><exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception><filterpriority>1</filterpriority>
		public override double GetDouble(int ordinal)
		{
			return 0;
		}

		/// <summary>
		/// Gets the value of the specified column as a single-precision floating point number.
		/// </summary>
		/// <returns>
		/// The value of the specified column.
		/// </returns>
		/// <param name="ordinal">The zero-based column ordinal.</param><exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception><filterpriority>2</filterpriority>
		public override float GetFloat(int ordinal)
		{
			return 0;
		}

		/// <summary>
		/// Gets the name of the column, given the zero-based column ordinal.
		/// </summary>
		/// <returns>
		/// The name of the specified column.
		/// </returns>
		/// <param name="ordinal">The zero-based column ordinal.</param><filterpriority>1</filterpriority>
		public override string GetName(int ordinal)
		{
			return string.Empty;
		}

		/// <summary>
		/// Gets the column ordinal given the name of the column.
		/// </summary>
		/// <returns>
		/// The zero-based column ordinal.
		/// </returns>
		/// <param name="name">The name of the column.</param><exception cref="T:System.IndexOutOfRangeException">The name specified is not a valid column name.</exception><filterpriority>1</filterpriority>
		public override int GetOrdinal(string name)
		{
			return 0;
		}

		/// <summary>
		/// Gets name of the data type of the specified column.
		/// </summary>
		/// <returns>
		/// A string representing the name of the data type.
		/// </returns>
		/// <param name="ordinal">The zero-based column ordinal.</param><exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception><filterpriority>1</filterpriority>
		public override string GetDataTypeName(int ordinal)
		{
			return string.Empty;
		}

		/// <summary>
		/// Gets the data type of the specified column.
		/// </summary>
		/// <returns>
		/// The data type of the specified column.
		/// </returns>
		/// <param name="ordinal">The zero-based column ordinal.</param><exception cref="T:System.InvalidCastException">The specified cast is not valid. </exception><filterpriority>1</filterpriority>
		public override Type GetFieldType(int ordinal)
		{
			return typeof(DBNull);
		}

		/// <summary>
		/// Returns an <see cref="T:System.Collections.IEnumerator"/> that can be used to iterate through the rows in the data reader.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> that can be used to iterate through the rows in the data reader.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override IEnumerator GetEnumerator()
		{
			return new FakeDbDataReader[0].GetEnumerator();
		}
	}

	public class FakeDbProviderFactory : DbProviderFactory
	{
		public static FakeDbProviderFactory Instance = new FakeDbProviderFactory();

		public override DbCommand CreateCommand()
		{
			return new FakeDbCommand();
		}

		public override DbConnection CreateConnection()
		{
			return new FakeDbConnection();
		}

		public override DbParameter CreateParameter()
		{
			return new FakeDbParameter();
		}
	}
}