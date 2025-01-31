using SatTrack.DAL;
using SatTrack.Satellites.DTO.SatTrack.DTO;

namespace SatTrack.Services.Interfaces
{
    public interface ISatService
    {
        Task<bool> CheckSatExistsByObjectName(string objectName);
        Task<bool> CheckSatExistsByObjectId(string objectId);
        Sat ConvertDTO(SatDTO satDTO, List<SatGroup> groups);
        Task<bool> InsertSat(Sat sat);
        Task<Sat?> GetSatByObjectName(string objectName);
        Task UpdateSat(Sat sat, SatGroup satGroup);
    }
}
