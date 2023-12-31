﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Ti.Pm.PmDb.Model
{
    [Table("Task")]
    public class TaskPm : IChangeLog
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

