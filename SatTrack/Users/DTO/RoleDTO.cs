namespace SatTrack.Users.DTO
{
    public class RoleDTO
    {
        public string RoleName { get; set; }



        public class RoleDTOEqualityComparer : IEqualityComparer<RoleDTO>
        {
            public bool Equals(RoleDTO x, RoleDTO y)
            {
                return string.Equals(x?.RoleName, y?.RoleName, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(RoleDTO obj)
            {
                return obj.RoleName?.GetHashCode(StringComparison.OrdinalIgnoreCase) ?? 0;
            }
        }
    }
}
