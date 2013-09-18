using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Collections.ObjectModel;

namespace RadicalGeek.Common.Data
{
    public abstract class DataRepository<TConnection> : IDataRepository where TConnection : DbConnection, new()
    {
        private class DatabaseResult<T>
        {
            public T FunctionReturnValue { get; set; }
            public object SqlReturnValue { get; set; }
        }

        private int? commandTimeout;

        [Obsolete("DO NOT USE this in new code; If your command is taking over 30s, your design is wrong. This property IS allowed for Legacy systems where it is not possible to immediately resolve the issue by other means.")]
        public int CommandTimeout { set { commandTimeout = value; } }

        readonly Dictionary<DbTransaction, DbConnection> transactionDictionary = new Dictionary<DbTransaction, DbConnection>();

        /// <summary>
        /// Stores the plaintext Connection String to be used by the generic database engine
        /// </summary>
        protected abstract string ConnectionString { get; set; }

        /// <summary>
        /// Executes a stored procedure and returns a DbDataReader. Make sure to use this in a Using block to ensure resources are properly managed.
        /// </summary>
        /// <param name="storedProcedure">The name of the stored procedure</param>
        /// <param name="parameters">The parameters to be passed to the stored procedure</param>
        /// <returns></returns>
        public DbDataReader ExecuteReader(string storedProcedure, params SimpleDbParam[] parameters)
        {
            return ExecuteReader(storedProcedure, CommandType.StoredProcedure, parameters);
        }

        public DbDataReader ExecuteReader(string commandText, CommandType commandType, params SimpleDbParam[] parameters)
        {
            return ExecuteReader(commandText, commandType, null, parameters);
        }

        public DbTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            DbConnection connection = new TConnection { ConnectionString = ConnectionString };
            DbTransaction transaction = connection.BeginTransaction(isolationLevel);
            transactionDictionary.Add(transaction, connection);
            return transaction;
        }

        readonly Action<DbTransaction> commit = t => t.Commit();
        readonly Action<DbTransaction> rollback = t => t.Rollback();

        public void CommitTransaction(DbTransaction transaction)
        {
            DisposeTransaction(transaction, commit);
        }

        public void AbortTransaction(DbTransaction transaction)
        {
            DisposeTransaction(transaction, rollback);
        }

        private void DisposeTransaction(DbTransaction transaction, Action<DbTransaction> preDispose)
        {
            preDispose(transaction);
            DbConnection connection = transactionDictionary[transaction];
            transactionDictionary.Remove(transaction);
            transaction.Dispose();
            connection.Close();
            connection.Dispose();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public DbDataReader ExecuteReader(string commandText, CommandType commandType, DbTransaction transaction = null, params SimpleDbParam[] parameters)
        {
            DbConnection connection = transaction != null ? transactionDictionary[transaction] : new TConnection { ConnectionString = ConnectionString };

            if (connection.State != ConnectionState.Open)
                connection.Open();

            DbCommand command = connection.CreateCommand();
            if (commandTimeout.HasValue) command.CommandTimeout = commandTimeout.Value;
            PrepareCommand(commandText, commandType, parameters, command);

            DisposingDbDataReader disposingDbDataReader = new DisposingDbDataReader(command);

            RewireParameterResults(command, parameters);

            return disposingDbDataReader;
        }

        private static void AddParametersToCommand(IEnumerable<SimpleDbParam> parameters, DbCommand command)
        {
            foreach (SimpleDbParam simpleDbParam in parameters)
            {
                DbParameter dbParameter = command.CreateParameter();
                dbParameter.ParameterName = simpleDbParam.ParameterName;
                dbParameter.Value = simpleDbParam.Value;
                dbParameter.Direction = simpleDbParam.Direction;
                if (simpleDbParam.Size.HasValue)
                    dbParameter.Size = simpleDbParam.Size.Value;
                if (simpleDbParam.DbType.HasValue)
                    dbParameter.DbType = simpleDbParam.DbType.Value;
                command.Parameters.Add(dbParameter);
            }
        }

        public object ExecuteScalar(string storedProcedure, params SimpleDbParam[] parameters)
        {
            return ExecuteScalar(storedProcedure, CommandType.StoredProcedure, parameters);
        }

        public object ExecuteScalar(string commandText, CommandType commandType, params SimpleDbParam[] parameters)
        {
            return ExecuteFunc<object>(commandText, commandType, parameters, c => c.ExecuteScalar()).FunctionReturnValue;
        }

        public int ExecuteNonQuery(string storedProcedure, params SimpleDbParam[] parameters)
        {
            return ExecuteNonQuery(storedProcedure, CommandType.StoredProcedure, parameters);
        }

        public int ExecuteNonQuery(string commandText, CommandType commandType, params SimpleDbParam[] parameters)
        {
            return ExecuteFunc(commandText, commandType, parameters, c => c.ExecuteNonQuery()).FunctionReturnValue;
        }

        public DataTable ExecuteToDataTable(string commandText, CommandType commandType,
                                            params SimpleDbParam[] parameters)
        {
            return ExecuteFunc(commandText, commandType, parameters, c => c.ExecuteToDataTable()).FunctionReturnValue;
        }

        public ReadOnlyCollection<T> ExecuteToReadOnlyCollection<T>(string commandText, CommandType commandType,
                                                                    params SimpleDbParam[] parameters)
                where T : class, new()
        {
            return ExecuteFunc(commandText, commandType, parameters, c => c.ExecuteToReadOnlyCollection<T>()).FunctionReturnValue;
        }

        public List<T> ExecuteToList<T>(string commandText, CommandType commandType, params SimpleDbParam[] parameters)
                where T : class, new()
        {
            return ExecuteFunc(commandText, commandType, parameters, c => c.ExecuteToList<T>()).FunctionReturnValue;
        }

        private DatabaseResult<T> ExecuteFunc<T>(string commandText, CommandType commandType, IEnumerable<SimpleDbParam> parameters,
                                 Func<DbCommand, T> func)
        {
            using (DbConnection connection = new TConnection { ConnectionString = ConnectionString })
            {
                DatabaseResult<T> databaseResult = new DatabaseResult<T>();
                connection.Open();
                using (DbCommand command = connection.CreateCommand())
                {
                    PrepareCommand(commandText, commandType, parameters, command);
                    databaseResult.FunctionReturnValue = func(command);
                    for (int i = command.Parameters.Count - 1; i >= 0; i--)
                        if (command.Parameters[i].ParameterName.ToUpper() == "@RETURN_VALUE")
                        {
                            databaseResult.SqlReturnValue = command.Parameters[i].Value;
                            break;
                        }
                    RewireParameterResults(command, parameters);
                }
                connection.Close();
                return databaseResult;
            }
        }

        private static void RewireParameterResults(DbCommand command, IEnumerable<SimpleDbParam> parameters)
        {
            foreach (SimpleDbParam simpleDbParam in parameters)
            {
                DbParameter dbParameter = command.Parameters[simpleDbParam.ParameterName];
                if (dbParameter != null)
                    simpleDbParam.Value = dbParameter.Value;
            }
        }

        private static void PrepareCommand(string commandText, CommandType commandType,
                                           IEnumerable<SimpleDbParam> parameters,
                                           DbCommand command)
        {
            command.CommandType = commandType;
            command.CommandText = commandText;
            AddParametersToCommand(parameters, command);
        }
    }
}