using MassTransit;
using Newtonsoft.Json;
using SatTrack.Contracts.Messages;
using SatTrack.Satellites.DTO.SatTrack.DTO;
using SatTrack.Services;
using SatTrack.Services.Interfaces;


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

            var groupID = await _groupService.GetGroupID(groupName);

            if (!groupID.HasValue)
            {
                Console.WriteLine($"Warning: Group ID not found for '{groupName}', skipping processing.");
                return;
            }

            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:SatelliteQueue"));

            try
            {
            var satellitesJson = await _celestrakService.GetGroupDataAsync(groupName);

                var satellites = JsonConvert.DeserializeObject<List<SatDTO>>(satellitesJson);

                if (satellites == null || !satellites.Any())
                {
                    Console.WriteLine($"Warning: No valid satellites found for group '{groupName}', skipping processing.");
                    return;
                }

                foreach (var sat in satellites)
                {
                    sat.SatGroupID = (int)groupID;
                    await endpoint.Send(sat);
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error: Failed to deserialize JSON response for group '{groupName}'. Exception: {ex.Message}");
                return;
            }

            await Task.CompletedTask;
        }
    }
}
