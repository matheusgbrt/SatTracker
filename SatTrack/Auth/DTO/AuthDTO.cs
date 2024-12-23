using System.ComponentModel.DataAnnotations;

namespace SatTrack.Auth.DTO
{
    public class AuthDTO
    {
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
    }
}
