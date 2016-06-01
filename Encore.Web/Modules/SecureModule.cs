namespace Encore.Web.Modules
{
    using AutoMapper;
    using Nancy.Security;
    using System;
    using Extensions;
    using Encore.Domain.Entities;
    using Nancy.Responses;
    using Nancy;
    using System.Collections.Generic;
    
    public abstract class SecureModule: BaseModule
    {
        private readonly UserRole requiredRole;

        public SecureModule(string modulePath, IMappingEngine mappingEngine, UserRole requiredRole)
            : base(modulePath, mappingEngine)
        {
            this.RequiresAuthentication();
            this.requiredRole = requiredRole;

            Before += (ctx) =>
            {
                if (IsAuthorized(ctx.CurrentUser))
                {
                    return null;
                }

                return new Response
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    ReasonPhrase = "User is not allowed to perform action."
                };
            };
        }

        private bool IsAuthorized(IUserIdentity userIdentity)
        {
            if (requiredRole == UserRole.Admin)
            {
                return userIdentity.HasClaim("Admin");
            }

            return true;
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
