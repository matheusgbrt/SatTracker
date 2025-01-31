using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SatTrack.Contracts.Messages;
using SatTrack.DTO;
using SatTrack.Satellites.DTO;
using SatTrack.Services;
using SatTrack.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SatTrack.Satellites.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SatGroupController : ControllerBase
    {

        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly ISatGroupService _groupService;
        public SatGroupController(ISendEndpointProvider sendEndpointProvider,ISatGroupService service)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _groupService = service;
        }

        [HttpGet("groups")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GroupDTO>))]

        public async Task<IActionResult> GetGroups()
        {
            var groupDTOs = await _groupService.GetAllGroupsDTO();
            return Ok(groupDTOs);
        }

        [HttpPost("updategroup")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(MessageDTO))]
        public async Task<IActionResult> UpdateGroup([FromBody, Required] GroupDTO groupDTO)
        {

            if (!_groupService.CheckGroupExists(groupDTO.GroupName).Result)
            {
                return NotFound("Group query not found.");
            }

            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:GroupUpdate"));
            await endpoint.Send(new SatGroupMessage(groupDTO.GroupName));

            return Accepted(new MessageDTO
            {
                Message = $"Group {groupDTO.GroupName} registered for update.",
                Detail= "",
                Timestamp = DateTime.UtcNow
            });
        }

        [HttpPost("updateallgroup")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(MessageDTO))]

        public async Task<IActionResult> UpdateAllGroups()
        {

            var groupDTOs = await _groupService.GetAllGroupsDTO();


            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:GroupUpdate"));
            foreach (SatGroupDTO dto in groupDTOs)
            {
            await endpoint.Send(new SatGroupMessage(dto.groupQuery));
            }

            return Accepted(new MessageDTO
            {
                Message = $"{groupDTOs.Count()} Groups registered for update.",
                Detail = "",
                Timestamp = DateTime.UtcNow
            });
        }


        public record GroupDTO(string GroupName);

    }
}
