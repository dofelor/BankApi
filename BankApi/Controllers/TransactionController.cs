using BankApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BankApi.DTOs;

namespace BankApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("transfer")]
        public async Task<ActionResult> Transfer([FromBody] TransferDto dto)
        {
            await _transactionService.TransferAsync(dto);
            return Ok(new { Message = "Transfer completed successfully" });
        }
    }
}
