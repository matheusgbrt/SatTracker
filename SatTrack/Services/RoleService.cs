using Microsoft.EntityFrameworkCore;
using SatTrack.DAL;
using SatTrack.Services.Interfaces;
using SatTrack.Users.DTO;

namespace SatTrack.Services
{
    public class RoleService : IRoleService
    {

        private readonly ElderveilContext elderveilContext;

        public RoleService(ElderveilContext context)
        {
            elderveilContext = context;
        }

        public async Task<IEnumerable<Role>> GetExistingRoles()
        {
            return await elderveilContext.Roles.ToListAsync();
        }


        public async Task<IEnumerable<Role>> GetRoleFromNames(List<RoleDTO> roles)
        {
            var roleNames = roles.Select(r => r.RoleName).ToList();
            return await elderveilContext.Roles.Where(r => roleNames.Contains(r.RoleName)).ToListAsync();
        }
    }
}
