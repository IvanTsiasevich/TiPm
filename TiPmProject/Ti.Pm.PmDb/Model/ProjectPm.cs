using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Ti.Pm.PmDb.Model
{
    [Table("Project")]

    public class ProjectPm
    {
        [Key]
        public int ProjectId { get; set; }
        public string Title { get; set; }
        [Required]
        [Timestamp]
        public byte[] Timestamp { get; set; }
        public string? ChangeLogJson { get; set; }
    }
}

