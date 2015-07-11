namespace Encore.DataStore
{
    using Encore.Domain.Interfaces.Services;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class QueryExtensions
    {
        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, ISortCriteria sortCriteria) where TEntity : class
        {
            if (String.IsNullOrEmpty(sortCriteria.SortBy))
                return source;

            var parameter = Expression.Parameter(typeof(TEntity));
            MemberExpression member = Expression.Property(parameter, sortCriteria.SortBy);
            var orderByExpression = Expression.Lambda(member, parameter);
            LambdaExpression untyped = orderByExpression;

            return sortCriteria.SortDescending
                ? Queryable.OrderByDescending(source, (dynamic)untyped)
                : Queryable.OrderBy(source, (dynamic)untyped);
        }

        public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> source, ISearchTerms searchTerms) where TEntity : class
        {
            if (searchTerms == null || !searchTerms.Any())
                return source;

            var entityType = typeof(TEntity);
            ParameterExpression param = Expression.Parameter(entityType, "t");
            Expression exp = null;

            foreach (var searchTerm in searchTerms)
            {
                exp = exp == null ? param.GetExpression(searchTerm) : Expression.AndAlso(exp, param.GetExpression(searchTerm));
            }

            var lambda = Expression.Lambda<Func<TEntity, bool>>(exp, param);
            return source.Where(lambda).AsQueryable();
        }

        private static Expression GetExpression(this ParameterExpression param, ISearchTerm searchTerm)
        {
            return Expression.Property(param, searchTerm.Property).BuildMemberExpression(searchTerm);
        }

        private static Expression BuildMemberExpression(this MemberExpression expression, ISearchTerm searchTerm)
        {
            if (searchTerm.Type == typeof(string))
            {
                return StringExpression(expression, searchTerm);
            }

            if (searchTerm.Type == typeof(DateTime) || searchTerm.Type == typeof(DateTime?))
            {
                return DateTimeExpression(expression, searchTerm);
            }

            return NumericExpression(expression, searchTerm);
        }

        private static Expression StringExpression(MemberExpression member, ISearchTerm searchTerm)
        {
            // Make string searches case insensitive
            var methodInfo = typeof(String).GetMethod("ToLower", new Type[] { });
            var lowerExpression = Expression.Call(member, methodInfo);
            return Expression.Call(lowerExpression, "Contains", null, Expression.Constant("" + searchTerm.Value.ToString().ToLower() + ""));
        }

        private static Expression DateTimeExpression(MemberExpression member, ISearchTerm searchTerm)
        {
            // dates can only be greater than, less than or equal to a date (default)
            DateTime dateValue = Convert.ToDateTime(searchTerm.Value).Date;

            switch (searchTerm.Operation)
            {
                case OperationType.GreaterThan:
                    return Expression.GreaterThanOrEqual(member, Expression.Constant(dateValue, searchTerm.Type));
                case OperationType.LessThan:
                    return Expression.LessThan(member, Expression.Constant(dateValue.AddDays(1), searchTerm.Type));
                default:
                    return Expression.AndAlso(
                        Expression.GreaterThanOrEqual(member, Expression.Constant(dateValue, searchTerm.Type)),
                        Expression.LessThan(member, Expression.Constant(dateValue.AddDays(1), searchTerm.Type)));
            }
        }

        private static Expression NumericExpression(MemberExpression member, ISearchTerm searchTerm)
        {
            // otherwise numeric - can only be greater than, less than, in or equal to (default)
            switch (searchTerm.Operation)
            {
                case OperationType.GreaterThan:
                    {
                        return Expression.GreaterThan(member, Expression.Constant(searchTerm.Value, searchTerm.Type));
                    }
                case OperationType.LessThan:
                    {
                        return Expression.LessThan(member, Expression.Constant(searchTerm.Value, searchTerm.Type));
                    }
                default:
                    {
                        return Expression.Equal(member, Expression.Constant(searchTerm.Value, searchTerm.Type));
                    }
            }
        }
    }
}
