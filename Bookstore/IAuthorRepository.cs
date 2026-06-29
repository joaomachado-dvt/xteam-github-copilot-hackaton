namespace Bookstore;

public interface IAuthorRepository
{
    IReadOnlyList<Author> GetAll();
    Author? FindById(int id);
    Author? FindByName(string name);
    bool IdExists(int id);
    bool Create(Author author);
    Author EnsureUnknownAuthor();
}
