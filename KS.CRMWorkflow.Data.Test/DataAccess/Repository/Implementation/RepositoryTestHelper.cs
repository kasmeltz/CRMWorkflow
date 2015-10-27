using KS.CRMWorkflow.Component.Cache.Implementation;
using KS.CRMWorkflow.Component.Cache.Interface;
using KS.CRMWorkflow.Data.DataAccess.Repository.Implementation;
using KS.CRMWorkflow.Data.DataAccess.Repository.Interface;

namespace RDD.SalesTracker.Data.Test.DataAccess.Repository.Implementation
{
    public class RepositoryTestHelper
    {
        public static void PrepareForTest()
        {
            // Arrange
            ICacheProvider cacheProvider = MemoryCacheProvider.Instance;
            IRepositoryCollection collection =
                new DapperRepositoryCollection(cacheProvider);
            ICompanyRepository companyRepository = collection.Companies();

            // Act
            companyRepository.PurgeForTest().Wait();
        }
    }
}
