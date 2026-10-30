using TeamDesk.Api.Entities;

namespace TeamDesk.Api.Services
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}