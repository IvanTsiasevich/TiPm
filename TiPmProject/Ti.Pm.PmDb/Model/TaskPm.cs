using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ti.Pm.PmDb.Model
{

    [Table("Task")]

    public class TaskPm
    {
        [Key]
        public int TaskId { get; set; }
        public string Title { get; set; }
        [Required]
        [Timestamp]
        public byte[] Timestamp { get; set; }
        public string? ChangeLogJson { get; set; }
        public int ProjectId { get; set; }
        public int StatusId { get; set; }
        public int TaskTypeId { get; set; }
        public string Description { get; set; }

    }
}

