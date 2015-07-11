namespace Encore.Domain.Interfaces.DataStore
{
    using Encore.Domain.Entities;

    public interface IRepositoryContext
    {
        IRepository<T> GetRepository<T>() where T : EntityBase;
    }
}
