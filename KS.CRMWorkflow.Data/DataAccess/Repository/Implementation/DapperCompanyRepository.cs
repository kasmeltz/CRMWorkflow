using Dapper;
using KS.CRMWorkflow.Component.Cache.Interface;
using KS.CRMWorkflow.Data.DataAccess.Exceptions;
using KS.CRMWorkflow.Data.DataAccess.Repository.Interface;
using KS.CRMWorkflow.Data.POCO;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace KS.CRMWorkflow.Data.DataAccess.Repository.Implementation
{
    /// <summary>
    /// Provides access to Company data in the data store using the Dapper framework.
    /// 
    /// Dapper documentation at https://github.com/StackExchange/dapper-dot-net
    /// </summary>
    public class DapperCompanyRepository : BaseDapperRepository<Company>,
        ICompanyRepository
    {
        public DapperCompanyRepository(ICacheProvider cacheProvider)
            : base(cacheProvider)
        {
        }

        public async Task<IEnumerable<Company>> List()
        {
            IEnumerable<Company> items =
                (IEnumerable<Company>)CacheProvider.Get("CompanyList");

            if (items != null)
            {
                return items;
            }

            using (SqlConnection connection =
                    SqlConnectionManager.GetConnection(DbConnectionString))
            {
                items = await connection.QueryAsync<Company>(@"
                    SET NOCOUNT ON;

                    SELECT  
                        Id, Name
                    FROM 
                         Company
                    ORDER BY
                        Name"
                ).ConfigureAwait(false);
            }

            CacheProvider
                .Insert("Company",
                    "CompanyList", items);

            return items;
        }

        public async Task<Company> Get(int id)
        {
            Company item = (Company)CacheProvider.Get("Company" + id);
            if (item != null)
            {
                return item;
            }

            using (SqlConnection connection =
                    SqlConnectionManager.GetConnection(DbConnectionString))
            {
                IEnumerable<Company> ciudades = await connection.QueryAsync<Company>(@"
                    SET NOCOUNT ON;

                    SELECT  
                        Id, Name
                    FROM 
                         Company
                    WHERE
                        Id = @Id",
                    new
                    {
                        Id = id
                    }).ConfigureAwait(false);

                item = ciudades.FirstOrDefault();
            }

            CacheProvider
                .Insert("Company",
                    "Company" + id, item);

            return item;
        }

        public async Task Insert(Company item)
        {
            int result = -1;

            using (SqlConnection connection =
                 SqlConnectionManager.GetConnection(DbConnectionString))
            {
                IEnumerable<int> results = await connection.QueryAsync<int>(@"
                SET NOCOUNT ON;

	            DECLARE @Id	int;
	            SET @Id = NULL;

	            SELECT TOP 1
		            @Id = Id
	            FROM	
		            Company
	            WHERE	
		            Name = @Name
	            
	            IF(@Id IS NULL)
	            BEGIN
		            INSERT INTO Company
		            (Name)
		            VALUES
		            (@Name)

		            SELECT TOP 1 
			            Id
		            FROM	
			            Company
		            WHERE	
    		            Name = @Name
	            END
	            ELSE
	            BEGIN
		            SELECT -1
	            END",
                new
                {
                    Name = item.Name
                }).ConfigureAwait(false);

                result = results.FirstOrDefault();
            }

            if (result > 0)
            {
                item.Id = result;
                CacheProvider
                    .ClearContainer("Company");
            }

            if (result == -1)
            {
                throw new ItemAlreadyExistsException();
            }
        }

        public async Task Update(Company item)
        {
            int result = -1;

            using (SqlConnection connection =
                 SqlConnectionManager.GetConnection(DbConnectionString))
            {
                IEnumerable<int> results = await connection.QueryAsync<int>(@"
                SET NOCOUNT ON;

                DECLARE @ExistingId	int;
	            SET @ExistingId = NULL;

	            SELECT TOP 1
		            @ExistingId = Id
	            FROM	
		            Company
	            WHERE	
		            Name = @Name

                IF(@ExistingId IS NULL OR @ExistingId = @Id)
	            BEGIN
		            UPDATE 
                        Company
                    SET
                        Name = @Name
		            WHERE	
		                Id = @Id
                    
                    SELECT @Id
                END
                ELSE
                BEGIN
                    SELECT -1
                END",
                new
                {
                    Id = item.Id,
                    Name = item.Name
                }).ConfigureAwait(false);

                result = results.FirstOrDefault();
            }

            if (result > 0)
            {
                CacheProvider
                    .ClearContainer("Company");
            }

            if (result == -1)
            {
                throw new ItemAlreadyExistsException();
            }
        }

        public async Task Delete(int id)
        {
            using (SqlConnection connection =
             SqlConnectionManager.GetConnection(DbConnectionString))
            {
                await connection.ExecuteAsync(@"
                SET NOCOUNT ON;

                DELETE FROM 
                    Company
                WHERE
                    Id = @Id",
                new
                {
                    Id = id
                }).ConfigureAwait(false);
            }

            CacheProvider
                .ClearContainer("Company");
        }

        public async Task PurgeForTest()
        {
#if DEBUG
            using (SqlConnection connection =
             SqlConnectionManager.GetConnection(DbConnectionString))
            {
                await connection.ExecuteAsync(@"
                    SET NOCOUNT ON;

                    DELETE FROM 
                        Company").ConfigureAwait(false);
            }

            CacheProvider
                .ClearContainer("Company");
#endif
        }
    }
}
