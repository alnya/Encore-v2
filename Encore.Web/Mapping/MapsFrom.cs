namespace Encore.Web.Mapping
{
    using System;
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MapsFrom : Attribute
    {
        public Type Type { get; private set; }

        public MapsFrom(Type type)
        {
            Type = type;
        }
    }
}
