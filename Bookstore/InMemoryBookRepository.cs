namespace Bookstore;

public sealed class InMemoryBookRepository : IBookRepository
{
    private readonly List<Book> _books = new();
    private long _lastId;

    public IReadOnlyList<Book> GetAll()
    {
        return _books;
    }

    public Book? FindById(long id)
    {
        return _books.FirstOrDefault(book => book.Id == id);
    }

    public bool TitleExists(string title)
    {
        return _books.Any(book => book.Title == title);
    }

    public Book Add(string title, int authorId, string authorName, int categoryId, string description)
    {
        var book = new Book
        {
            Id = NextId(),
            Title = title,
            AuthorId = authorId,
            Author = authorName,
            CategoryId = categoryId,
            Description = description,
            IsDiscontinued = false
        };

        _books.Add(book);
        return book;
    }

    private long NextId()
    {
        return ++_lastId;
    }
}
