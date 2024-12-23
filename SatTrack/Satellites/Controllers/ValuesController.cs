using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SatTrack.DAL;
using SatTrack.Satellites.DTO;
using SatTrack.Services;
using SatTrack.Services.Interfaces;

namespace SatTrack.Satellites.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        private readonly IUserService _userService;


        public ValuesController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet("users")]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var users = await _userService.GetAllUsersAsync();   
            return Ok(users);

        }
    }
}
