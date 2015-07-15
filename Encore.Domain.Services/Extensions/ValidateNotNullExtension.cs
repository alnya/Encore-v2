namespace Encore.Domain.Services.Extensions
{
    using Encore.Domain.Services.Exceptions;
    
    public static class ValidateNotNullExtension
    {
        public static T ValidateNotNull<T>(this T item, string message = null) where T : class
        {
            if (item == null)
            {
                throw new DomainException(message ?? typeof(T).Name + " not found");
            }

            return item;
        }
    }
}
