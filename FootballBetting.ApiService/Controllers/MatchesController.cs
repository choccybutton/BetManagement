using Microsoft.AspNetCore.Mvc;
using FootballBetting.Application.Interfaces;
using FootballBetting.Application.DTOs;

namespace FootballBetting.ApiService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchesController : ControllerBase
{
    private readonly IMatchService _matchService;

    public MatchesController(IMatchService matchService)
    {
        _matchService = matchService;
    }

    [HttpGet("upcoming")]
    public async Task<ActionResult<IEnumerable<MatchDto>>> GetUpcomingMatches([FromQuery] int hoursAhead = 48)
    {
        var matches = await _matchService.GetUpcomingMatchesAsync(hoursAhead);
        return Ok(matches);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MatchDto>> GetMatch(int id)
    {
        var match = await _matchService.GetMatchByIdAsync(id);
        if (match == null)
            return NotFound();

        return Ok(match);
    }

    [HttpGet("date-range")]
    public async Task<ActionResult<IEnumerable<MatchDto>>> GetMatchesByDateRange(
        [FromQuery] DateTime from, 
        [FromQuery] DateTime to)
    {
        var matches = await _matchService.GetMatchesByDateRangeAsync(from, to);
        return Ok(matches);
    }

    [HttpPost("trigger-scraping")]
    public async Task<ActionResult> TriggerScraping()
    {
        await _matchService.TriggerScrapingAsync();
        return Ok(new { message = "Scraping triggered successfully" });
    }
}