using MassTransit;
using Newtonsoft.Json;
using SatTrack.Contracts.Messages;
using SatTrack.Satellites.DTO.SatTrack.DTO;
using SatTrack.Services;
using SatTrack.Services.Interfaces;


namespace SatTrack.Contracts.Consumers
{
    public class GroupMessageConsumer(ISendEndpointProvider sendEndpointProvider, ISatGroupService groupService,CelestrakClientService celestrakClientService, ILoggingService loggingService) : IConsumer<SatGroupMessage>
    {
        private ISatGroupService _groupService = groupService;
        private CelestrakClientService _celestrakService = celestrakClientService;
        private readonly ISendEndpointProvider _sendEndpointProvider = sendEndpointProvider;
        private ILoggingService _loggingService = loggingService;

        public async Task Consume(ConsumeContext<SatGroupMessage> context)
        {
            var message = context.Message;
            var groupName = message.groupName;

            var groupID = await _groupService.GetGroupID(groupName);

            if (!groupID.HasValue)
            {
                _loggingService.PersistGroupUpdateLog("Group ID not found in Database", groupName, true);
                return;
            }

            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:SatelliteQueue"));

            try
            {
            var satellitesJson = await _celestrakService.GetGroupDataAsync(groupName);

                var satellites = JsonConvert.DeserializeObject<List<SatDTO>>(satellitesJson);

                if (satellites == null || !satellites.Any())
                {
                    _loggingService.PersistGroupUpdateLog($"No valid satellites found for group '{groupName}'", groupName, true);
                    return;
                }

                foreach (var sat in satellites)
                {
                    sat.SatGroupID = (int)groupID;
                    await endpoint.Send(sat);
                    _loggingService.PersistGroupUpdateLog($"Group sent for updates.", groupName, false);
                }
            }
            catch (JsonException ex)
            {
                _loggingService.PersistGroupUpdateLog($"Failed to deserialize JSON response for group '{groupName}'", groupName, true);
                return;
            }

            await Task.CompletedTask;
        }
    }
}
