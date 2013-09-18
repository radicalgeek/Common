using System.Configuration;
using System.Data.Odbc;

namespace RadicalGeek.Common.Data
{
    public sealed class OdbcDataRepository : DataRepository<OdbcConnection>
    {
        public OdbcDataRepository(string connectionName)
        {
            ConnectionString = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
        }

        /// <summary>
        /// Stores the plaintext Connection String to be used by the generic database engine
        /// </summary>
        protected override string ConnectionString { get; set;}
    }
}