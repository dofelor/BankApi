using BankApi.DTOs;
using BankApi.DTOs.CreateDTOs;
using BankApi.DTOs.ResponseDTOs;
using BankApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IBankAccountService _accountService;

        public AccountsController(IBankAccountService service)
        {
            _accountService = service;
        }

        [HttpPost("client/{clientId:int}")]
        public async Task<ActionResult<BankAccountResponseDto>> CreateAccount(int clientId, [FromBody] CreateAccountDto dto)
        {

            var result = await _accountService.CreateAccountAsync(clientId, dto);
            return CreatedAtAction(nameof(GetAccountById), new { id = result.Id }, result);
        }

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

        [HttpGet("client/{clientId:int}")]
        public async Task<ActionResult<List<BankAccountResponseDto>>> GetAccountsByClientId(int clientId)
        {
             var accounts = await _accountService.GetAccountsByClientIdAsync(clientId);
             return Ok(accounts);
            
        }

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

        [HttpPost("{id:int}/deposit")]
        public async Task<ActionResult<BankAccountResponseDto>> AddBalance(int id, [FromBody] AddBalanceDto dto)
        {
            var updated = await _accountService.AddBalanceAsync(id, dto.Amount);
            return Ok(updated);
        }
    }
}
