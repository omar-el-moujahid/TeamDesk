using Microsoft.EntityFrameworkCore;
using TeamDesk.Api.Data;
using TeamDesk.Api.Dtos.Lookups;

namespace TeamDesk.Api.Services;

public class LookupService : ILookupService
{
    private readonly AppDbContext _db;
    public LookupService(AppDbContext db) => _db = db;

    public async Task<List<CategoryDto>> GetCategoriesAsync() =>
        await _db.Categories
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto(c.Id, c.Name))      // projection : SELECT Id, Name seulement
            .ToListAsync();

    public async Task<List<UserDto>> GetUsersAsync() =>
        await _db.Users
            .OrderBy(u => u.Name)
            .Select(u => new UserDto(u.Id, u.Name, u.Role.ToString()))
            .ToListAsync();
}