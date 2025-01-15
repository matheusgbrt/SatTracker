using Microsoft.EntityFrameworkCore;
using SatTrack.DAL;
using SatTrack.Satellites.DTO;
using SatTrack.Services.Interfaces;

namespace SatTrack.Services
{
    public class SatGroupService(ElderveilContext context) : ISatGroupService
    {
        private readonly ElderveilContext elderveilContext = context;

        public async Task<IEnumerable<SatGroupDTO>> GetAllGroupsDTO()
        {
            var groups = await elderveilContext.SatGroups.Include(g => g.SatGroupCat).ToListAsync();
            return groups.Select(group => new SatGroupDTO { 
                groupCatDescription = group.SatGroupCat.Description, 
                groupID = group.SatGroupId, 
                groupName = group.SatGroupName, 
                groupQuery = group.SatGroupQuery })
                .ToList();

        }

        public async Task<bool> CheckGroupExists(string groupQuery)
        {
            return await elderveilContext.SatGroups.AnyAsync(group => group.SatGroupQuery == groupQuery);
        }

        public async Task<int?> GetGroupID(string groupQuery)
        {
            return await elderveilContext.SatGroups.Where(group => group.SatGroupQuery == groupQuery).Select(group => (int?)group.SatGroupId).FirstOrDefaultAsync();
        }

        public async Task<SatGroup?> GetGroupByQuery(string groupQuery)
        {
            return await elderveilContext.SatGroups.Where(group => group.SatGroupQuery == groupQuery).Select(group => group).FirstOrDefaultAsync();
        }

        public async Task<SatGroup?> GetGroupByID(int groupID)
        {
            return await elderveilContext.SatGroups.Where(group => group.SatGroupId == groupID).Select(group => group).FirstOrDefaultAsync();
        }

    }
}
