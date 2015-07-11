namespace Encore.Domain.Services.Search
{
    using Encore.Domain.Interfaces.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    
    public class SortBuilder<TEntityType> : ISortCriteria
    {
        private readonly Type entityType = typeof(TEntityType);

        public string SortBy { get; private set; }

        public bool SortDescending { get; private set; }

        public SortBuilder(Expression<Func<TEntityType, object>> propertyLambda, bool sortDescending = true)
        {
            MemberExpression expr = GetMemberExpression(propertyLambda);
            SortBy = expr.Member.Name;
            SortDescending = sortDescending;
        }

        public SortBuilder(ISortCriteria sortCriteria)
        {
            SortBy = sortCriteria.SortBy;
            SortDescending = sortCriteria.SortDescending;
        }

        public SortBuilder(string propertyName, string sortDescendingString)
        {
            if (!String.IsNullOrWhiteSpace(propertyName))
            {
                var propertyInfo = GetPropertyInfo(entityType, propertyName);

                if (propertyInfo != null)
                {
                    SortBy = propertyInfo.Name;
                }
            }

            if (!String.IsNullOrWhiteSpace(sortDescendingString))
            {
                bool sortDescending;

                if (bool.TryParse(sortDescendingString, out sortDescending))
                {
                    SortDescending = sortDescending;
                }
            }
        }

        private MemberExpression GetMemberExpression(Expression<Func<TEntityType, object>> property)
        {
            if (property == null)
                throw new ArgumentNullException("property", "propertyRefExpr is null.");

            var memberExpr = property.Body as MemberExpression;
            if (memberExpr == null)
            {
                var unaryExpr = property.Body as UnaryExpression;
                if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
                    memberExpr = unaryExpr.Operand as MemberExpression;
            }

            if (memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property)
            {
                return memberExpr;
            }

            throw new ArgumentException("No property reference expression was found.", "property");
        }

        private PropertyInfo GetPropertyInfo(Type type, string key)
        {
            return type.GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        }
    }
}
