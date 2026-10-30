using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using TeamDesk.Api.Entities;

namespace TeamDesk.Api.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;                 // accès à appsettings
        public TokenService(IConfiguration config) => _config = config; // injection de dépendances

        public string CreateToken(User user)
        {
            var claims = new[]                                    // les "infos" contenues dans le token
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // l'Id, pour savoir QUI appelle
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role.ToString())  // utilisé par [Authorize(Roles = ...)]
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));     // la clé secrète en octets
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // algo de signature

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpiresMinutes"]!)), // UTC : pas de souci de fuseau
                SigningCredentials = creds
            };

            return new JsonWebTokenHandler().CreateToken(descriptor); // produit la chaîne "xxx.yyy.zzz"
        }
    }
}
