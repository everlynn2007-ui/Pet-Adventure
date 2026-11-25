using System.Security.Claims;
using GameApi.DTOs;
using GameApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameApi.Controllers;

[ApiController]
[Route("api/characters")]
[Authorize]
public class CharactersController : ControllerBase
{
    private readonly CharacterService service;

    public CharactersController(CharacterService service)
    {
        this.service = service;
    }

    private int CurrentAccountId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Token thiếu NameIdentifier"));

    [HttpGet]
    public async Task<IActionResult> GetMine()
    {
        return Ok(await service.GetMineAsync(CurrentAccountId));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCharacterRequest request)
    {
        CharacterResponse response = await service.CreateAsync(CurrentAccountId, request);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCharacterRequest request)
    {
        CharacterResponse? response = await service.UpdateAsync(CurrentAccountId, id, request);
        if (response is null)
        {
            return NotFound("Không tìm thấy nhân vật của bạn");
        }

        return Ok(response);
    }
}
