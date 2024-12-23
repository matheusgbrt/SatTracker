using SatTrack.DAL;
using SatTrack.Users.DTO;

namespace SatTrack.Services.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetExistingRoles();
        Task<IEnumerable<Role>> GetRoleFromNames(List<RoleDTO> roles);
    }
}
