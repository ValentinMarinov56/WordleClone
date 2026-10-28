
using System.Diagnostics.Contracts;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WordleClone.API.Models;
using WordleClone.API.Services;


[ApiController]
[Route("api/[controller]")]

public sealed class WordlController : Controller
{
    private readonly IWordlService _wordlService;
    public WordlController(IWordlService wordlService)
    {
        _wordlService = wordlService;
    }
    [HttpGet] // IActionResult allows us to use Ok() and NotFound()
    public async Task<IActionResult> GetWordls()
    {
        var wordls = await _wordlService.GetAllWordl();
        return Ok(wordls);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetWordl(int id)
    {
        var wordl = await _wordlService.GetWordl(id);
        if (wordl == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(wordl);
        }
    }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateWordl([FromBody]Wordl? wordl)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }
        bool created = await _wordlService.AddWordl(wordl);
        return Ok(created);
    }
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> RemoveWordl(int id)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }
        bool deleted = await _wordlService.RemoveWordl(id);
        return Ok(deleted);
    }
}