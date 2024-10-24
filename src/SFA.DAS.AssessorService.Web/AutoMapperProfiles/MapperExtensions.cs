using AutoMapper;

namespace SFA.DAS.AssessorService.Web.AutoMapperProfiles
{
    public static class AutoMapperExtensions
    {
        public static IMappingExpression<TSource, TDestination> IgnoreAll<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> mappingExpression)
        {
            mappingExpression.ForAllMembers(opts => opts.Ignore());
            return mappingExpression;
        }
    }
}
