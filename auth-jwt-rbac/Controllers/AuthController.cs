using auth_jwt_rbac.Dtos;
using auth_jwt_rbac.Entities;
using auth_jwt_rbac.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace auth_jwt_rbac.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] UserDto req)
        {
            var user = await authService.RegisterAsync(req);

            if (user is null) return BadRequest("Username already exists!");

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] UserDto req)
        {
            var result = await authService.LoginAsync(req);

            if (result is null) return BadRequest("Invalid username or password!");

            return Ok(result);
        }

        [HttpPost("refresh-tokens")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto req)
        {
            var result = await authService.RefreshTokensAsync(req);

            if (result is null || result.AccessToken is null || result.RefreshToken is null)
            {
                return Unauthorized(result);
            }

            return result;
        }

        [Authorize]
        [HttpGet("authenticated-endpoint")]
        public IActionResult AuthenticatedEnpointOnly()
        {
            return Ok("You are authenticated!");
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("admin-endpoint")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("You are authenticated as an admin!");
        }
    }
}
