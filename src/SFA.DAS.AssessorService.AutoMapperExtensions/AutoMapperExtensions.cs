using AutoMapper;
using AutoMapper.Configuration;
using AutoMapper.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.AutoMapperExtensions
{
    public static class AutoMapperExtensions
    {
        public static void ResolveUsing<TSource, TDestination, TMember, TResult>(this IMemberConfigurationExpression<TSource, TDestination, TMember> member, Func<TSource, TResult> resolver)
            => member.MapFrom((Func<TSource, TDestination, TResult>)((src, dest) => resolver(src)));

        public static void PrintMappings<TSource, TDestination>(this IMapper mapper)
        {
            var configurationProvider = mapper.ConfigurationProvider;

            // Retrieve the TypeMap for the specific source-destination mapping
            var typeMap = configurationProvider.Internal().GetAllTypeMaps()
                .FirstOrDefault(tm => tm.SourceType == typeof(TSource) && tm.DestinationType == typeof(TDestination));

            if (typeMap == null)
            {
                Trace.WriteLine($"No mapping configuration found for {typeof(TSource).Name} -> {typeof(TDestination).Name}");
                return;
            }

            var ignoredProperties = new List<string>();
            var mappedProperties = new List<string>();

            foreach (var propertyMap in typeMap.PropertyMaps)
            {
                if (propertyMap.Ignored)
                {
                    ignoredProperties.Add(propertyMap.DestinationMember.Name);
                }
                else
                {
                    mappedProperties.Add(propertyMap.DestinationMember.Name);

                }
            }

            Trace.WriteLine($"Ignored Properties for mapping {typeof(TSource).Name} -> {typeof(TDestination).Name}:");

            foreach (var ignoredProperty in ignoredProperties)
            {
                Trace.WriteLine($"  - {ignoredProperty}");
            }

            Trace.WriteLine($"Mapped Properties for mapping {typeof(TSource).Name} -> {typeof(TDestination).Name}:");

            foreach (var mappedProperty in mappedProperties)
            {
                Trace.WriteLine($"  - {mappedProperty}");
            }
        }




    }
}


