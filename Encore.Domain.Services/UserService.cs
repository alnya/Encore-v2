namespace Encore.Domain.Services
{
    using Encore.Domain.Entities;
    using Encore.Domain.Interfaces.DataStore;
    using Encore.Domain.Interfaces.Services;
    using Encore.Domain.Services.Exceptions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Extensions;

    public class UserService : IUserService
    {
        private readonly IRepositoryContext context;

        public UserService(IRepositoryContext context)
        {
            this.context = context;
        }

        public SystemUser CreateUser(string name, string password, string email)
        {
            var userRepo = context.GetRepository<SystemUser>();

            var foundUser = userRepo.GetWhere(x => x.Name.ToLowerInvariant() == name.ToLowerInvariant()).FirstOrDefault();

            if (foundUser != null)
            {
                throw new DomainException("Name already exists.");
            }

            var newUser = new SystemUser
            {
                Name = name,
                Email = email,
                Salt = BCrypt.Net.BCrypt.GenerateSalt()
            };

            newUser.Password = BCrypt.Net.BCrypt.HashPassword(password, newUser.Salt);
            userRepo.Save(newUser);

            return newUser;
        }

        public SystemUser UpdateUser(Guid userId, string name, string password, string email)
        {
            var userRepo = context.GetRepository<SystemUser>();
            var systemUser = userRepo.Get(userId);
            systemUser.ValidateNotNull<SystemUser>();

            if(userRepo.Exists(x => x.Name.ToLowerInvariant() == name.ToLowerInvariant() && x.Id != userId))
            {
                throw new DomainException("Name already exists.");
            }

            systemUser.Name = name;
            systemUser.Email = email;

            if (!String.IsNullOrEmpty(password) && password != systemUser.Password)
            {
                systemUser.Password = BCrypt.Net.BCrypt.HashPassword(password, systemUser.Salt);
            }

            userRepo.Save(systemUser);
            return systemUser;
        }

        public SystemUser GetUser(Guid userId)
        {
            var userRepo = context.GetRepository<SystemUser>();
            return userRepo.Get(userId);
        }

        public SystemUser AuthenticateUser(string name, string password)
        {
            var userRepo = context.GetRepository<SystemUser>();

            var foundUser = userRepo.GetWhere(x => x.Name.ToLowerInvariant() == name.ToLowerInvariant()).FirstOrDefault();

            if(foundUser != null)
            {
                if(foundUser.Password == BCrypt.Net.BCrypt.HashPassword(password, foundUser.Salt))
                {
                    return foundUser;
                }
            }

            return null;
        }
    }
}
