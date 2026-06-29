namespace Bookstore;

public class Book
{
    public long Id { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
    public int CategoryId { get; set; }
    public required string Description { get; set; }
    public bool IsDiscontinued { get; set; }
}
