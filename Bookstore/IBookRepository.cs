namespace Bookstore;

public interface IBookRepository
{
    IReadOnlyList<Book> GetAll();
    Book? FindById(long id);
    bool TitleExists(string title);
    Book Add(string title, string author, int categoryId, string description);
}
