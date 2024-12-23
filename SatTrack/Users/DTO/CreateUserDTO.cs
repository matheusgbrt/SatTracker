namespace SatTrack.Users.DTO
{
    public class CreateUserDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public List<RoleDTO> Roles { get; set; } = new List<RoleDTO>();
    }
}
