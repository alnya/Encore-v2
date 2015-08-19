namespace Encore.Domain.Services
{
    using Encore.Domain.Entities;
    using Encore.Domain.Interfaces.DataStore;
    using Encore.Domain.Interfaces.Services;
    using System.Threading;
        
    public class FieldService : IFieldService
    {
        private readonly IRepositoryContext context;

        private readonly IProvideFieldData fieldProvider;

        public FieldService(IRepositoryContext context, IProvideFieldData fieldProvider)
        {
            this.context = context;
            this.fieldProvider = fieldProvider;
        }

        public bool SyncFields()
        {
            var newFields = fieldProvider.GetFieldsAsync(CancellationToken.None).Result;

            if(newFields != null)
            {
                var fieldRepo = context.GetRepository<Field>();
                fieldRepo.DeleteAll();
                fieldRepo.Insert(newFields);
                return true;
            }

            return false;
        }
    }
}
