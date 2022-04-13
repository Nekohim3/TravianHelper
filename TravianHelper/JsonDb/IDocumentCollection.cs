using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TravianHelper.JsonDb
{
    /// <summary>
    /// Collection of items
    /// </summary>
    /// <typeparam name="T">Type of item</typeparam>
    public interface IDocumentCollection<T>
    {
        /// <summary>
        /// Collection as queryable
        /// </summary>
        /// <returns>All items in queryable collection</returns>
        IEnumerable<T> AsQueryable();

        /// <summary>
        /// Find all items matching the query
        /// </summary>
        /// <param name="query">Filter predicate</param>
        /// <returns>Items matching the query</returns>
        IEnumerable<T> Find(Predicate<T> query);

        /// <summary>
        /// Full-text search
        /// </summary>
        /// <param name="text">Search text</param>
        /// <param name="caseSensitive">Is the search case sensitive</param>
        /// <returns>Items matching the search text</returns>
        IEnumerable<T> Find(string text, bool caseSensitive = false);

        /// <summary>
        /// Get next value for id field
        /// </summary>
        /// <returns>Integer or string identifier</returns>
        dynamic GetNextIdValue();

        /// <summary>
        /// Insert single item
        /// </summary>
        /// <param name="item">New item to be inserted</param>
        /// <returns>true if operation is successful</returns>
        bool Insert(T item);

        /// <summary>
        /// Update the first item matching the filter
        /// </summary>
        /// <param name="filter">First item matching the predicate will be replaced</param>
        /// <param name="item">New content</param>
        /// <returns>true if item found for update</returns>
        bool Update(Predicate<T> filter, dynamic item);

        /// <summary>
        /// Update the item matching the id
        /// </summary>
        /// <param name="id">The item matching the id-value will be replaced</param>
        /// <param name="item">New content</param>
        /// <returns>true if item found for update</returns>
        bool Update(dynamic id, dynamic item);
        bool Update(T item);

        /// <summary>
        /// Delete first item matching the filter
        /// </summary>
        /// <param name="filter">First item matching the predicate will be deleted</param>
        /// <returns>true if item found for deletion</returns>
        bool Delete(Predicate<T> filter);

        /// <summary>
        /// Delete the item matching the id
        /// </summary>
        /// <param name="id">The item matching the id-value will be deleted</param>
        /// <returns>true if item found for deletion</returns>
        bool Delete(dynamic id);
        bool Delete(T item);

        /// <summary>
        /// Number of items in the collection
        /// </summary>
        int Count { get; }
    }
}