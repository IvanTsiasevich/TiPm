using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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
