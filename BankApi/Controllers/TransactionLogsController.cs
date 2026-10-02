using BankApi.DTOs.ResponseDTOs;
using BankApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{
    [Authorize(Roles = "Admin")]
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
            var totalCount = await _logService.GetLogsCountAsync();
            Response.Headers.Append("X-Total-Count", totalCount.ToString());

            var logs = await _logService.GetLogsAsync(page, pageSize);
            return Ok(logs);
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> GetLogsCount()
        {
            var count = await _logService.GetLogsCountAsync();
            return Ok(count);
        }
    }
}
