using System;
using System.Dynamic;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SFA.DAS.NLog.Targets.Redis.DotNetCore;
namespace SFA.DAS.AssessorService.NLog.Targets.Redis
{
    // Class taken from https://github.com/ReactiveMarkets/NLog.Targets.ElasticSearch/commit/82bd41d46e15b08f3e7770e40f0660394f8359ba
    public static class StringExtensions
    {
        private static IConfiguration _configuration;

        static  StringExtensions()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile("appsettings.Development.json", true, true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();
        }

        public static object ToSystemType(this string field, Type type, IFormatProvider formatProvider)
        {
            if (formatProvider == null)
            {
                formatProvider = CultureInfo.CurrentCulture;
            }

            switch (type.FullName)
            {
                case "System.Boolean":
                    return Convert.ToBoolean(field, formatProvider);
                case "System.Double":
                    return Convert.ToDouble(field, formatProvider);
                case "System.DateTime":
                    return Convert.ToDateTime(field, formatProvider);
                case "System.Int32":
                    return Convert.ToInt32(field, formatProvider);
                case "System.Int64":
                    return Convert.ToInt64(field, formatProvider);
                case "System.Object":
                    return JsonConvert.DeserializeObject<ExpandoObject>(field).ReplaceDotInKeys();
                default:
                    return field;
            }
        }

        public static string GetConfigurationValue(this string name)
        {
            var value = GetEnvironmentVariable(name);
            if (!string.IsNullOrEmpty(value))
                return value;
            return _configuration[name];
        }

        public static string GetConnectionString(this string name)
        {
            var value = GetEnvironmentVariable(name);
            if (!string.IsNullOrEmpty(value))
                return value;

            return _configuration.GetConnectionString(name);
        }

        private static string GetEnvironmentVariable(this string name)
        {
            return string.IsNullOrEmpty(name) ? null : Environment.GetEnvironmentVariable(name);
        }
    }
}
