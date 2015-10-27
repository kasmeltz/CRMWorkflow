using System.Data.SqlClient;

namespace KS.CRMWorkflow.Data.DataAccess.Repository.Implementation
{
    /// <summary>
    /// Manages Sql Connections.
    /// </summary>
    public class SqlConnectionManager
    {
        /// <summary>
        /// Returns a new SqlConnection using the provided connection 
        /// string.
        /// </summary>
        /// <param name="dbConnectionString"></param>
        /// <returns></returns>
        public static SqlConnection GetConnection(string dbConnectionString)
        {
            SqlConnection connection =
                new SqlConnection(dbConnectionString);

            connection.Open();

            return connection;
        }
    }
}
