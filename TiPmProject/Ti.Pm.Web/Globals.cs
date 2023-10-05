using Ti.Pm.PmDb.Model;

namespace Ti.Pm.Web
{
    public class Globals
    {
        public static List<UserRole> UserRoleList => InitUnterRole();
        private static List<UserRole> InitUnterRole()
        {
            var list = new List<UserRole>
                {
                    new UserRole {RoleId = 1, RoleName = "Admin" },
                    new UserRole {RoleId = 2, RoleName = "User"},
                };
            return list;
        }
    }
}
