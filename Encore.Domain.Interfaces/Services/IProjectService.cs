namespace Encore.Domain.Interfaces.Services
{
    using Encore.Domain.Entities;
    using System;

    public interface IProjectService : IEntityService<Project> 
    {
        bool SyncProjectSites(Guid projectId);

        bool SyncProjectFields(Guid projectId);

        bool SyncAllProjectSummaries();

        void SyncAllProjectSites();

        void SyncAllProjectFields();

        bool TestProjectUrl(string url);
    }
}
