using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using ArgonautCore.Lw;

namespace ArgonautCore.Cache.Services
{
    /// <summary>
    /// This is the base implementation of a <see cref="ICacheService{TKey}"/> instance. This can be used as is or expanded / altered.
    /// </summary>
    public class CacheBase<TKey> : ICacheService<TKey>
    {
        private readonly ConcurrentDictionary<TKey, CacheItem> _cache = new ConcurrentDictionary<TKey, CacheItem>();

        // I'm not sure if its a smart idea to remove all the pointers to a 
        // reoccurring event. I dont want it to be GC'd. Not sure if that is a
        // thing but i'll just leave it to be sure :)
        // ReSharper disable once NotAccessedField.Local
        private readonly Timer _timer;

        private const short _CACHE_CLEAN_DELAY = 60;

        /// <summary>
        /// Default constructor initializing the clean cache function.
        /// </summary>
        public CacheBase()
        {
            _timer = new Timer(CleanCache, null, TimeSpan.FromSeconds(_CACHE_CLEAN_DELAY),
                TimeSpan.FromSeconds(_CACHE_CLEAN_DELAY));
        }
        
        private void CleanCache(object stateInfo)
        {
            var dKeys = _cache.Keys;
            foreach (var key in dKeys)
            {
                if (!_cache.TryGetValue(key, out var item)) continue;
                if (!item.IsValid())
                {
                    _cache.TryRemove(key, out _);
                }
            }
        }

        /// <inheritdoc />
        public virtual Option<object> Get(TKey id)
        {
            if (!_cache.TryGetValue(id, out var item))
                return Option.None<object>();
            if (item.IsValid()) return Option.Some<object>(item.Content);

            _cache.TryRemove(id, out _);
            return Option.None<object>();
        }

        /// <inheritdoc />
        public virtual Option<TVal> Get<TVal>(TKey id)
        {
            if (!_cache.TryGetValue(id, out var item))
                return Option.None<TVal>();
            if (item.IsValid()) return Option.Some<TVal>((TVal) item.Content);

            _cache.TryRemove(id, out _);
            return Option.None<TVal>();
        }


        /// <inheritdoc />
        public virtual bool Contains(TKey id) => _cache.ContainsKey(id);

        /// <inheritdoc />
        public virtual Option<TVal> GetOrSetAndGet<TVal>(TKey id, Func<TVal> set, TimeSpan? ttl = null)
        {
            if (_cache.TryGetValue(id, out var item) && item != null && item.IsValid())
            {
                return (TVal)item.Content;
            }
            // Otherwise we have to set it
            TVal result = set();
            if (result == null)
            {
                throw new ArgumentException("Result of the set function was null. This is NOT acceptable");
            }
            var itemToStore = new CacheItem(result, ttl.HasValue ? (DateTime?)DateTime.UtcNow.Add(ttl.Value) : null);
            _cache.AddOrUpdate(id, itemToStore, ((key, cacheItem) => itemToStore));
            return (TVal) itemToStore.Content;
        }

        /// <inheritdoc />
        public virtual async Task<Option<TVal>> GetOrSetAndGetAsync<TVal>(TKey id, Func<Task<TVal>> set, TimeSpan? ttl = null)
        {
            if (_cache.TryGetValue(id, out var item) && item != null && item.IsValid())
            {
                return (TVal)item.Content;
            }
            // Otherwise we have to set it
            TVal result = await set().ConfigureAwait(false);
            if (result == null)
            {
                throw new ArgumentException("Result of the set function was null. This is NOT acceptable");
            }
            var itemToStore = new CacheItem(result, ttl.HasValue ? (DateTime?)DateTime.UtcNow.Add(ttl.Value) : null);
            _cache.AddOrUpdate(id, itemToStore, ((key, cacheItem) => itemToStore));
            return (TVal) itemToStore.Content;
        }

        /// <inheritdoc />
        public virtual async Task<Option<TVal>> TryGetOrSetAndGetAsync<TVal>(TKey id, Func<Task<TVal>> set,
            TimeSpan? ttl = null)
        {
            if (_cache.TryGetValue(id, out var item) && item != null && item.IsValid())
            {
                return Option.Some((TVal)item.Content);
            }
            // Otherwise we have to set it
            TVal result = await set().ConfigureAwait(false);
            if (result == null)
            {
                return Option.None<TVal>();
            }
            var itemToStore = new CacheItem(result, ttl.HasValue ? (DateTime?)DateTime.UtcNow.Add(ttl.Value) : null);
            _cache.AddOrUpdate(id, itemToStore, ((key, cacheItem) => itemToStore));
            return Option.Some((TVal) itemToStore.Content);
        }

        /// <inheritdoc />
        public virtual void Set(TKey id, object obj, TimeSpan? ttl = null)
        {
            var itemToStore = new CacheItem(obj, ttl.HasValue ? (DateTime?)DateTime.UtcNow.Add(ttl.Value) : null);
            _cache.AddOrUpdate(id, itemToStore, ((key, cacheItem) => itemToStore));
        }

        /// <inheritdoc />
        public virtual void AddOrUpdate(TKey id, CacheItem addItem, Func<TKey, CacheItem, CacheItem> updateFunc)
        {
            this._cache.AddOrUpdate(id, addItem, updateFunc);
        }

        /// <inheritdoc />
        public virtual Option<TVal> TryRemove<TVal>(TKey id)
        {
            if (!_cache.TryRemove(id, out var cacheItem))
                return Option.None<TVal>();
            if (!cacheItem.IsValid()) return Option.None<TVal>();
            return Option.Some((TVal) cacheItem.Content);
        }

        /// <inheritdoc />
        public virtual void TryRemove(TKey id)
        {
         _cache.TryRemove(id, out _);   
        }
    }
}