using auth_jwt_rbac.Database;
using auth_jwt_rbac.Dtos;
using auth_jwt_rbac.Entities;
using auth_jwt_rbac.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
                new Claim(ClaimTypes.NameIdentifier, req.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: config.GetValue<string>("AppSettings:Issuer"),
                audience: config.GetValue<string>("AppSetting:Audience"),
                claims: claims, expires: DateTime.UtcNow.AddDays(7));

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public async Task<string?> LoginAsync(UserDto req)
        {
            try
            {
                var user = await context.users.FirstOrDefaultAsync(u => u.Username == req.Username);

                if (user == null) { return null; }

                if (await context.users.AnyAsync(u => u.Username != req.Username || new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, req.PasswordHash) == PasswordVerificationResult.Failed))
                {
                    return "Incorrect username, password or the account doesn't exist!";
                }

                string token = CreateToken(user);

                return token;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
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
