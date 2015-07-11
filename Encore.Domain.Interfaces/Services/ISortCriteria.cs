namespace Encore.Domain.Interfaces.Services
{
    public interface ISortCriteria
    {
        bool SortDescending { get; }

        string SortBy { get; }
    }
}
