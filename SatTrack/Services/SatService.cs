using Microsoft.EntityFrameworkCore;
using SatTrack.DAL;
using SatTrack.Satellites.DTO.SatTrack.DTO;
using SatTrack.Services.Interfaces;

namespace SatTrack.Services
{
    public class SatService(ElderveilContext context) : ISatService
    {
        private readonly ElderveilContext _context = context;


        public async Task<bool> CheckSatExistsByObjectName(string objectName)
        {
            return await _context.Sats.AnyAsync(x => x.ObjectName == objectName);
        }

        public async Task<Sat?> GetSatByObjectName(string objectName)
        {
            return await _context.Sats.Include(s => s.SatGroups).Where(sat => sat.ObjectName == objectName).Select(sat => sat).FirstAsync();
        } 

        public async Task<bool> InsertSat(Sat sat)
        {
            if (sat == null)
                throw new ArgumentNullException(nameof(sat));

            if (await CheckSatExistsByObjectName(sat.ObjectName!))
                return false;

            _context.Sats.Add(sat);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("An error occurred while inserting the satellite.", ex);
            }
        }


        public async Task UpdateSat(Sat sat, SatGroup satGroup)
        {
            if (!sat.SatGroups.Any(g => g.SatGroupId == satGroup.SatGroupId)){
                sat.SatGroups.Add(satGroup);
            }
            await _context.SaveChangesAsync();
        }

        public Sat ConvertDTO(SatDTO satDTO, List<SatGroup> groups)
        {

            Sat sat =  new Sat
            {
                ObjectName = satDTO.ObjectName,
                ObjectId = satDTO.ObjectId,
                Epoch = satDTO.Epoch,
                MeanMotion = satDTO.MeanMotion,
                Eccentricity = satDTO.Eccentricity,
                Inclination = satDTO.Inclination,
                RaOfAscNode = satDTO.RaOfAscNode,
                ArgOfPericenter = satDTO.ArgOfPericenter,
                MeanAnomaly = satDTO.MeanAnomaly,
                EphemerisType = satDTO.EphemerisType,
                ClassificationType = satDTO.ClassificationType,
                NoradCatId = satDTO.NoradCatId,
                ElementSetNo = satDTO.ElementSetNo,
                RevAtEpoch = satDTO.RevAtEpoch,
                Bstar = satDTO.Bstar,
                MeanMotionDot = satDTO.MeanMotionDot,
                MeanMotionDdot = satDTO.MeanMotionDdot,
            };
            sat.SatGroups = groups;
            return sat;

        }

    }
}
