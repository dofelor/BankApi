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
    public class PhonesController : ControllerBase
    {
        private readonly IPhoneService _phoneService;

        public PhonesController(IPhoneService phoneService)
        {
            _phoneService = phoneService;
        }

        [HttpPost("client/{clientId:int}")]
        public async Task<ActionResult<PhoneResponseDto>> CreatePhone(int clientId, CreatePhoneDto dto)
        {
            var phone = await _phoneService.CreatePhoneAsync(clientId, dto);
            return CreatedAtAction(nameof(GetPhoneById), new {id = phone.Id}, phone);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PhoneResponseDto>> GetPhoneById(int id)
        {
            var phone = await _phoneService.GetPhoneByIdAsync(id);
            if (phone == null) return NotFound();

            return Ok(phone);
        }

        [HttpGet("client/{clientId:int}")]
        public async Task<ActionResult<List<PhoneResponseDto>>> GetPhonesByClientId(int clientId)
        {
            var phones = await _phoneService.GetPhonesByClientIdAsync(clientId);
            return Ok(phones);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeletePhone(int id)
        {
            var result = await _phoneService.DeletePhoneAsync(id);
            if (!result) return NotFound();

            return NoContent();
        }
    }
}
