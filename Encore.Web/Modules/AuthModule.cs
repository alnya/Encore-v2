namespace Encore.Web.Modules
{
    using AutoMapper;
    using Encore.Domain.Interfaces.Services;
    using Encore.Web.Models;
    using Nancy;
    using Nancy.ModelBinding;
    using Extensions;
    using Encore.Domain.Entities;

    public class AuthModule : BaseModule
    {
        public AuthModule(IUserService userService, IMappingEngine mappingEngine)
            : base("data", mappingEngine)
        {
            Post["/login"] = args =>
            {
                var model = this.BindAndValidate<Models.Login>();
                if (!ModelValidationResult.IsValid)
                {
                    return RespondWithValidationFailure(ModelValidationResult);
                }

                var user = userService.AuthenticateUser(model.Name, model.Password);

                if (user == null)
                {
                    return HttpStatusCode.Unauthorized;
                }

                Context.SetupSession(user);

                return MapTo<User>(user);
            };
        }
    }
}
