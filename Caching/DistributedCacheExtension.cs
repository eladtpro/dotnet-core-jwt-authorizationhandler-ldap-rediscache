﻿using Microsoft.Extensions.Caching.Distributed;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace MS.Poc.Server.Caching
{
    public static class DistributedCacheExtension
    {
        public static void Set<T>(this IDistributedCache distributedCache, string key, T value, DistributedCacheEntryOptions options)
        {
            distributedCache.Set(key, value.ToByteArray(), options);
        }

        public static T Get<T>(this IDistributedCache distributedCache, string key) 
            where T : class
        {
            byte[] result = distributedCache.Get(key);
            return result.FromByteArray<T>();
        }

        public async static Task SetAsync<T>(this IDistributedCache distributedCache, string key, T value, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            await distributedCache.SetAsync(key, value.ToByteArray(), options, token);
        }

        public async static Task<T> GetAsync<T>(this IDistributedCache distributedCache, string key, CancellationToken token = default(CancellationToken)) where T : class
        {
            var result = await distributedCache.GetAsync(key, token);
            return result.FromByteArray<T>();
        }

        public static byte[] ToByteArray(this object obj)
        {
            if (obj == null) return null;

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }
        public static T FromByteArray<T>(this byte[] byteArray) where T : class
        {
            if (byteArray == null)
                return default(T);

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream(byteArray))
            {
                return binaryFormatter.Deserialize(memoryStream) as T;
            }
        }
    }
}
