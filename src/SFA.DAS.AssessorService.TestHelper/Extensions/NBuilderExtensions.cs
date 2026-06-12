using FizzWare.NBuilder;
using System.Linq.Expressions;
using System.Reflection;

namespace SFA.DAS.AssessorService.TestHelper
{
    public static class NBuilderExtensions
    {
        public static ISingleObjectBuilder<T> WithPrivate<T, TProperty>(this
            ISingleObjectBuilder<T> b, Expression<Func<T, TProperty>> property,
            TProperty value)
        {
            b.Do(x => ((PropertyInfo)
            ((MemberExpression)property.Body).Member).SetValue(x, value, null));
            return b;
        }

        public static IOperable<T> WithPrivate<T, TProperty>(this
            IOperable<T> b, Expression<Func<T, TProperty>> property,
            TProperty value)
        {
            b.Do(x => ((PropertyInfo)
            ((MemberExpression)property.Body).Member).SetValue(x, value, null));
            return b;
        }
    }
}
