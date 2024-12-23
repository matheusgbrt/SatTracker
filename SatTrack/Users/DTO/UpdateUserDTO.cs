namespace SatTrack.Users.DTO
{
    public class UpdateUserDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }
        public List<RoleDTO> Roles { get; set; } = new List<RoleDTO>();
    }
}
