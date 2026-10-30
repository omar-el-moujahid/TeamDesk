using TeamDesk.Api.Dtos.Auth;

namespace TeamDesk.Api.Services
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest dto);
    }
}