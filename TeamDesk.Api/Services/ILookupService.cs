using TeamDesk.Api.Dtos.Lookups;

namespace TeamDesk.Api.Services;

public interface ILookupService
{
    Task<List<CategoryDto>> GetCategoriesAsync();
    Task<List<UserDto>> GetUsersAsync();
}