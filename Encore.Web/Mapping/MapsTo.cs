namespace Encore.Web.Mapping
{
    using System;
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MapsTo : Attribute
    {
        public Type Type { get; private set; }

        public MapsTo(Type type)
        {
            Type = type;
        }
    }
}
