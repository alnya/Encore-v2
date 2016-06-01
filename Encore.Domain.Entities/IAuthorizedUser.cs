namespace Encore.Domain.Entities
{
    using System;

    public interface IAuthorizedUser
    {
        Guid Id { get; }

        string Name { get; }

        string Email { get; }

        UserRole UserRole { get; }
    }
}
