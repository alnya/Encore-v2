namespace Encore.Web.Modules
{
    using AutoMapper;
    using Encore.Domain.Interfaces.Services;
    using Encore.Web.Models;
    using Nancy;
    using Nancy.ModelBinding;
    using Extensions;
    using System;
    using Encore.Domain.Entities;

    public class MyAccountModule : SecureModule
    {
        public MyAccountModule(IUserService userService, IMappingEngine mappingEngine)
            : base("data/users", mappingEngine, UserRole.Standard)
        {   
            Get["/myAccount"] = args =>
            {
                var authorizedUser = Context.GetAuthorizedUser();

                var foundUser = userService.Get(authorizedUser.Id);
                return MapTo<User>(foundUser);
            };

            Put["/myAccount"] = args =>
            {
                var model = this.BindAndValidate<Models.User>();

                if (!ModelValidationResult.IsValid)
                {
                    return RespondWithValidationFailure(ModelValidationResult);
                }

                var authorizedUser = Context.GetAuthorizedUser();
                var updateEntity = MapTo<SystemUser>(model);
                var updatedUser = userService.UpdateUser(authorizedUser.Id, authorizedUser.Id, updateEntity);

                Context.SetupSession(updatedUser);
                return MapTo<User>(updatedUser);
            };
        }
    }
}
