using MassTransit;
using Newtonsoft.Json;
using SatTrack.Contracts.Messages;
using SatTrack.Satellites.DTO.SatTrack.DTO;
using SatTrack.Services;
using SatTrack.Services.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;
using static SatTrack.Satellites.Controllers.SatGroupController;

namespace SatTrack.Contracts.Consumers
{
    public class GroupMessageConsumer(ISendEndpointProvider sendEndpointProvider, ISatGroupService groupService,CelestrakClientService celestrakClientService) : IConsumer<SatGroupMessage>
    {
        private ISatGroupService _groupService = groupService;
        private CelestrakClientService _celestrakService = celestrakClientService;
        private readonly ISendEndpointProvider _sendEndpointProvider = sendEndpointProvider;

        public async Task Consume(ConsumeContext<SatGroupMessage> context)
        {
            var message = context.Message;
            var groupName = message.groupName;
            Console.WriteLine($"Received message: groupName ={message.groupName}");

            var groupID = await _groupService.GetGroupID(groupName);

            if (groupID.HasValue)
            {
                var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:SatelliteQueue"));

                var satellitesJson = await _celestrakService.GetGroupDataAsync(groupName);
                var satellites = JsonConvert.DeserializeObject<List<SatDTO>>(satellitesJson);

                foreach(SatDTO sat in satellites)
                {
                    sat.SatGroupID = (int)groupID;
                    await endpoint.Send(sat);
                }

            }

            await Task.CompletedTask;
        }
    }
}
