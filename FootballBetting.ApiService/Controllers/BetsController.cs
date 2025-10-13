using Microsoft.AspNetCore.Mvc;
using FootballBetting.Application.Interfaces;
using FootballBetting.Application.DTOs;

namespace FootballBetting.ApiService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BetsController : ControllerBase
{
    private readonly IBettingService _bettingService;

    public BetsController(IBettingService bettingService)
    {
        _bettingService = bettingService;
    }

    [HttpPost]
    public async Task<ActionResult<BetDto>> CreateBet([FromBody] CreateBetDto createBetDto)
    {
        // TODO: Get userId from JWT token
        int userId = 1; // Placeholder
        
        var bet = await _bettingService.CreateBetAsync(userId, createBetDto);
        return CreatedAtAction(nameof(GetBet), new { id = bet.Id }, bet);
    }

    [HttpPost("{id}/place")]
    public async Task<ActionResult> PlaceBet(int id)
    {
        var success = await _bettingService.PlaceBetAsync(id);
        if (!success)
            return BadRequest(new { message = "Failed to place bet" });

        return Ok(new { message = "Bet placed successfully" });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BetDto>> GetBet(int id)
    {
        var bet = await _bettingService.GetBetByIdAsync(id);
        if (bet == null)
            return NotFound();

        return Ok(bet);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<BetDto>>> GetUserBets(int userId)
    {
        var bets = await _bettingService.GetUserBetsAsync(userId);
        return Ok(bets);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> CancelBet(int id)
    {
        var success = await _bettingService.CancelBetAsync(id);
        if (!success)
            return BadRequest(new { message = "Failed to cancel bet" });

        return Ok(new { message = "Bet cancelled successfully" });
    }
}