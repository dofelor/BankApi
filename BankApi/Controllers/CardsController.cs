using BankApi.DTOs.CreateDTOs;
using BankApi.DTOs.ResponseDTOs;
using BankApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardsController : ControllerBase
    {
        private readonly ICardService _cardService;

        public CardsController(ICardService cardService)
        {
            _cardService = cardService;
        }

        [HttpPost("account/{accountId:int}")]
        public async Task<ActionResult<CardResponseDto>> CreateCard(int accountId, [FromBody] CreateCardDto dto)
        {
            var card = await _cardService.CreateCardAsync(accountId, dto);
            return CreatedAtAction(nameof(GetCardById), new { id = card.Id }, card);
            
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CardResponseDto>> GetCardById(int id)
        {
            var card = await _cardService.GetCardByIdAsync(id);
            if (card == null) return NotFound();

            return Ok(card);
        }

        [HttpGet("account/{accountId:int}")]
        public async Task<ActionResult<List<CardResponseDto>>> GetCardsByAccountId(int accountId)
        {
            var cards = await _cardService.GetCardsByAccountIdAsync(accountId);
            return Ok(cards);
        }

        [HttpPatch("{id:int}/block")]
        public async Task<ActionResult> BlockCard(int id)
        {
            var result = await _cardService.BlockCardAsync(id);
            if (!result) return NotFound();

            return NoContent();
        }

        [HttpPatch("{id:int}/unblock")]
        public async Task<IActionResult> UnblockCard(int id)
        {
            var result = await _cardService.UnblockCardAsync(id);
            if (!result) return NotFound();


            return NoContent();
        }
    }
}
