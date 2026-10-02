using BankApi.DTOs.CreateDTOs;
using BankApi.DTOs.ResponseDTOs;
using BankApi.DTOs.UpdateDTOs;
using BankApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ClientResponseDto>> CreateClient([FromBody]CreateClientDto dto)
    {
        var result = await _clientService.CreateClientAsync(dto);
        return CreatedAtAction(nameof(GetClientById), new {id = result.Id}, result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClientResponseDto>> GetClientById(int id)
    {
        var client = await _clientService.GetClientByIdAsync(id);

        if(client == null) return NotFound($"Client with ID = {id} not found.");

        return Ok(client);
    }

    [HttpGet]
    public async Task<ActionResult<List<ClientResponseDto>>> GetAllClients(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return BadRequest("Pagination parameters must be greater than 0.");
        }

        var totalCount = await _clientService.GetClientsCountAsync();
        Response.Headers.Append("X-Total-Count", totalCount.ToString());

        var clients = await _clientService.GetClientsAsync(pageNumber, pageSize);
        return Ok(clients);
    }

    [HttpGet("count")]
    public async Task<ActionResult<int>> GetClientsCount()
    {
        var count = await _clientService.GetClientsCountAsync();
        return Ok(count);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ClientResponseDto>> UpdateClient(int id, [FromBody]UpdateClientDto dto)
    {
        var updatedClient = await _clientService.UpdateClientAsync(id, dto);

        if (updatedClient == null) return NotFound($"Client with ID = {id} not found.");

        return Ok(updatedClient);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteClient(int id)
    {
        var isDeleted = await _clientService.DeleteClientAsync(id);

        if (!isDeleted) return NotFound($"Client with ID = {id} not found.");

        return NoContent();
    }
}
