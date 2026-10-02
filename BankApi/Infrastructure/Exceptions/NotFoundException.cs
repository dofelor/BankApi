namespace BankApi.Infrastructure.Exceptions
{
    /// <summary>
    /// Выбрасывается, когда запрошенный ресурс не найден.
    /// Маппится на HTTP 404 Not Found.
    /// Используй вместо KeyNotFoundException для более явной семантики.
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string resourceName, object key)
            : base($"{resourceName} with ID = {key} was not found.") { }

        public NotFoundException(string message) : base(message) { }
    }
}
