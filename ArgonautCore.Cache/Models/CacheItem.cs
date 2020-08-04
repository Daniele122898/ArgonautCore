using System;

namespace ArgonautCore.Cache.Models
{
    /// <summary>
    /// Wrapper around a cached item with additional data
    /// </summary>
    public class CacheItem
    {
        /// <summary>
        /// Actual object cached
        /// </summary>
        public object Content { get; }
        
        /// <summary>
        /// Valid until 
        /// </summary>
        public DateTime? ValidUntil { get; }

        /// <summary>
        /// Ctor 
        /// </summary>
        public CacheItem(object content, DateTime? validUntil)
        {
            this.Content = content;
            this.ValidUntil = validUntil;
        }
        
        /// <summary>
        /// Ctor
        /// </summary>
        public CacheItem(object content, in TimeSpan timeSpan)
        {
            this.Content = content;
            this.ValidUntil = DateTime.UtcNow.Add(timeSpan);
        }

        /// <summary>
        /// Check if cache item is still valid. 
        /// </summary>
        public bool IsValid()
        {
            if (this.ValidUntil.HasValue && this.ValidUntil.Value.CompareTo(DateTime.UtcNow) <= 0)
            {
                return false;
            }

            return true;
        }
    }
}