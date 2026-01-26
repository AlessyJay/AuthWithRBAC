using auth_jwt_rbac.Database;
using auth_jwt_rbac.Dtos;
using auth_jwt_rbac.Entities;
using auth_jwt_rbac.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace auth_jwt_rbac.Services
{
    public class AuthService(AppDbContext context, IConfiguration config) : IAuthService
    {
        private string CreateToken(User req)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, req.Username),
                new Claim(ClaimTypes.NameIdentifier, req.Id.ToString()),
                new Claim(ClaimTypes.Role, req.Roles.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: config.GetValue<string>("AppSettings:Issuer")!,
                audience: config.GetValue<string>("AppSettings:Audience")!,
                signingCredentials: creds,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7));

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        private string GenerateRefreshToken()
        {
            var random = new Byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(random);

            return Convert.ToBase64String(random);
        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(User req)
        {
            var refreshToken = GenerateRefreshToken();
            req.RefreshToken = refreshToken;
            req.RefreshExpiresAt = DateTimeOffset.UtcNow.AddDays(30);

            await context.SaveChangesAsync();

            return refreshToken;
        }

        private async Task<User> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await context.users.FindAsync(userId);

            if (user is null || user.RefreshToken != refreshToken || user.RefreshExpiresAt <= DateTimeOffset.UtcNow)
            {
                return null!;
            }

            return user;
        }

        public async Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto req)
        {
            var user = await ValidateRefreshTokenAsync(req.UserID, req.RefreshToken);

            if (user is null)
            {
                return null!;
            }

            return await CreateTokenResponse(user);
        }

        public async Task<TokenResponseDto?> LoginAsync(UserDto req)
        {
            try
            {
                var user = await context.users.FirstOrDefaultAsync(u => u.Username == req.Username);

                if (user == null) { return null; }

                if (await context.users.AnyAsync(u => u.Username != req.Username || new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, req.PasswordHash) == PasswordVerificationResult.Failed))
                {
                    return null;
                }

                string token = CreateToken(user);

                return await CreateTokenResponse(user);
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        private async Task<TokenResponseDto> CreateTokenResponse(User user)
        {
            return new TokenResponseDto
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user),
            };
        }

        public async Task<User?> RegisterAsync(UserDto req)
        {
            if (await context.users.AnyAsync(u => u.Username == req.Username))
            {
                return null;
            }

            var user = new User();

            var hashedPassword = new PasswordHasher<User>().HashPassword(user, req.PasswordHash);

            user.Username = req.Username;
            user.PasswordHash = hashedPassword;

            context.users.Add(user);

            await context.SaveChangesAsync();

            return user;
        }
    }
}
