using System.Text;
using Tasks;

namespace Bookstore.Tests;

public class BookstoreBehaviorTests
{
    [Fact]
    public void Run_WithQuit_ExitsWithoutErrors()
    {
        var console = new FakeConsole("quit");
        var app = new global::Bookstore.Bookstore(console);

        app.Run();

        Assert.Contains("> ", console.OutputAsString);
    }

    [Fact]
    public void Run_ShowOnEmptyCatalog_PrintsEmptyMessage()
    {
        var console = new FakeConsole("show", "quit");
        var app = new global::Bookstore.Bookstore(console);

        app.Run();

        Assert.Contains("No books in catalog.", console.OutputAsString);
    }

    [Fact]
    public void Run_AddThenShow_PrintsAddedBook()
    {
        var console = new FakeConsole(
            "add Dune Unknown Fiction Classic space saga",
            "show",
            "quit");
        var app = new global::Bookstore.Bookstore(console);

        app.Run();

        Assert.Contains("[1] Dune: Unknown Author", console.OutputAsString);
    }

    [Fact]
    public void Run_AddDuplicateTitle_PrintsDuplicateError()
    {
        var console = new FakeConsole(
            "add Dune Herbert Fiction First",
            "add Dune Another Fiction Second",
            "quit");
        var app = new global::Bookstore.Bookstore(console);

        app.Run();

        Assert.Contains("Duplicate book with the name \"Dune\".", console.OutputAsString);
    }

    [Fact]
    public void Run_DiscontinueBookInvalidId_PrintsInvalidId()
    {
        var console = new FakeConsole("discontinueBook nope", "quit");
        var app = new global::Bookstore.Bookstore(console);

        app.Run();

        Assert.Contains("Invalid id: nope", console.OutputAsString);
    }

    [Fact]
    public void Run_DiscontinueBookSuccess_MarksBookAsDiscontinued()
    {
        var console = new FakeConsole(
            "add Dune Unknown Fiction Classic",
            "discontinueBook 1",
            "show",
            "quit");
        var app = new global::Bookstore.Bookstore(console);

        app.Run();

        Assert.Contains("Discontinued: 'Dune' (ID: 1)", console.OutputAsString);
        Assert.Contains("(discontinued)", console.OutputAsString);
    }

    [Fact]
    public void Run_DiscontinueAuthorSuccess_MarksAuthorBooks()
    {
        var console = new FakeConsole(
            "addAuthor 12 Herbert 1980-11-02 Nobel,National",
            "addAuthor 13 Asimov 1920-01-02 Hugo",
            "add Dune Herbert Fiction Classic",
            "add Foundation Asimov Science Fiction Epic",
            "discontinueAuthor Herbert",
            "show",
            "quit");
        var app = new global::Bookstore.Bookstore(console);

        app.Run();

        Assert.Contains("Discontinued: 'Dune' from author Herbert (ID: 1)", console.OutputAsString);
        Assert.Contains("[2] Foundation: Asimov", console.OutputAsString);
    }

    [Fact]
    public void Run_Help_PrintsCurrentCommands()
    {
        var console = new FakeConsole("help", "quit");
        var app = new global::Bookstore.Bookstore(console);

        app.Run();

        Assert.Contains("add <title> <author> <category> <description>", console.OutputAsString);
        Assert.Contains("addAuthor <id> <name> <bornDateYYYY-MM-DD> [awardsCommaSeparated]", console.OutputAsString);
        Assert.Contains("showAuthors", console.OutputAsString);
        Assert.Contains("discontinueBook <id>", console.OutputAsString);
        Assert.Contains("discontinueAuthor <author>", console.OutputAsString);
    }

    [Fact]
    public void Run_AddAuthorThenShowAuthors_PrintsCreatedAuthor()
    {
        var console = new FakeConsole(
            "addAuthor 12 Jane 1980-11-02 Booker,Nobel",
            "showAuthors",
            "quit");
        var app = new global::Bookstore.Bookstore(console);

        app.Run();

        Assert.Contains("Author created: 12 - Jane", console.OutputAsString);
        Assert.Contains("[12] Jane | Born: 1980-11-02 | Awards: Booker,Nobel", console.OutputAsString);
    }

    [Fact]
    public void Run_AddAuthorDuplicateId_PrintsDuplicateError()
    {
        var console = new FakeConsole(
            "addAuthor 12 Jane 1980-11-02 Booker",
            "addAuthor 12 John 1982-01-01 Award",
            "quit");
        var app = new global::Bookstore.Bookstore(console);

        app.Run();

        Assert.Contains("Duplicate author id: 12", console.OutputAsString);
    }

    [Fact]
    public void Run_AddAuthorInvalidDate_PrintsValidationError()
    {
        var console = new FakeConsole(
            "addAuthor 12 Jane 1980-13-99 Booker",
            "quit");
        var app = new global::Bookstore.Bookstore(console);

        app.Run();

        Assert.Contains("Invalid born date. Use YYYY-MM-DD.", console.OutputAsString);
    }

    [Fact]
    public void Run_AddAuthorReservedId_PrintsReservedIdError()
    {
        var console = new FakeConsole(
            "addAuthor 0 Unknown 1900-01-01 None",
            "quit");
        var app = new global::Bookstore.Bookstore(console);

        app.Run();

        Assert.Contains("Author id 0 is reserved for Unknown Author.", console.OutputAsString);
    }

    [Fact]
    public void Run_ShowAuthorsEmpty_PrintsEmptyMessage()
    {
        var console = new FakeConsole(
            "showAuthors",
            "quit");
        var app = new global::Bookstore.Bookstore(console);

        app.Run();

        Assert.Contains("No authors in catalog.", console.OutputAsString);
    }

    [Fact]
    public void Run_AddWithMissingAuthor_UsesUnknownAuthorFallback()
    {
        var console = new FakeConsole(
            "add Dune NonExisting Fiction Classic",
            "show",
            "showAuthors",
            "quit");
        var app = new global::Bookstore.Bookstore(console);

        app.Run();

        Assert.Contains("[1] Dune: Unknown Author", console.OutputAsString);
        Assert.Contains("[0] Unknown Author", console.OutputAsString);
    }

    [Fact]
    public void Run_AddWithMissingAuthor_ReusesUnknownAuthor()
    {
        var console = new FakeConsole(
            "add BookOne MissingA Fiction Classic",
            "add BookTwo MissingB Fiction Classic",
            "showAuthors",
            "quit");
        var app = new global::Bookstore.Bookstore(console);

        app.Run();

        var output = console.OutputAsString;
        Assert.Equal(1, CountOccurrences(output, "[0] Unknown Author"));
    }

    private static int CountOccurrences(string source, string value)
    {
        var count = 0;
        var index = 0;
        while ((index = source.IndexOf(value, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += value.Length;
        }

        return count;
    }
}

internal sealed class FakeConsole(params string[] inputs) : IConsole
{
    private readonly Queue<string> _inputs = new(inputs);
    private readonly StringBuilder _output = new();

    public string OutputAsString => _output.ToString();

    public string ReadLine()
    {
        return _inputs.Count > 0 ? _inputs.Dequeue() : "quit";
    }

    public void Write(string format, params object[] args)
    {
        _output.AppendFormat(format, args);
    }

    public void WriteLine(string format, params object[] args)
    {
        _output.AppendFormat(format, args);
        _output.AppendLine();
    }

    public void WriteLine()
    {
        _output.AppendLine();
    }
}
