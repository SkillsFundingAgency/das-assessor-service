using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public static class AutoMapperExtensions
    {
        public static IMappingExpression<TSource, TDestination> MapMatchingMembersAndIgnoreOthers<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mappingExpression)
        {
            var sourceProperties = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var destinationProperties = typeof(TDestination).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var matchingProperties = new HashSet<string>(sourceProperties.Select(p => p.Name));

            foreach (var destinationProperty in destinationProperties)
            {
                if (!matchingProperties.Contains(destinationProperty.Name))
                {
                    mappingExpression.ForMember(destinationProperty.Name, opt => opt.Ignore());
                }
            }

            return mappingExpression;
        }

        public static void ResolveUsing<TSource, TDestination, TMember, TResult>(this IMemberConfigurationExpression<TSource, TDestination, TMember> member, Func<TSource, TResult> resolver) 
            => member.MapFrom((Func<TSource, TDestination, TResult>)((src, dest) => resolver(src)));

    }
}

