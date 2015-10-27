using System.Collections.Generic;
using System.Threading.Tasks;

namespace KS.CRMWorkflow.Data.DataAccess.Repository.Interface
{
    /// <summary>
    /// Represents an item that can perform basic CRUD operations on a data store.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataRepository<T>
    {
        /// <summary>
        /// Returns all of the items in the data store.
        /// </summary>
        /// <returns>All of the items in the data store.</returns>
        Task<IEnumerable<T>> List();

        /// <summary>
        /// Gets the item from the data store with the provided id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The item from the data store with the provided id.</returns>
        Task<T> Get(int id);

        /// <summary>
        /// Inserts an item into the data store.
        /// <exception cref="ItemAlreadyExistsException">Thrown if the specified object already exists in the data store.</exception>
        /// </summary>
        /// <param name="item"></param>
        /// <returns>The item's new Id on success, an error code of less than 0 on failure.</returns>
        Task Insert(T item);

        /// <summary>
        /// Updates an item in the data store.
        /// <exception cref="ItemAlreadyExistsException">Thrown if the specified object already exists in the data store.</exception>
        /// </summary>
        /// <param name="item"></param>
        /// <returns>The item's Id on success, an error code of less than 0 on failure.</returns>
        Task Update(T item);

        /// <summary>
        /// Deletes an item from the data store.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>1 on success, an error code of less than 0 on failure.</returns>
        Task Delete(int id);

        /// <summary>
        /// Deletes everything in the data store. Used for testing only.
        /// </summary>
        Task PurgeForTest();
    }
}
