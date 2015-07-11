namespace Encore.Domain.Interfaces.Services
{
    public interface IProcessReportResults
    {
        void ProcessReportQueue();

        void RemoveOldResults(int deleteAfterDays);
    }
}
