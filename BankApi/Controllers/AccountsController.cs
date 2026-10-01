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
            try
            {
                var result = await _accountService.CreateAccountAsync(clientId, dto);
                return CreatedAtAction(nameof(GetAccountById), new { id = result.Id }, result);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new {message = ex.Message});
            }
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
            try
            {
                var accounts = await _accountService.GetAccountsByClientIdAsync(clientId);
                return Ok(accounts);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new {message = ex.Message});
            }
            
        }

        [HttpPatch("{id:int}/close")]
        public async Task<ActionResult> CloseAccount(int id)
        {
            try
            {
                var isClosed = await _accountService.CloseAccountAsync(id);
                if (!isClosed)
                {
                    return NotFound($"Account with Id = {id} not found.");
                }
                return NoContent();
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }
    }
}
