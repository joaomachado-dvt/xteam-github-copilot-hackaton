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
            "add Dune Herbert Fiction Classic space saga",
            "show",
            "quit");
        var app = new global::Bookstore.Bookstore(console);

        app.Run();

        Assert.Contains("[1] Dune: Herbert", console.OutputAsString);
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
            "add Dune Herbert Fiction Classic",
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
        Assert.Contains("discontinueBook <id>", console.OutputAsString);
        Assert.Contains("discontinueAuthor <author>", console.OutputAsString);
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
