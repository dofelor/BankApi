using System;

namespace BankApi.DTOs.ResponseDTOs
{
    public class TransactionLogResponseDto
    {
        public int Id { get; set; }
        public int? FromAccountId { get; set; }
        public string FromAccountNumber { get; set; } = string.Empty;
        public int? ToAccountId { get; set; }
        public string ToAccountNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? Description { get; set; }
    }
}
