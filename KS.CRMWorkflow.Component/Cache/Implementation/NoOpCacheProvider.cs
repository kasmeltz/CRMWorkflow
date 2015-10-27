using RDD.SalesTracker.Component.Cache.Interface;
using System;

namespace RDD.SalesTracker.Component.Cache.Implementation
{
    /// <summary>
    /// Represents an item that doesn't actually cache anything. 
    /// </summary>
    public class NoOpCacheProvider : ICacheProvider
    {
        public void ClearContainer(string containerName)
        {            
        }

        public object Get(string key)
        {
            return null;
        }

        public void Insert(string containerName, string key, object item)
        {
        }

        public void Insert(string containerName, string key, object item, DateTimeOffset offset)
        {
        }

        public void Purge()
        {
        }

        public void Remove(string key)
        {
        }
    }
}
