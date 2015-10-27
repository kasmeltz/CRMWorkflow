using KS.CRMWorkflow.Component.Cache.Implementation;
using KS.CRMWorkflow.Component.Cache.Interface;
using KS.CRMWorkflow.Data.DataAccess.Repository.Implementation;
using KS.CRMWorkflow.Data.DataAccess.Repository.Interface;
using KS.CRMWorkflow.Data.POCO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RDD.SalesTracker.Data.Test.DataAccess.Repository.Implementation
{
    [TestClass]
    public class DapperCompanyRepositoryTest
    {
        [TestInitialize]
        public void Initialize()
        {
            RepositoryTestHelper.PrepareForTest();
        }

        [TestMethod]
        public void TestCompanyRepository()
        {
            // Arrange
            ICacheProvider cacheProvider = MemoryCacheProvider.Instance;
            IRepositoryCollection collection = 
                new DapperRepositoryCollection(cacheProvider);
            ICompanyRepository repository = collection.Companies();

            IEnumerable<Company> companies = null;
            Company company1 = null;
            Company company2 = null;
            Company company3 = null;
            Company company4 = null;
            Company company5 = null;
            Company getCompany1 = null;
            Company getCompany2 = null;
            Company getCompany3 = null;
            Company getCompany4 = null;
            bool exceptionThrown = false;

            company1 = new Company { Name = "Coca-Cola" };
            company2 = new Company { Name = "KS Software" };
            company3 = new Company { Name = "Apple" };
            company4 = new Company { Name = "Orange" };

            // Act
            repository.Insert(company1).Wait();        
            repository.Insert(company2).Wait();
            repository.Insert(company3).Wait();
            repository.Insert(company4).Wait();

            // Assert
            getCompany1 = repository.Get(company1.Id).Result;
            Assert.AreEqual(company1.Id, getCompany1.Id);
            Assert.AreEqual("Coca-Cola", getCompany1.Name);

            getCompany2 = repository.Get(company2.Id).Result;
            Assert.AreEqual(company2.Id, getCompany2.Id);
            Assert.AreEqual("KS Software", getCompany2.Name);

            getCompany3 = repository.Get(company3.Id).Result;
            Assert.AreEqual(company3.Id, getCompany3.Id);
            Assert.AreEqual("Apple", getCompany3.Name);

            getCompany4 = repository.Get(company4.Id).Result;
            Assert.AreEqual(company4.Id, getCompany4.Id);
            Assert.AreEqual("Orange", getCompany4.Name);
            
            exceptionThrown = false;
            try
            {
                // Act
                company5 = new Company { Name = "Coca-Cola" };
                repository.Insert(company5).Wait();
            }
            catch (AggregateException)
            {
                exceptionThrown = true;
            }

            // Assert
            Assert.IsTrue(exceptionThrown);

            // Act            
            companies = repository.List().Result;
            Assert.AreEqual(4, companies.Count());

            // Act            
            repository.Delete(company1.Id).Wait();
            companies = repository.List().Result;
            getCompany1 = repository.Get(company1.Id).Result;

            // Assert
            Assert.AreEqual(3, companies.Count());
            Assert.IsNull(getCompany1);

            // Act
            company2.Name = "Pepsi";
            repository.Update(company2).Wait();
            getCompany2 = repository.Get(company2.Id).Result;

            // Assert
            Assert.AreEqual("Pepsi", company2.Name);

            exceptionThrown = false;
            try
            {
                // Act
                company3.Name = "Bucaramunga";
                repository.Update(company3).Wait();
            }
            catch (AggregateException)
            {
                exceptionThrown = true;
            }
            getCompany3 = repository.Get(company3.Id).Result;

            // Assert
            Assert.IsTrue(exceptionThrown);
            Assert.AreEqual("Apple", getCompany3.Name);

            // Act            
            repository.Delete(company2.Id);
            repository.Delete(company3.Id);
            repository.Delete(company4.Id);
            companies = repository.List().Result;

            // Assert
            Assert.AreEqual(0, companies.Count());
        }
    }
}
