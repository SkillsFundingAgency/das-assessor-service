using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SFA.DAS.AssessorService.Web.AutoMapperProfiles
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
    }
}
