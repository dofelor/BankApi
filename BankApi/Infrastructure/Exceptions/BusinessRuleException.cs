namespace BankApi.Infrastructure.Exceptions
{
    /// <summary>
    /// Выбрасывается, когда операция нарушает бизнес-правила приложения.
    /// Например: закрытие счёта с ненулевым балансом, перевод с заблокированной карты и т.д.
    /// Маппится на HTTP 422 Unprocessable Entity.
    /// </summary>
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message) { }

        public BusinessRuleException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
