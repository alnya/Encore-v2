namespace Encore.Domain.Interfaces.Services
{
    public interface IRequestedPage
    {
        int Page { get; }

        int PageSize { get; }
    }
}
