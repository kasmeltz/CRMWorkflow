using System;

namespace KS.CRMWorkflow.Component.Cache.Interface
{
    /// <summary>
    /// Represents an item that provides a cache for rapid data access.
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// Adds an item to the cache for the default length of time.
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="key"></param>
        /// <param name="item"></param>
        void Insert(string containerName, string key, object item);

        /// <summary>
        /// Adds an item to the cache for an explicit length of time.
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <param name="timeToLive"></param>
        void Insert(string containerName, string key, object item, DateTimeOffset offset);

        /// <summary>
        /// Returns the object from the cache inserted with the provided key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The object from the cache inserted with the provided key.</returns>
        object Get(string key);

        /// <summary>
        /// Removes the object from the cache inserted with the provided key
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);

        /// <summary>
        /// Empties one of the cache containers.
        /// </summary>
        /// <param name="containerName"></param>
        void ClearContainer(string containerName);

        /// <summary>
        /// Purges the entire cache.
        /// </summary>
        void Purge();
    }
}
