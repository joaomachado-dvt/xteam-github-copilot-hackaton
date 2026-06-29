namespace Bookstore;

public sealed class InMemoryAuthorRepository : IAuthorRepository
{
    public const int UnknownAuthorId = 0;
    public const string UnknownAuthorName = "Unknown Author";

    private readonly Dictionary<int, Author> _authors = new();

    public IReadOnlyList<Author> GetAll()
    {
        return _authors.Values.OrderBy(author => author.Id).ToList();
    }

    public Author? FindById(int id)
    {
        return _authors.TryGetValue(id, out var author) ? author : null;
    }

    public Author? FindByName(string name)
    {
        return _authors.Values.FirstOrDefault(
            author => string.Equals(author.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    public bool IdExists(int id)
    {
        return _authors.ContainsKey(id);
    }

    public bool Create(Author author)
    {
        if (author.Id == UnknownAuthorId || _authors.ContainsKey(author.Id))
        {
            return false;
        }

        _authors[author.Id] = author;
        return true;
    }

    public Author EnsureUnknownAuthor()
    {
        if (_authors.TryGetValue(UnknownAuthorId, out var existing))
        {
            return existing;
        }

        var unknown = new Author
        {
            Id = UnknownAuthorId,
            Name = UnknownAuthorName,
            BornDate = new DateOnly(1900, 1, 1),
            Awards = Array.Empty<string>()
        };

        _authors[UnknownAuthorId] = unknown;
        return unknown;
    }
}
