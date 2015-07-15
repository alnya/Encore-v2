namespace Encore.Web.Modules
{
    using AutoMapper;
    using Encore.Domain.Interfaces.Services;
    using Encore.Web.Models;
    using Nancy;
    using Nancy.ModelBinding;
    using Extensions;

    public class UserModule : SecureModule
    {
        public UserModule(IUserService userService, IMappingEngine mappingEngine)
            : base("data", mappingEngine)
        {
            Get["/account"] = args =>
            {
                var authorizedUser = Context.GetAuthorizedUser();

                var foundUser = userService.GetUser(authorizedUser.Id);
                return MapTo<User>(foundUser);
            };

            Put["/account"] = args =>
            {
                var model = this.BindAndValidate<Models.User>();

                if (!ModelValidationResult.IsValid)
                {
                    return RespondWithValidationFailure(ModelValidationResult);
                }

                var authorizedUser = Context.GetAuthorizedUser();

                var updatedUser = userService.UpdateUser(
                    authorizedUser.Id, 
                    model.Name, 
                    model.Password, 
                    model.Email);

                Context.SetupSession(updatedUser);
                return MapTo<User>(updatedUser);
            };
        }
    }
}
