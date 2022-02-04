using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SFA.DAS.AssessorService.Data.Extensions
{
    public static class EntityFrameworkExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyName)
        {
            return OrderByMethodName<T>(query, propertyName, "OrderBy");
        }

        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string propertyName)
        {
            return OrderByMethodName<T>(query, propertyName, "OrderByDescending");
        }

        private static IQueryable<T> OrderByMethodName<T>(this IQueryable<T> query, string propertyName, string orderByMethodName)
        {
            try
            {
                var entityType = typeof(T);

                //Create x=>x.PropName
                var propertyInfo = entityType.GetProperty(propertyName);
                ParameterExpression arg = Expression.Parameter(entityType, "x");
                MemberExpression property = Expression.Property(arg, propertyName);
                var selector = Expression.Lambda(property, new ParameterExpression[] { arg });

                //Get System.Linq.Queryable.OrderBy() method.
                var enumarableType = typeof(System.Linq.Queryable);
                var method = enumarableType.GetMethods()
                     .Where(m => m.Name == orderByMethodName && m.IsGenericMethodDefinition)
                     .Where(m =>
                     {
                         var parameters = m.GetParameters().ToList();
                         return parameters.Count == 2;
                     }).Single();

                //The linq's OrderByDescending<TSource, TKey> has two generic types, which provided here
                MethodInfo genericMethod = method.MakeGenericMethod(entityType, propertyInfo.PropertyType);

                // Call query.OrderByDescending(selector), with query and selector: x=> x.PropName
                // Note that we pass the selector as Expression to the method and we don't compile it.
                // By doing so EF can extract "order by" columns and generate SQL for it
                var newQuery = (IQueryable<T>)genericMethod.Invoke(genericMethod, new object[] { query, selector });
                return newQuery;
            }
            catch (Exception)
            {
                return query;
            }
        }
    }
}
