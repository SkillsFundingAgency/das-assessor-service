using System.Linq.Expressions;
using System;
using System.Security.AccessControl;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Castle.Components.DictionaryAdapter;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public class HandlerBase
    {
        protected static string NullQueryParam<T, TKey>(T item, Expression<Func<T, TKey>> propertySelector)
        {
            GetPropertyNameAndValue(item, propertySelector, out string propertyName, out TKey propertyValue);

            if (!IsNullableType(typeof(TKey)))
            {
                throw new InvalidOperationException($"Property '{propertyName}' is not nullable.");
            }
            
            if (Equals(propertyValue, default(TKey)))
            {
                return $"[{propertyName}] IS NULL";
            }

            return QueryParam(propertyName, propertyValue);
        }

        protected static string NotNullQueryParam<T, TKey>(T item, Expression<Func<T, TKey>> propertySelector)
        {
            GetPropertyNameAndValue(item, propertySelector, out string propertyName, out TKey propertyValue);
            return QueryParam(propertyName, propertyValue);
        }

        private static string QueryParam<TKey>(string propertyName, TKey propertyValue)
        {
            if (typeof(TKey) == typeof(DateTime) || typeof(TKey) == typeof(DateTime?))
            {
                if (IsDateOnly(DateTime.Parse(propertyValue.ToString())))
                {
                    // convert to a DATE so that SQL can compare correctly
                    return $"[{propertyName}] = CONVERT(DATE,@{LowercaseFirstLetter(propertyName)})";
                }
                else
                {
                    // convert to yyyy-MM-dd HH:mm:ss to remove milliseconds which sometimes have rounding errors
                    return $"CONVERT(VARCHAR,[{propertyName}], 120) = CONVERT(VARCHAR,@{LowercaseFirstLetter(propertyName)}, 120)";
                }
            }

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

        private static bool IsDateOnly(DateTime dateTime)
        {
            return dateTime.TimeOfDay == TimeSpan.Zero;
        }

        private static bool IsNullableType(Type type)
        {
            return !type.IsValueType || (Nullable.GetUnderlyingType(type) != null);
        }

        public static string GetAcademicYear(DateTime date)
        {
            if (date.Month < 8)
            {
                return (date.Year - 1).ToString().Substring(2) + date.Year.ToString().Substring(2);
            }
            else
            {
                return date.Year.ToString().Substring(2) + (date.Year + 1).ToString().Substring(2);
            }
        }
    }
}
