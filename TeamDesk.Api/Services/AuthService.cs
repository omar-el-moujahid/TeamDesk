using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TeamDesk.Api.Data;
using TeamDesk.Api.Dtos.Auth;
using TeamDesk.Api.Entities;

namespace TeamDesk.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher<User> _hasher;
        private readonly ITokenService _tokens;

        public AuthService(AppDbContext db, IPasswordHasher<User> hasher, ITokenService tokens)
        { _db = db; _hasher = hasher; _tokens = tokens; }

        public async Task<LoginResponse?> LoginAsync(LoginRequest dto)
        {
            var user = await _db.Users.AsNoTracking()             // lecture seule : pas besoin de tracking
                .FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user is null) return null;

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed) return null;

            return new LoginResponse(_tokens.CreateToken(user), user.Name, user.Role.ToString());
        }
    }
}
