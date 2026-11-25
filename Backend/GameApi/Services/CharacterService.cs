using GameApi.Data;
using GameApi.DTOs;
using GameApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameApi.Services;

public class CharacterService
{
    private readonly AppDbContext db;

    public CharacterService(AppDbContext db)
    {
        this.db = db;
    }

    public async Task<List<CharacterResponse>> GetMineAsync(int accountId)
    {
        return await db.Characters
            .Where(character => character.AccountId == accountId)
            .OrderBy(character => character.Id)
            .Select(character => new CharacterResponse(
                character.Id, character.Name, character.Class, character.Level, character.Hp, character.AccountId))
            .ToListAsync();
    }

    public async Task<CharacterResponse> CreateAsync(int accountId, CreateCharacterRequest request)
    {
        Character character = new Character
        {
            Name = request.Name.Trim(),
            Class = request.Class.Trim(),
            Level = 1,
            Hp = 100,
            CreatedAt = DateTime.UtcNow,
            AccountId = accountId
        };

        db.Characters.Add(character);
        await db.SaveChangesAsync();

        return ToResponse(character);
    }

    public async Task<CharacterResponse?> UpdateAsync(int accountId, int characterId, UpdateCharacterRequest request)
    {
        Character? character = await db.Characters
            .FirstOrDefaultAsync(item => item.Id == characterId && item.AccountId == accountId);

        if (character is null)
        {
            return null;
        }

        character.Name = request.Name.Trim();
        character.Class = request.Class.Trim();
        character.Level = request.Level ?? character.Level;
        character.Hp = request.Hp ?? character.Hp;

        await db.SaveChangesAsync();
        return ToResponse(character);
    }

    private static CharacterResponse ToResponse(Character character)
    {
        return new CharacterResponse(
            character.Id, character.Name, character.Class, character.Level, character.Hp, character.AccountId);
    }
}
