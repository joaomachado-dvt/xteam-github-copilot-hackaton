namespace Bookstore;

public interface IBookRepository
{
    IReadOnlyList<Book> GetAll();
    Book? FindById(long id);
    bool TitleExists(string title);
    Book Add(string title, int authorId, string authorName, int categoryId, string description);
}
