namespace Encore.Domain.Interfaces.Services
{
    using Encore.Domain.Entities;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IProvideFieldData
    {
        Task<IEnumerable<Field>> GetFieldsAsync(CancellationToken token);
    }
}
