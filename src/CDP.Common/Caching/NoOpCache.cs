//---------------------------------------------------------------
// Date: 1/17/2018
// Rights: 
// FileName: NoOpCache.cs
//---------------------------------------------------------------

namespace CDP.Common.Caching
{
    /// <summary>
    /// Describes a cache that does exactly what you don't want a cache to do...cache nothing :).
    /// </summary>
    /// <seealso cref="ContestDirectoryPro.Common.Caching.ICache" />
    public class NoOpCache : ICache
    {
        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            return;
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public T Get<T>(string key) where T : class => default(T);

        /// <summary>
        /// Puts the specified key. Will overwrite existing item.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        public bool Put<T>(string key, T itemToCache) where T : class => true;

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool Remove(string key) => true;
    }
}
