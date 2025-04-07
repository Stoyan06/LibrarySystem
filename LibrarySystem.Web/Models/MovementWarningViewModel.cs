namespace LibrarySystem.Web.Models
{
    public class MovementWarningViewModel
    {
        public DateOnly? DeadLine { get; set; }
        public int LibraryUnitId { get; set; }
        public int ReaderId { get; set; }

        public string ReaderName { get; set; }
        }
    }