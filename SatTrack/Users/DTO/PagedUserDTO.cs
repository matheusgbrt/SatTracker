using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace SatTrack.Users.DTO
{
    public class PagedUserDTO
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public List<UserDTO> Users { get; set; } = [];
    }
}
