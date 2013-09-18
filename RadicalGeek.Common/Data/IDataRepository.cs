using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;

namespace RadicalGeek.Common.Data
{
    public interface IDataRepository
    {
        /// <summary>
        /// Executes a stored procedure and returns a DbDataReader. Make sure to use this in a Using block to ensure resources are properly managed.
        /// </summary>
        /// <param name="storedProcedure">The name of the stored procedure</param>
        /// <param name="parameters">The parameters to be passed to the stored procedure</param>
        /// <returns></returns>
        DbDataReader ExecuteReader(string storedProcedure, params SimpleDbParam[] parameters);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security",
                "CA2100:Review SQL queries for security vulnerabilities")]
        DbDataReader ExecuteReader(string commandText, CommandType commandType, params SimpleDbParam[] parameters);

        object ExecuteScalar(string storedProcedure, params SimpleDbParam[] parameters);
        object ExecuteScalar(string commandText, CommandType commandType, params SimpleDbParam[] parameters);
        int ExecuteNonQuery(string storedProcedure, params SimpleDbParam[] parameters);
        int ExecuteNonQuery(string commandText, CommandType commandType, params SimpleDbParam[] parameters);
        DataTable ExecuteToDataTable(string commandText, CommandType commandType, params SimpleDbParam[] parameters);

        ReadOnlyCollection<T> ExecuteToReadOnlyCollection<T>(string commandText, CommandType commandType,
                                                             params SimpleDbParam[] parameters) where T : class, new();

        List<T> ExecuteToList<T>(string commandText, CommandType commandType, params SimpleDbParam[] parameters)
                where T : class, new();
    }
}