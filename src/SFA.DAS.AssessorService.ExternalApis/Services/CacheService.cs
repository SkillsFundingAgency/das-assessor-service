﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.ExternalApis.Services
{
    public class CacheService
    {
        private readonly IDistributedCache _distributedCache;
        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public async Task SaveToCache<T>(string key, T item, int expirationInHours)
        {
            var json = JsonConvert.SerializeObject(item);
            await _distributedCache.SetStringAsync(key, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(expirationInHours)
            });
        }

        public async Task<T> RetrieveFromCache<T>(string key)
        {
            var json = await _distributedCache.GetStringAsync(key);
            return json == null ? default(T) : JsonConvert.DeserializeObject<T>(json);
        }
    }
}
