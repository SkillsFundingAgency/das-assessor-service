using AutoMapper;
using System;

namespace SFA.DAS.AssessorService.Application.Api.AutoMapperProfiles
{
    public static class AutoMapperExtensions
    {
        public static IMappingExpression<TSource, TDestination> IgnoreAll<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> mappingExpression)
        {
            mappingExpression.ForAllMembers(opts => opts.Ignore());
            return mappingExpression;
        }

        public static void ResolveUsing<TSource, TDestination, TMember, TResult>(this IMemberConfigurationExpression<TSource, TDestination, TMember> member, Func<TSource, TResult> resolver) 
            => member.MapFrom((Func<TSource, TDestination, TResult>)((src, dest) => resolver(src)));
    }
}

