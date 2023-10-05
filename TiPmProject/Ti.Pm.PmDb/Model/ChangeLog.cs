namespace Ti.Pm.PmDb.Model
{
    public class ChangeLog
    {
        public string Operation {  get; set; }
        public string? User { get; set; }
        public DateTime Date { get; set; }
    }
}
