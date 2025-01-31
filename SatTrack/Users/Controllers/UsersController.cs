using Microsoft.AspNetCore.Mvc;
using SatTrack.DTO;
using SatTrack.Services.Interfaces;
using SatTrack.Services;
using Microsoft.AspNetCore.Authorization;
using SatTrack.Users.DTO;
using System.ComponentModel.DataAnnotations;

namespace SatTrack.Users.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class UsersController(IUserService userService, IRoleService roleService) : ControllerBase
    {


        private readonly IUserService _userService = userService;
        private readonly IRoleService _roleService = roleService;

        [HttpPost("create")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created, Type = typeof(UserDTO))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(MessageDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(MessageDTO))]
        [Authorize("ADMIN")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO createUserDTO)
        {
            var user = await _userService.GetUserByNameAsync(createUserDTO.Username);

            if (user != null)
            {
                return Conflict(new MessageDTO { Detail = "User already registered.", Message = $"A user with the username {createUserDTO.Username} already exists.",Timestamp=DateTime.Now });
            }

            var existingRoles = await _roleService.GetExistingRoles();
            var rolesDTO = existingRoles.Select(x => new RoleDTO
            {
                RoleName = x.RoleName
            });

            if (createUserDTO.Roles.Except(rolesDTO, new RoleDTO.RoleDTOEqualityComparer()).Any())
            {
                return BadRequest(new MessageDTO { Message = "Supplied roles are invalid.", Detail = "One of the supplied roles doesn't exist." });
            }

            user = await _userService.CreateUser(createUserDTO);

            return Created();

        }

        [HttpPatch("update")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK, Type = typeof(UserDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(MessageDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(MessageDTO))]
        [Authorize("ADMIN")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDTO updateUserDTO)
        {
            var user = await _userService.GetUserByNameAsync(updateUserDTO.Username);
            if (user == null)
            {
                return NotFound(new MessageDTO { Message = "User not found.", Detail = $"A user with the username {updateUserDTO.Username} was not found.", Timestamp = DateTime.Now });
            }

            var existingRoles = await _roleService.GetExistingRoles();
            var rolesDTO = existingRoles.Select(x => new RoleDTO
            {
                RoleName = x.RoleName
            });

            if (updateUserDTO.Roles.Except(rolesDTO, new RoleDTO.RoleDTOEqualityComparer()).Any())
            {
                return BadRequest(new MessageDTO { Message = "Supplied roles are invalid.", Detail = "One of the supplied roles doesn't exist.", Timestamp = DateTime.Now });
            }

            user = await _userService.UpdateUser(user, updateUserDTO);
            var userDTO = new UserDTO { Active = user.Active, Id = user.UserId, Roles = updateUserDTO.Roles, Username = user.Username };
            return Ok(userDTO);

        }
        [HttpGet("users")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK, Type = typeof(PagedUserDTO))]
        [Authorize("ADMIN")]
        public async Task<IActionResult> GetUsers(
            [FromQuery,Range(1,int.MaxValue,ErrorMessage ="Page number must be greater than 0")] int pageNumber = 1,
            [FromQuery,Range(1,100,ErrorMessage ="Page size must be between 1 and 100")] int pageSize = 10)
        {
            var totalItems = await _userService.GetTotalCount();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var users = (await _userService.GetUserPage(pageNumber, pageSize)).ToList();

            return Ok(new PagedUserDTO
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalItems = totalItems,
                Users = users
            });
        }
    }
}