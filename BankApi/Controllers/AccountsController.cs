using BankApi.DTOs;
using BankApi.DTOs.CreateDTOs;
using BankApi.DTOs.ResponseDTOs;
using BankApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IBankAccountService _accountService;

        public AccountsController(IBankAccountService service)
        {
            _accountService = service;
        }

        // Только Admin может открывать новые счета клиентам
        [Authorize(Roles = "Admin")]
        [HttpPost("client/{clientId:int}")]
        public async Task<ActionResult<BankAccountResponseDto>> CreateAccount(int clientId, [FromBody] CreateAccountDto dto)
        {
            var result = await _accountService.CreateAccountAsync(clientId, dto);
            return CreatedAtAction(nameof(GetAccountById), new { id = result.Id }, result);
        }

        // Просмотр — все авторизованные
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BankAccountResponseDto>> GetAccountById(int id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if(account == null)
            {
                return NotFound(new {message = $"Account with Id = {id} not found." });
            }
            return Ok(account);
        }

        // Просмотр счетов клиента — все авторизованные
        [HttpGet("client/{clientId:int}")]
        public async Task<ActionResult<List<BankAccountResponseDto>>> GetAccountsByClientId(int clientId)
        {
             var accounts = await _accountService.GetAccountsByClientIdAsync(clientId);
             return Ok(accounts);
        }

        // Бизнес-операция: закрытие счёта — доступно User и Admin
        // Бизнес-правила проверяются в сервисе: нулевой баланс и нет активных карт
        [HttpPatch("{id:int}/close")]
        public async Task<ActionResult> CloseAccount(int id)
        {
            var isClosed = await _accountService.CloseAccountAsync(id);
            if (!isClosed)
            {
                return NotFound($"Account with Id = {id} not found.");
            }
            return NoContent();
        }

        // Только Admin может пополнять баланс (кассовая операция)
        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/deposit")]
        public async Task<ActionResult<BankAccountResponseDto>> AddBalance(int id, [FromBody] AddBalanceDto dto)
        {
            var updated = await _accountService.AddBalanceAsync(id, dto.Amount);
            return Ok(updated);
        }
    }
}
