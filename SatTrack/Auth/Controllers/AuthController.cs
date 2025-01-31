using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SatTrack.Auth.DTO;
using SatTrack.DAL;
using SatTrack.DTO;
using SatTrack.Services;
using SatTrack.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SatTrack.Auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AuthController : ControllerBase
    {


        private readonly IUserService _userService;
        private readonly PasswordService _passwordService;
        private readonly string unauthMessage = "Invalid credentials or permission denied.";

        public AuthController(IUserService userService, PasswordService passwordService)
        {
            _passwordService = passwordService;
            _userService = userService;
        }


        [HttpPost("login")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK, Type = typeof(AuthResultDTO))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(MessageDTO))]
        public async Task<IActionResult> Login([FromBody] AuthDTO authDTO)
        {
            var user = await _userService.GetUserByNameAsync(authDTO.username);

            if (user == null)
            {
                return Unauthorized(new MessageDTO { Detail = unauthMessage, Message = "Unauthorized access.",Timestamp = DateTime.Now });
            }

            if (_passwordService.VerifyPassword(user.Password, authDTO.password))
            {
                var token = GenerateJwtToken(user);
                return Ok(new AuthResultDTO
                {
                    Token = token,
                    ValidThrough = DateTime.Now.AddMinutes(30),
                    Roles = user.Roles.Select(r => r.RoleName).ToList()
                });
            }
            else
            {
                return Unauthorized(new MessageDTO { Detail = unauthMessage, Message = "Unauthorized access.",Timestamp = DateTime.Now });
            }
        }

        private string GenerateJwtToken(User user)
        {
            var appSettings = System.Configuration.ConfigurationManager.AppSettings;
            var claims = new List<Claim>{
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (user.Roles != null)
            {
                claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.RoleName)));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Util.appSettingsConfiguration.GetSetting("SecretKey")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "yourdomain.com",
                audience: "yourdomain.com",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
