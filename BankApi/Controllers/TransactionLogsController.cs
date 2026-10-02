using BankApi.DTOs.ResponseDTOs;
using BankApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionLogsController : ControllerBase
    {
        private readonly ITransactionLogService _logService;

        public TransactionLogsController(ITransactionLogService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TransactionLogResponseDto>>> GetLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var logs = await _logService.GetLogsAsync(page, pageSize);
            return Ok(logs);
        }
    }
}
