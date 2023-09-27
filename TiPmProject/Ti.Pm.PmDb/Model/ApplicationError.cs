using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Ti.Pm.PmDb.Model
{
    [Table("LogApplicationError")]
    public class ApplicationError
    {
        [Key]
        public int LogApplicationErrorId { get; set; }
        public DateTime InsertDate { get; set; }
        public string ErrorContext { get; set; }
        public string ErrorMessage { get; set; }
        public string? ErrorInnerException { get; set; }
    }
}

