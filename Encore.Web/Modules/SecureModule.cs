namespace Encore.Web.Modules
{
    using AutoMapper;
    using Nancy;
    using Nancy.Security;
    using System;
    using Extensions;
    
    public abstract class SecureModule : BaseModule
    {
        public SecureModule(string modulePath, IMappingEngine mappingEngine)
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
