using KS.CRMWorkflow.Component.Cache.Interface;
using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace KS.CRMWorkflow.Component.Cache.Implementation
{
    /// <summary>
    /// Represents an item that provides a data cache using
    /// System.Runtime.Caching.MemoryCache
    /// </summary>
    public class MemoryCacheProvider : ICacheProvider
    {
        protected Dictionary<string, Dictionary<string, bool>> ContainerKeys;

        private static volatile MemoryCacheProvider instance;
        private static object syncRoot = new Object();

        public static MemoryCacheProvider Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new MemoryCacheProvider();
                    }
                }

                return instance;
            }
        }

        private MemoryCacheProvider()
        {
            ContainerKeys = new Dictionary<string, Dictionary<string, bool>>();
        }

        public void ClearContainer(string containerName)
        {
            if (string.IsNullOrEmpty(containerName))
            {
                return;
            }

            if (ContainerKeys.ContainsKey(containerName))
            {
                foreach (string key in ContainerKeys[containerName].Keys)
                {
                    MemoryCache.Default.Remove(key);
                }

                ContainerKeys[containerName].Clear();
            }
        }

        public object Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            return MemoryCache.Default.Get(key);
        }

        public void Insert(string containerName, string key, object item)
        {
            if (item == null)
            {
                return;
            }

            DateTimeOffset offset = new DateTimeOffset(DateTime.UtcNow.AddMinutes(10));
            Insert(containerName, key, item, offset);
        }

        public void Insert(string containerName, string key, object item, DateTimeOffset offset)
        {

            try
            {
                if (item == null || string.IsNullOrEmpty(key))
                {
                    return;
                }

                if (!ContainerKeys.ContainsKey(containerName))
                {
                    ContainerKeys[containerName] = new Dictionary<string, bool>();
                }

                ContainerKeys[containerName][key] = true;

                MemoryCache.Default.Add(key, item, offset);
            }
            catch
            {
            }
        }

        public void Purge()
        {
            foreach (string containerName in ContainerKeys.Keys)
            {
                ClearContainer(containerName);
            }
        }

        public void Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            MemoryCache.Default.Remove(key);
        }
    }
}
