using LibrarySystem.Models;

public class MovementOfLibraryUnit
{
    public int Id { get; set; }

    public DateTime DateTime { get; set; }

    public DateOnly? Deadline { get; set; }

    public string Type { get; set; }

    public int LibraryUnitId { get; set; }
    public LibraryUnit LibraryUnit { get; set; }

    public int ReaderId { get; set; }
    public User Reader { get; set; }

    public int LibrarianId { get; set; }
    public User Librarian { get; set; }

    public string Condition { get; set; }
}