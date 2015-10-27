using KS.CRMWorkflow.Component.Cache.Interface;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading.Tasks;

namespace KS.CRMWorkflow.Data.DataAccess.Repository.Implementation
{
    /// <summary>
    /// A class that represents the common functionality required
    /// for Dapper based repositories.
    /// 
    /// Dapper documentation at https://github.com/StackExchange/dapper-dot-net
    /// </summary>
    public abstract class BaseDapperRepository<T>
    {
        /// <summary>
        /// The Cache Provider for this repository.
        /// </summary>
        protected ICacheProvider CacheProvider { get; set; }

        /// <summary>
        /// The DB Connection String for this repository.
        /// </summary>
        protected string DbConnectionString { get; set; }

        /// <summary>
        /// Creates a new instance of a base dapper repository.
        /// </summary>
        public BaseDapperRepository(ICacheProvider cacheProvider)
        {
            DbConnectionString = ConfigurationManager
                .ConnectionStrings["CRMWorkflowDB"]
                .ToString();

            CacheProvider = cacheProvider;
        }

        #pragma warning disable 1998

        /// <summary>
        /// Once the bulk records have been inserted into the staging area,
        /// this method will be called to move the imported records
        /// to the final table.
        /// </summary>
        protected virtual async Task FinalizeBulkImport()
        {
        }

        #pragma warning restore 1998

        /// <summary>
        /// The name of the staging table where the bulk records will initially
        /// be inserted.
        /// </summary>
        protected string StagingTableName { get; set; }

        /// <summary>
        /// A mapping from field names to table column names for a bulk insert.
        /// </summary>
        protected Dictionary<string, string> BulkColumnMapping { get; set; }

        /// <summary>
        /// Converts a collection of items to a data table.
        /// This could be overridden in the extending classes
        /// for superior performance, or the class can just
        /// accept the default version based on Reflection.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        protected virtual DataTable ItemToDataTable(IEnumerable<T> items)
        {
            DataTable dt = new DataTable();

            PropertyInfo[] props = typeof(T)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                dt.Columns.Add(prop.Name, prop.PropertyType);
            }

            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                dt.Rows.Add(values);
            }

            return dt;
        }

        /// <summary>
        /// Imports a set of bulk records into the data store.
        /// </summary>
        public async Task BulkImport(IEnumerable<T> items)
        {
            if (string.IsNullOrEmpty(StagingTableName))
            {
                return;
            }
                
            using (SqlConnection connection =
                  SqlConnectionManager.GetConnection(DbConnectionString))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connection))
                {
                    sqlBulkCopy.DestinationTableName = StagingTableName;
                    sqlBulkCopy.BatchSize = 500;

                    foreach(KeyValuePair<string, string> kvp in BulkColumnMapping)
                    {
                        sqlBulkCopy.ColumnMappings.Add(kvp.Key, kvp.Value);
                    }

                    DataTable dt = ItemToDataTable(items);
                    await sqlBulkCopy.WriteToServerAsync(dt);
                    connection.Close();
                }
            }        

            await FinalizeBulkImport();
        }
    }
}
