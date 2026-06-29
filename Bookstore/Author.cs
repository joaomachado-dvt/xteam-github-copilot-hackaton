namespace Bookstore;

public class Author
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateOnly BornDate { get; set; }
    public required IReadOnlyList<string> Awards { get; set; }
}
