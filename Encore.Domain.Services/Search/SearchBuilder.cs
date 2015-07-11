namespace Encore.Domain.Services.Search
{
    using Encore.Domain.Interfaces.Services;
    using Encore.Domain.Services.Exceptions;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    
    public class SearchBuilder<TEntityType> : ISearchTerms
    {
        private class SearchTerm : ISearchTerm
        {
            public string Property { get; set; }
            public object Value { get; set; }
            public Type Type { get; set; }
            public OperationType Operation { get; set; }
        }

        private const string ErrorMessageFormat = "Invalid value for property {0}.";

        private readonly List<ISearchTerm> properties = new List<ISearchTerm>();
        private readonly Type entityType = typeof(TEntityType);

        public SearchBuilder(IEnumerable<ISearchTerm> fromSearchTerms)
        {
            foreach (var term in fromSearchTerms)
            {
                AddTerm(term.Property, term.Value);
            }
        }

        public SearchBuilder(IEnumerable<KeyValuePair<string, object>> fromQuery)
        {
            foreach (KeyValuePair<string, dynamic> queryTerm in fromQuery)
            {
                AddTerm(queryTerm.Key, queryTerm.Value);
            }
        }

        public void AddTerm(string itemKey, object itemValue)
        {
            var operation = GetOperationType(itemKey);
            var key = itemKey.Split(':')[0];
            AddSearchTermAndValue(entityType, key, operation, itemValue);
        }

        private void AddSearchTermAndValue(Type type, string key, OperationType operation, object itemValue, string parentProperty = null)
        {
            var propertyInfo = GetPropertyInfo(type, key);

            if (propertyInfo != null && itemValue != null &&
                !String.IsNullOrWhiteSpace(itemValue.ToString()))
            {
                var value = GetObjectValue(propertyInfo, operation, itemValue);

                properties.Add(new SearchTerm
                {
                    Property = propertyInfo.Name,
                    Value = value,
                    Type = propertyInfo.PropertyType,
                    Operation = operation
                });
            }
        }

        private PropertyInfo GetPropertyInfo(Type type, string key)
        {
            return type.GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        }

        public IEnumerator<ISearchTerm> GetEnumerator()
        {
            return properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return properties.GetEnumerator();
        }

        private static OperationType GetOperationType(string key)
        {
            if (key.EndsWith(":like")) return OperationType.Contains;
            if (key.EndsWith(":from")) return OperationType.GreaterThan;
            if (key.EndsWith(":to")) return OperationType.LessThan;
            return OperationType.Contains;
        }

        private static object GetObjectValue(PropertyInfo propertyInfo, OperationType operation, object queryValue)
        {
            try
            {
                return GetObjectAsType(propertyInfo, queryValue);
            }
            catch (FormatException)
            {
                throw new DomainException(String.Format(ErrorMessageFormat, propertyInfo.Name));
            }
            catch (ArgumentException)
            {
                throw new DomainException(String.Format(ErrorMessageFormat, propertyInfo.Name));
            }
        }

        private static object GetObjectAsType(PropertyInfo propertyInfo, object queryValue)
        {
            if (propertyInfo.PropertyType.IsEnum)
            {
                return Enum.Parse(propertyInfo.PropertyType, queryValue.ToString(), ignoreCase: true);
            }

            if (Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null)
            {
                Type t = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                return (queryValue == null) ? null : Convert.ChangeType(queryValue, t);
            }

            return Convert.ChangeType(queryValue, propertyInfo.PropertyType);
        }
    }
}
