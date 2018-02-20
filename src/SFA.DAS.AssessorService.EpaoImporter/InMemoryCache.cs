using System;
using System.Runtime.Caching;
using SFA.DAS.AssessorService.Application.Api.Client;

namespace SFA.DAS.AssessorService.EpaoImporter
{
    public class InMemoryCache : ICache
    {
        public string GetString(string key)
        {
            return MemoryCache.Default.Get(key).ToString();
        }

        public void SetString(string key, string value)
        {
            MemoryCache.Default.Set(key, value, new CacheItemPolicy(){SlidingExpiration = TimeSpan.FromMinutes(5)});
        }
    }
}