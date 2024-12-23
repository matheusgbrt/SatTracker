using SatTrack.DAL;
using SatTrack.Users.DTO;

namespace SatTrack.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO> GetUserByIdAsync(int userId);
        Task<User> GetUserByNameAsync(string name);

        Task<User> CreateUser(CreateUserDTO userDTO);

        Task<User> UpdateUser(User user, UpdateUserDTO updateUserDTO);
    }
}
