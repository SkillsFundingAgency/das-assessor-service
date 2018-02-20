using System;
using System.Collections.Generic;
using SFA.DAS.AssessorService.Application.Api.Client;

namespace SFA.DAS.AssessorService.EpaoImporter
{
    public class InMemoryCache : ICache
    {
        private static Dictionary<string, object> _cache = new Dictionary<string, object>();

        public string GetString(string key)
        {
            return !_cache.ContainsKey(key) ? null : _cache[key].ToString();
        }

        public void SetString(string key, string value)
        {
            _cache[key] = value;
            //MemoryCache.Default.Set(key, value, new CacheItemPolicy(){SlidingExpiration = TimeSpan.FromMinutes(5)});
        }
    }
}