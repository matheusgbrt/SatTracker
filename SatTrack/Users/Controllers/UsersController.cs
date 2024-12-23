using Microsoft.AspNetCore.Mvc;
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


        [HttpPost("create")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created, Type = typeof(UserDTO))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ErrorMessageDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorMessageDTO))]
        [Authorize("ADMIN")]
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

            if (createUserDTO.Roles.Except(rolesDTO, new RoleDTO.RoleDTOEqualityComparer()).Any())
            {
                return BadRequest(new ErrorMessageDTO { Message = "Supplied roles are invalid.", Detail = "One of the supplied roles doesn't exist." });
            }

            user = await _userService.CreateUser(createUserDTO);

            return Created();

        }

        [HttpPatch("update")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK, Type = typeof(UserDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorMessageDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorMessageDTO))]
        [Authorize("ADMIN")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDTO updateUserDTO)
        {
            var user = await _userService.GetUserByNameAsync(updateUserDTO.Username);
            if (user == null)
            {
                return NotFound(new ErrorMessageDTO { Message = "User not found.", Detail = $"A user with the username {updateUserDTO.Username} was not found." });
            }

            var existingRoles = await _roleService.GetExistingRoles();
            var rolesDTO = existingRoles.Select(x => new RoleDTO
            {
                RoleName = x.RoleName
            });

            if (updateUserDTO.Roles.Except(rolesDTO, new RoleDTO.RoleDTOEqualityComparer()).Any())
            {
                return BadRequest(new ErrorMessageDTO { Message = "Supplied roles are invalid.", Detail = "One of the supplied roles doesn't exist." });
            }

            user = await _userService.UpdateUser(user, updateUserDTO);
            var userDTO = new UserDTO { Active = user.Active, Id = user.UserId, Roles = updateUserDTO.Roles, Username = user.Username };
            return Ok(userDTO);

        }
        [HttpGet("allusers")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK, Type = typeof(UserDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorMessageDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorMessageDTO))]
        [Authorize("ADMIN")]
        public async Task<IActionResult> GetAllUsers([FromBody] UpdateUserDTO updateUserDTO)
        {
            var user = await _userService.GetUserByNameAsync(updateUserDTO.Username);
            if (user == null)
            {
                return NotFound(new ErrorMessageDTO { Message = "User not found.", Detail = $"A user with the username {updateUserDTO.Username} was not found." });
            }

            var existingRoles = await _roleService.GetExistingRoles();
            var rolesDTO = existingRoles.Select(x => new RoleDTO
            {
                RoleName = x.RoleName
            });

            if (updateUserDTO.Roles.Except(rolesDTO, new RoleDTO.RoleDTOEqualityComparer()).Any())
            {
                return BadRequest(new ErrorMessageDTO { Message = "Supplied roles are invalid.", Detail = "One of the supplied roles doesn't exist." });
            }

            user = await _userService.UpdateUser(user, updateUserDTO);
            var userDTO = new UserDTO { Active = user.Active, Id = user.UserId, Roles = updateUserDTO.Roles, Username = user.Username };
            return Ok(userDTO);

        }
    }
}