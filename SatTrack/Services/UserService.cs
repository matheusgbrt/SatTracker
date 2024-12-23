using Microsoft.EntityFrameworkCore;
using SatTrack.DAL;
using SatTrack.Services.Interfaces;
using SatTrack.Users.DTO;

namespace SatTrack.Services
{
    public class UserService : IUserService
    {

        private readonly ElderveilContext elderveilContext;
        private readonly PasswordService passwordService;
        private readonly IRoleService roleService;

        public UserService(ElderveilContext context, PasswordService passwordService, IRoleService roleService)
        {
            elderveilContext = context;
            this.passwordService = passwordService;
            this.roleService = roleService;
        }



        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await elderveilContext.Users.Include(u => u.Roles).ToListAsync();
            return users.Select(user => new UserDTO
            {
                Id = user.UserId,
                Username = user.Username,
                Active = (bool)user.Active,
                Roles = user.Roles.Select(r => new RoleDTO
                {
                    RoleName = r.RoleName
                }).ToList()
            });
        }
        public async Task<UserDTO> GetUserByIdAsync(int userId)
        {
            var user = await elderveilContext.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return null;

            return new UserDTO
            {
                Id = user.UserId,
                Username = user.Username,
                Active = (bool)user.Active,
                Roles = user.Roles.Select(r => new RoleDTO
                {
                    RoleName = r.RoleName
                }).ToList()
            };
        }

        public async Task<User> GetUserByNameAsync(string userName)
        {
            var user = await elderveilContext.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Username == userName);
            if (user == null) return null;

            return user;
        }

        public async Task<User> CreateUser(CreateUserDTO userDTO)
        {

            var existingRoles = (await roleService.GetRoleFromNames(userDTO.Roles)).ToList();

            var user = new User
            {
                Username = userDTO.Username,
                Password = passwordService.HashPassword(userDTO.Password),
                Active = true,
                Roles = existingRoles
            };
            elderveilContext.Users.Add(user);
            await elderveilContext.SaveChangesAsync();
            return user;

        }

        public async Task<User> UpdateUser(User user, UpdateUserDTO updateUserDTO)
        {
            user.Password = passwordService.HashPassword(updateUserDTO.Password);
            var roles = (await roleService.GetRoleFromNames(updateUserDTO.Roles)).ToList();
            user.Roles = roles;
            user.Active = updateUserDTO.Active;
            elderveilContext.Users.Update(user);
            await elderveilContext.SaveChangesAsync();
            return user;
        }

    }
}
