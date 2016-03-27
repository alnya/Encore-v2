namespace Encore.Domain.Services
{
    using Encore.Domain.Entities;
    using Encore.Domain.Interfaces.DataStore;
    using Encore.Domain.Interfaces.Services;
    using Encore.Domain.Services.Exceptions;
    using System;
    using System.Linq;
    using Extensions;
    using Encore.Domain.Services.Search;

    public class UserService : EntityService<SystemUser>, IUserService
    {
        private readonly IRepositoryContext context;

        public UserService(IRepositoryContext context)
            : base(context)
        {
            this.context = context;
        }

        public override SystemUser Add(SystemUser user)
        {
            if (String.IsNullOrWhiteSpace(user.Name) || String.IsNullOrWhiteSpace(user.Password))
            {
                throw new DomainException("Username and Password must be supplied");
            }            

            var userRepo = context.GetRepository<SystemUser>();
            var foundUser = userRepo.GetWhere(x => x.Name.ToLowerInvariant() == user.Name.ToLowerInvariant()).FirstOrDefault();

            if (foundUser != null)
            {
                throw new DomainException("Name already exists.");
            }
            
            user.Id = Guid.Empty;
            user.Salt = BCrypt.Net.BCrypt.GenerateSalt();
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, user.Salt);

            userRepo.Save(user);

            return user;
        }

        public SystemUser UpdateUser(Guid updatedByUserId, Guid userId, SystemUser user)
        {
            if (String.IsNullOrWhiteSpace(user.Name) || String.IsNullOrWhiteSpace(user.Password))
            {
                throw new DomainException("Username and Password must be supplied");
            }    

            var userRepo = context.GetRepository<SystemUser>();
            var updateUser = userRepo.Get(userId);
            updateUser.ValidateNotNull<SystemUser>();

            if (userRepo.Exists(x => x.Name.ToLowerInvariant() == user.Name.ToLowerInvariant() && x.Id != userId))
            {
                throw new DomainException("Name already exists.");
            }

            bool selfEdit = updatedByUserId == userId;

            updateUser.Name = user.Name;
            updateUser.Email = user.Email;
            updateUser.ProjectTokens = user.ProjectTokens;

            if (!String.IsNullOrEmpty(user.Password) && user.Password != updateUser.Password)
            {
                updateUser.Salt = BCrypt.Net.BCrypt.GenerateSalt();
                updateUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, updateUser.Salt);
            }
            
            if (!selfEdit)
            {
                if (updateUser.UserRole == UserRole.Admin && user.UserRole != UserRole.Admin && userRepo.Count(x => x.UserRole, UserRole.Admin) == 1)
                {
                    throw new DomainException("System requires at least 1 admin user.");
                }

                updateUser.UserRole = user.UserRole;
            }

            userRepo.Save(updateUser);
            return updateUser;
        }

        public virtual bool DeleteUser(Guid deletedByUserId, Guid userId)
        {
            if (userId == deletedByUserId)
            {
                throw new DomainException("User cannot delete themselves.");
            }
            
            var userRepo = context.GetRepository<SystemUser>();

            if (userRepo.Count(x => x.UserRole, UserRole.Admin) == 1 && userRepo.Exists(x => x.Id == userId && x.UserRole == UserRole.Admin))
            {
                throw new DomainException("System requires at least 1 admin user.");
            }

            userRepo.DeleteWhere(x => x.Id == userId);

            return true;
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

        public void EnsureAdminUser()
        {
            var userRepo = context.GetRepository<SystemUser>();

            if(!userRepo.Exists(x => x.UserRole == UserRole.Admin))
            {
                var defaultAdminUser = new SystemUser
                {
                    Name = "Administrator",
                    Password = "T3rr1bl3Tr0ubl3",
                    UserRole = UserRole.Admin
                };

                Add(defaultAdminUser);
            }
        }
    }
}
