using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ti.Pm.PmDb.Model
{
    [Table("User")]

    public class Users : IChangeLog
    {
        [Key]
        public int UserId { get; set; }
        public string PersonName { get; set; }
        public string PersonSurname { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public string? ChangeLogJson { get; set; }
        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}

