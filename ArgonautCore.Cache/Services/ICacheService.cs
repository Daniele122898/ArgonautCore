using System;
using System.Threading.Tasks;
using ArgonautCore.Cache.Models;
using ArgonautCore.Lw;

namespace ArgonautCore.Cache.Services
{
    /// <summary>
    /// Interface for any simple cache service
    /// </summary>
    public interface ICacheService<TKey>
    {
        /// <summary>
        /// Get the raw object if it exists 
        /// </summary>
        Option<object> Get(TKey id);
        
        /// <summary>
        /// Get the item if it exists and already cast it
        /// </summary>
        Option<T> Get<T>(TKey id);

        /// <summary>
        /// Returns true if the Cache contains an entity with the specified key
        /// </summary>
        bool Contains(TKey id);

        /// <summary>
        /// Tries to get the value out of the cache first. If it cant it will use the set function to get and cache it.
        /// </summary>
        Option<T> GetOrSetAndGet<T>(TKey id, Func<T> set, TimeSpan? ttl = null);

        /// <summary>
        /// Tries to get the value out of the cache first. If it cant it will use the set function to get and cache it.
        /// </summary>
        Task<Option<T>> GetOrSetAndGetAsync<T>(TKey id, Func<Task<T>> set, TimeSpan? ttl = null);

        /// <summary>
        /// The difference from this to <see cref="GetOrSetAndGet{T}(TKey,System.Func{T},System.Nullable{System.TimeSpan})"/> is
        /// that here we dont throw an exception and just return Maybe.Zero 
        /// </summary>
        Task<Option<T>> TryGetOrSetAndGetAsync<T>(TKey id, Func<Task<T>> set, TimeSpan? ttl = null);

        /// <summary>
        /// Adds the specified object with the key to the cache
        /// </summary>
        void Set(TKey id, object obj, TimeSpan? ttl = null);

        /// <summary>
        /// Similar to Dictionary. Will add or update atomicly the specified item.
        /// </summary>
        void AddOrUpdate(TKey id, CacheItem addItem, Func<TKey, CacheItem, CacheItem> updateFunc);

        /// <summary>
        /// Try remove the object and return it
        /// </summary>
        Option<T> TryRemove<T>(TKey id);

        /// <summary>
        /// Just try to remove the object
        /// </summary>
        void TryRemove(TKey id);
    }
}