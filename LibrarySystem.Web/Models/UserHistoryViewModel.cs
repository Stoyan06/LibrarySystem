using LibrarySystem.Models;

namespace LibrarySystem.Web.Models
{
    public class UserHistoryViewModel
    {
        public DateTime DateTime { get; set; }

        public DateOnly? DeadLine { get; set; }

        public string TypeOperation {  get; set; }

        public LibraryUnit LibraryUnit { get; set; }

        public User Librarian {  get; set; }

        public string Condition {  get; set; }
    }
}
