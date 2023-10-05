using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ti.Pm.PmDb.Model
{
    public class UserRole
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
