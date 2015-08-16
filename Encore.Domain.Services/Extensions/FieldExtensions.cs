namespace Encore.Domain.Services.Extensions
{
    using Encore.Domain.Entities;
    using System.Collections.Generic;
    using System;
    
    public static class FieldExtensions
    {
        public static Dictionary<int, Field> ToSourceIdMap(this IEnumerable<Field> fields, string projectPrefix)
        {
            var fieldMap = new Dictionary<int, Field>();

            foreach (var field in fields)
            {
                foreach (var projectId in field.ProjectIds)
                {
                    var strippedProjectId = projectId;

                    if(!String.IsNullOrEmpty(projectPrefix))
                    {
                        strippedProjectId = strippedProjectId.Replace(projectPrefix, "");
                    }

                    int sourceId;

                    if (int.TryParse(strippedProjectId, out sourceId))
                    {
                        fieldMap.Add(sourceId, field);
                    }
                }
            }

            return fieldMap;
        }
    }
}
