namespace TeamDesk.Api.Dtos.Lookups;

public record CategoryDto(int Id, string Name);
public record UserDto(int Id, string Name, string Role);   // jamais PasswordHash ni Email