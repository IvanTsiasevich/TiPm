using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Ti.Pm.PmDb.Model
{
    [Table("TaskType")]

    public class TaskTypePm
    {
        [Key]
        public int TaskTypeId { get; set; }
        public string Title { get; set; }
        [Required]
        [Timestamp]
        public byte[] Timestamp { get; set; }
        public string? ChangeLogJson { get; set; }
    }
}
