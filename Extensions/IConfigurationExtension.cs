using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace MS.Poc.Server.Extensions
{
    public static class IConfigurationExtension
    {
        public static IList<T> List<T>(this IConfiguration configuration, string key)
        {
            IConfigurationSection section = configuration.GetSection(key);
            T[] values = section.Get<T[]>();
            return values.ToList();
        }
    }
}
