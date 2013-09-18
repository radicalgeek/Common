using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using RadicalGeek.Common.Text;
using System.Collections.Specialized;
using RadicalGeek.Common.Cryptography;

namespace RadicalGeek.Common.Data
{
    public sealed class SqlDataRepository : DataRepository<SqlConnection>
    {
        private readonly string[] userlessKeys = new[] { "Integrated Security", "Trusted_Connection" };

        protected override string ConnectionString { get; set; }

        public SqlDataRepository(string connectionName)
        {
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionName];
            if (!connectionStringSettings.ConnectionString.Contains(';'))
                throw new InvalidOperationException(string.Format("Connection string {0} appears to be improperly formatted.", connectionName));

            string[] elements = connectionStringSettings.ConnectionString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
            string badElement = elements.FirstOrDefault(e => !e.Contains('='));
            if (badElement != null)
                throw new InvalidOperationException(string.Format("Connection string Element \"{0}\" is not a Key Value Pair, e.g. \"Key=Value\"", badElement));

            Dictionary<string, string> splitConnectionString = elements.Select(s => s.Split('=')).ToDictionary(s => s[0].Trim().ToUpper(), s => s[1].Trim());

            string connectionUserIdKey = string.Format("{0}_UserId", connectionName);
            string connectionPasswordKey = string.Format("{0}_Password", connectionName);

            if (splitConnectionString.ContainsKey("USER ID"))
                throw new InvalidOperationException(string.Format("Sql Connection Strings must not contain a User Id, even if blank. Instead, add a key with the name {0} to the AppSettings section of the config file.", connectionUserIdKey));
            if (splitConnectionString.ContainsKey("PASSWORD"))
                throw new InvalidOperationException(string.Format("Sql Connection Strings must not contain a Password, even if blank. Instead, add a key with the name {0} to the AppSettings section of the config file.", connectionPasswordKey));

            if (!splitConnectionString.Any(keyValuePair => userlessKeys.Contains(keyValuePair.Key)))
            {
                NameValueCollection settings = ConfigurationManager.AppSettings;

                if (!settings.AllKeys.Contains(connectionUserIdKey))
                    throw new KeyNotFoundException(string.Format("For a Sql Connection String without Integrated Security or Trusted_Connection, you must supply a Username in the {0} key in the AppSettings section of the config file.", connectionUserIdKey));
                if (!settings.AllKeys.Contains(connectionPasswordKey))
                    throw new KeyNotFoundException(string.Format("For a Sql Connection String without Integrated Security or Trusted_Connection, you must supply a Password in the {0} key in the AppSettings section of the config file.", connectionPasswordKey));

                string userName = settings[connectionUserIdKey].Decrypt(EncryptionKeySet.General);
                string password = settings[connectionPasswordKey].Decrypt(EncryptionKeySet.General);
                if (!string.IsNullOrWhiteSpace(userName))
                    splitConnectionString.Add("User Id", userName);
                if (!string.IsNullOrWhiteSpace(password))
                    splitConnectionString.Add("Password", password);
            }
            ConnectionString = string.Join(";", splitConnectionString.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));
        }
    }
}