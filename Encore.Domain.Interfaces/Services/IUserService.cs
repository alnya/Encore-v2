namespace Encore.Domain.Interfaces.Services
{
    using Encore.Domain.Entities;
    using System;

    public interface IUserService : IEntityService<SystemUser> 
    {
        SystemUser UpdateUser(Guid updatedByUserId, Guid userId, SystemUser user);

        SystemUser AuthenticateUser(string name, string password);

        bool DeleteUser(Guid deletedByUserId, Guid userId);

        void EnsureAdminUser();
    }
}
