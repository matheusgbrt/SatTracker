using Microsoft.AspNetCore.Mvc;
using SatTrack.Auth.DTO;
using SatTrack.DTO;
using SatTrack.Services.Interfaces;
using SatTrack.Services;
using Microsoft.AspNetCore.Authorization;
using SatTrack.Users.DTO;

namespace SatTrack.Users.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class UsersController : ControllerBase
    {


        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly PasswordService _passwordService;

        public UsersController(IUserService userService, PasswordService passwordService, IRoleService roleService)
        {
            _passwordService = passwordService;
            _userService = userService;
            _roleService = roleService;
        }


        [HttpPost("createuser")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created, Type = typeof(UserDTO))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ErrorMessageDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorMessageDTO))]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO createUserDTO)
        {
            var user = await _userService.GetUserByNameAsync(createUserDTO.Username);

            if (user != null)
            {
                return Conflict(new ErrorMessageDTO { Detail = "User already registered.", Message = $"A user with the username {createUserDTO.Username} already exists." });
            }

            var existingRoles = await _roleService.GetExistingRoles();
            var rolesDTO = existingRoles.Select(x => new RoleDTO
            {
                RoleName = x.RoleName
            });

            if (rolesDTO.Except(createUserDTO.Roles, new RoleDTO.RoleDTOEqualityComparer()).Any())
            {
                return BadRequest(new ErrorMessageDTO { Message = "Supplied roles are invalid.", Detail = "One of the supplied roles doesn't exist." });
            }

            user = await _userService.CreateUser(createUserDTO);

            return Created();

        }
    }
}