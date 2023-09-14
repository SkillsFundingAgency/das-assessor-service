using System.Linq.Expressions;
using System;
using System.Security.AccessControl;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public class HandlerBase
    {
        protected static string NullQueryParam<T, TKey>(T item, Expression<Func<T, TKey>> propertySelector)
        {
            GetPropertyNameAndValue(item, propertySelector, out string propertyName, out TKey propertyValue);
            return $"[{propertyName}] {(propertyValue == null ? "IS NULL" : "= @" + LowercaseFirstLetter(propertyName))}";
        }

        protected static string NotNullQueryParam<T, TKey>(T item, Expression<Func<T, TKey>> propertySelector)
        {
            GetPropertyNameAndValue(item, propertySelector, out string propertyName, out TKey propertyValue);

            return $"[{propertyName}] = @{LowercaseFirstLetter(propertyName)}";
        }

        private static void GetPropertyNameAndValue<T, TKey>(T item, Expression<Func<T, TKey>> propertySelector, out string propertyName, out TKey propertyValue)
        {
            var memberExpression = propertySelector.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("The provided expression is not a MemberExpression.", nameof(propertySelector));
            }

            propertyName = memberExpression.Member.Name;
            propertyValue = propertySelector.Compile().Invoke(item);
        }

        private static string LowercaseFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return char.ToLower(input[0]) + input.Substring(1);
        }
    }
}
