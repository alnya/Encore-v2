namespace Encore.Domain.Services.Extensions
{
    using Encore.Domain.Entities;
    using System.Collections.Generic;
    
    public static class FieldExtensions
    {
        public static Dictionary<int, Field> ToProjectIdMap(this IEnumerable<Field> fields)
        {
            var fieldMap = new Dictionary<int, Field>();

            foreach (var field in fields)
            {
                foreach (var projectId in field.ProjectIds)
                {
                    fieldMap.Add(projectId, field);
                }
            }

            return fieldMap;
        }
    }
}
