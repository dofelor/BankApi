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

            try
            {
                var card = await _cardService.CreateCardAsync(accountId, dto);
                return CreatedAtAction(nameof(GetCardById), new { id = card.Id }, card);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            
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
            try
            {
                var cards = await _cardService.GetCardsByAccountIdAsync(accountId);
                return Ok(cards);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPatch("{id:int}/block")]
        public async Task<ActionResult> BlockCard(int id)
        {
            try
            {
                var result = await _cardService.BlockCardAsync(id);
                if (!result) return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                BadRequest(ex.Message);
            }

            

            return NoContent();
        }

        [HttpPatch("{id:int}/unblock")]
        public async Task<IActionResult> UnblockCard(int id)
        {
            try
            {
                var result = await _cardService.UnblockCardAsync(id);
                if (!result) return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                BadRequest(ex.Message);
            }


            return NoContent();
        }
    }
}
