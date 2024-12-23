namespace SatTrack.Auth.DTO
{
    public class AuthResultDTO
    {
        public string Token { get; set; }
        public DateTime ValidThrough  { get; set; }
        public List<string> Roles { get; set; }

    }
}
