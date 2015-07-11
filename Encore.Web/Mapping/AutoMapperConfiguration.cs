namespace Encore.Web.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Models;

    public static class AutoMapperConfiguration
    {
        public static void Configure(IConfiguration configuration, Type[] types)
        {
            LoadStandardMappings(configuration, types);
            LoadCustomMappings(configuration, types);
        }

        private static void LoadStandardMappings(IConfiguration configuration, IEnumerable<Type> types)
        {
            var classTypes = types.Where(t => !t.IsAbstract && !t.IsInterface).ToList();

            var maps = classTypes
                .SelectMany(t => Attribute.GetCustomAttributes(t, typeof(MapsFrom)).Select(x => (MapsFrom)x)
                    .Select(a => new { From = a.Type, To = t }))
                .Union(classTypes.SelectMany(t => Attribute.GetCustomAttributes(t, typeof(MapsTo)).Select(x => (MapsTo)x)
                    .Select(a => new { From = t, To = a.Type })));

            foreach (var map in maps)
            {
                configuration.CreateMap(map.From, map.To);
            }
        }

        private static void LoadCustomMappings(IConfiguration configuration, IEnumerable<Type> types)
        {
            var maps = types
                .Where(t => !t.IsAbstract && !t.IsInterface && typeof(IHaveCustomMappings).IsAssignableFrom(t))
                .Select(t => (IHaveCustomMappings)Activator.CreateInstance(t));

            foreach (var map in maps)
            {
                map.CreateMappings(configuration);
            }
        }
    }
}
