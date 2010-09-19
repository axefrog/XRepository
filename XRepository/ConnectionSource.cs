using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;

namespace XRepository
{
	/// <summary>
	/// Specifies a unique connection source to provide a context for opening new connections.
	/// </summary>
	public class ConnectionSource
	{
		private int _hash;

		/// <summary>
		/// The connection string to use when opening a new connection
		/// </summary>
		public string ConnectionString { get; private set; }

		/// <summary>
		/// The connection provider that will manage a connection using the specified <see cref="ConnectionString" />.
		/// </summary>
		public DbProviderFactory Provider { get; private set; }

		/// <summary>
		/// Constructs a new <see cref="ConnectionSource"/> object using the specified connection string and
		/// <see cref="DbProviderFactory"/> instance.
		/// </summary>
		/// <param name="connectionString">The connection string to use</param>
		/// <param name="provider">The DbProviderFactory to use</param>
		public ConnectionSource(string connectionString, DbProviderFactory provider)
		{
			if(connectionString == null)
				throw new ArgumentNullException("connectionString");
			if(provider == null)
				throw new ArgumentNullException("provider");
			ConnectionString = connectionString;
			Provider = provider;
		}

		/// <summary>
		/// Constructs a new <see cref="ConnectionSource"/> object using the specified ConnectionString entry in the
		/// application configuration file.
		/// </summary>
		/// <param name="connectionStringConfigName">The name of the connection string to use</param>
		public ConnectionSource(string connectionStringConfigName)
		{
			if(connectionStringConfigName == null)
				throw new ArgumentNullException("connectionStringConfigName");
			RegisterFromConnectionStringSettings(connectionStringConfigName);
		}

		/// <summary>
		/// Constructs a <see cref="ConnectionSource"/> object using values from the application configuration file. At
		/// least one ConnectionString entry must be present. If only one is present, it will be used. If more than one
		/// ConnectionString entry is present, add an entry to appSettings specifying the name of the connection string
		/// to use. e.g. &lt;add key=&quot;ConnectionString.Default&quot; value=&quot;MyDatabase&quot; /&gt;
		/// </summary>
		public ConnectionSource()
		{
			if(ConfigurationManager.ConnectionStrings.Count == 0)
				throw new Exception("There are no connection strings specified in the application configuration file");
			if(ConfigurationManager.ConnectionStrings.Count > 1)
			{
				var defaultName = ConfigurationManager.AppSettings["ConnectionString.Default"];
				if(defaultName == null)
					throw new Exception("You have multiple connection strings in your application configuration file. You can specify which one is the default by adding its name to your appSettings configuration section, under the key \"ConnectionString.Default\". Alternatively, specify its name using one of the other constructor overloads.");
				RegisterFromConnectionStringSettings(defaultName);
			}
			else
				RegisterFromConnectionStringSettings(ConfigurationManager.ConnectionStrings[0]);
		}

		private void RegisterFromConnectionStringSettings(string connectionStringConfigName)
		{
			var cs = ConfigurationManager.ConnectionStrings[connectionStringConfigName];
			if(cs == null)
				throw new Exception("There isn't a ConnectionString entry in the application configuration file with the name " + connectionStringConfigName);
			RegisterFromConnectionStringSettings(cs);
		}

		private void RegisterFromConnectionStringSettings(ConnectionStringSettings cs)
		{
			ConnectionString = cs.ConnectionString;
			Provider = DbProviderFactories.GetFactory(cs.ProviderName);
			_hash = string.Concat(ConnectionString, Provider.GetType().AssemblyQualifiedName).GetHashCode();
		}

		public override int GetHashCode()
		{
			return _hash;
		}

		public long GetHashForThread()
		{
			var bytes = new byte[8];
			var buf1 = BitConverter.GetBytes(Thread.CurrentThread.ManagedThreadId);
			var buf2 = BitConverter.GetBytes(_hash);
			Buffer.BlockCopy(buf1, 0, bytes, 0, 4);
			Buffer.BlockCopy(buf2, 0, bytes, 4, 4);
			return BitConverter.ToInt64(bytes, 0);
		}

		public DbConnection CreateConnection()
		{
			var conn = Provider.CreateConnection();
			conn.ConnectionString = ConnectionString;
			return conn;
		}
	}
}
