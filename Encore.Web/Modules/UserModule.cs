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

    public class UserModule : SecureModule
    {
        public UserModule(IUserService userService, IMappingEngine mappingEngine)
            : base("data/users", mappingEngine, UserRole.Admin)
        {
            Get["/"] = args =>
            {
                var entities = userService.Search(
                    Context.RequestedPage(),
                    null,
                    Context.SortCriteria<User>(),
                    Context.SearchTerms<User>());

                return MapToResultList<SystemUser, Encore.Web.Models.User>(entities);
            };

            Get["/{id}"] = args =>
            {
                var entity = userService.Get(new Guid(args.id));

                if (entity == null)
                {
                    return Negotiate.WithStatusCode(HttpStatusCode.NotFound);
                }

                return MapTo<Encore.Web.Models.User>(entity);
            };

            Put["/{id}"] = args =>
            {
                var model = this.BindAndValidate<Models.User>();

                if (!ModelValidationResult.IsValid)
                {
                    return RespondWithValidationFailure(ModelValidationResult);
                }

                var authorizedUser = Context.GetAuthorizedUser();
                
                var updateEntity = MapTo<SystemUser>(model);
                var updateId = new Guid(args.id);
                var updatedUser = userService.UpdateUser(authorizedUser.Id, updateId, updateEntity);

                if (authorizedUser.Id == updateId)
                {
                    Context.SetupSession(updatedUser);
                }

                return MapTo<User>(updatedUser);
            };

            Post["/"] = args =>
            {
                var model = this.BindAndValidate<Models.User>();

                if (!ModelValidationResult.IsValid)
                {
                    return RespondWithValidationFailure(ModelValidationResult);
                }

                var addEntity = MapTo<SystemUser>(model);
                var user = userService.Add(addEntity);

                if (user == null)
                {
                    return HttpStatusCode.Unauthorized;
                }

                return MapTo<User>(user);
            };
                        
            Delete["/{id}"] = args =>
            {               
                var authorizedUser = Context.GetAuthorizedUser();
                var deleteId = new Guid(args.id);

                return userService.DeleteUser(authorizedUser.Id, deleteId);
            };    
        }
    }
}
