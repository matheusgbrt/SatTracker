using MassTransit.Transports;
using MassTransit;
using SatTrack.Services.Interfaces;
using SatTrack.Services;
using SatTrack.Contracts.Messages;
using SatTrack.Satellites.DTO.SatTrack.DTO;
using SatTrack.DAL;

namespace SatTrack.Contracts.Consumers
{
    public class SatMessageConsumer(ISendEndpointProvider sendEndpointProvider, ISatGroupService groupService, ISatService satService) : IConsumer<SatDTO>
    {

        private ISatGroupService _groupService = groupService;
        private ISatService _satService = satService;
        private readonly ISendEndpointProvider _sendEndpointProvider = sendEndpointProvider;


        public async Task Consume(ConsumeContext<SatDTO> context)
        {
            var satExists = await _satService.CheckSatExistsByObjectName(context.Message.ObjectName);

            if (satExists)
            {
                SatDTO message = context.Message;
                var satGroup = await _groupService.GetGroupByID(context.Message.SatGroupID);
                Sat? sat = await _satService.GetSatByObjectName(context.Message.ObjectName);

                sat.Epoch = message.Epoch;
                sat.Bstar = message.Bstar;
                sat.Inclination = message.Inclination;
                sat.ArgOfPericenter = message.ArgOfPericenter;
                sat.Eccentricity = message.Eccentricity;
                sat.ClassificationType = message.ClassificationType;
                sat.ElementSetNo = message.ElementSetNo;
                sat.EphemerisType = message.EphemerisType;
                sat.MeanAnomaly = message.MeanAnomaly;
                sat.RaOfAscNode = message.RaOfAscNode;
                sat.MeanMotion = message.MeanMotion;
                sat.MeanMotionDot = message.MeanMotionDot;
                sat.MeanMotionDdot = message.MeanMotionDdot;
                sat.RevAtEpoch = message.RevAtEpoch;
                await _satService.UpdateSat(sat, satGroup);
            }
            else
            {
                var satGroup = await _groupService.GetGroupByID(context.Message.SatGroupID);
                List<SatGroup> list = new List<SatGroup> { satGroup };
                Sat sat = _satService.ConvertDTO(context.Message, list);
                await _satService.InsertSat(sat);
            }

        }
    }

}
