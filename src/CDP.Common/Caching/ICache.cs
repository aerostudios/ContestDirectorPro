//---------------------------------------------------------------
// Date: 1/17/2018
// Rights: 
// FileName: ICache.cs
//---------------------------------------------------------------

namespace CDP.Common.Caching
{
    /// <summary>
    /// Defines the behaviors of a caching implementation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICache
    {
        /// <summary>
        /// Gets an item from cache with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        T Get<T>(string key) where T : class;

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        bool Remove(string key);

        /// <summary>
        /// Puts an object in cache with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="itemToCache">The item to cache.</param>
        /// <returns></returns>
        bool Put<T>(string key, T itemToCache) where T : class;

        /// <summary>
        /// Clears this instance.
        /// </summary>
        void Clear();
    }
}
