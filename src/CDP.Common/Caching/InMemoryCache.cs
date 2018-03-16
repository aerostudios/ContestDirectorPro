//---------------------------------------------------------------
// Date: 1/17/2018
// Rights: 
// FileName: ICache.cs
//---------------------------------------------------------------

namespace CDP.Common.Caching
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// (very basic) In-memory cache implementation
    /// </summary>
    /// <seealso cref="CDP.Common.Caching.ICache" />
    public sealed class InMemoryCache : ICache
    {
        /// <summary>
        /// The internal cache
        /// </summary>
        Dictionary<string, object> internalCache = new Dictionary<string, object>();

        /// <summary>
        /// Clears this instance.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Clear()
        {
            internalCache.Clear();
        }

        /// <summary>
        /// Gets an item from cache with the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public T Get<T>(string key) where T : class
        {
            var returnVal = default(T);

            if (string.IsNullOrEmpty(key))
            {
                return returnVal;
            }

            if (internalCache.ContainsKey(key))
            {
                returnVal = internalCache[key] as T;
            }

            return returnVal;
        }

        /// <summary>
        /// Puts an object in cache with the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="itemToCache">The item to cache.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Put<T>(string key, T itemToCache) where T : class
        {
            try
            {
                if (this.internalCache.ContainsKey(key))
                {
                    this.internalCache[key] = itemToCache;
                    return true;
                }
                else
                {
                    this.internalCache.Add(key, internalCache);
                    return true;
                }
            }
            // Eat it.
            catch (Exception) { }

            return false;
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            if(string.IsNullOrEmpty(key)) { return false; }
            return this.internalCache.Remove(key);
        }
    }
}
