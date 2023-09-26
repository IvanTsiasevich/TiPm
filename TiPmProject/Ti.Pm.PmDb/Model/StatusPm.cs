using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ti.Pm.PmDb.Model
{
    [Table("Status")]

    public class StatusPm
    {
        [Key]
        public int StatusId { get; set; }
        public string Title { get; set; }
        [Required]
        [Timestamp]
        public byte[] Timestamp { get; set; }
        public string? ChangeLogJson { get; set; }
        public int OrderId { get; set; }

    }
}
