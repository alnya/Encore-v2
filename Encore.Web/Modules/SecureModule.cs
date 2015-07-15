namespace Encore.Web.Modules
{
    using AutoMapper;
    using Nancy.Security;
    using System;
    using Extensions;
    
    public abstract class SecureModule : BaseModule
    {
        protected SecureModule(string modulePath, IMappingEngine mappingEngine)
            : base(modulePath, mappingEngine)
        {
            this.RequiresAuthentication();
        }

        public Guid AuthorizedUserId
        {
            get
            {
                return Context.GetAuthorizedUser().Id;
            }
        }
    }
}
