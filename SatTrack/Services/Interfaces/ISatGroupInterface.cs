using SatTrack.DAL;
using SatTrack.Satellites.DTO;
using SatTrack.Users.DTO;

namespace SatTrack.Services.Interfaces
{
    public interface ISatGroupService
    {
        Task<IEnumerable<SatGroupDTO>> GetAllGroupsDTO();
        Task<bool> CheckGroupExists(string groupQuery);
        Task<int?> GetGroupID(string groupQuery);
        Task<SatGroup?> GetGroupByQuery(string groupQuery);
        Task<SatGroup?> GetGroupByID(int groupID);
    }
}
