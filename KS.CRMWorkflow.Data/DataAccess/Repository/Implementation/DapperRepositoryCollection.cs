using KS.CRMWorkflow.Component.Cache.Interface;
using KS.CRMWorkflow.Data.DataAccess.Repository.Interface;

namespace KS.CRMWorkflow.Data.DataAccess.Repository.Implementation
{
    public class DapperRepositoryCollection : IRepositoryCollection
    {
        protected ICacheProvider CacheProvider { get; set; }

        public DapperRepositoryCollection(ICacheProvider cacheProvider)
        {
            CacheProvider = cacheProvider;
        }

        public ICompanyRepository Companies()
        {
            return new DapperCompanyRepository(CacheProvider);
        }
    }
}
