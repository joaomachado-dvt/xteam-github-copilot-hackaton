using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tasks;
using System.Globalization;

namespace Bookstore
{
	public sealed class Bookstore
	{
		private const string QUIT = "quit";
		private readonly IConsole console;
		private readonly IBookRepository repository;
		private readonly IAuthorRepository authorRepository;

		private IReadOnlyDictionary<int, string> CategoryDictionary = new Dictionary<int, string>
		{
			{ 1, "Fiction" },
			{ 2, "Romance" },
			{ 3, "Science Fiction" },
			{ 4, "Fantasy" },
			{ 5, "Mystery" },
			{ 6, "Biography" }
		};

		public static void Main(string[] args)
		{
			using var host = Host
				.CreateDefaultBuilder(args)
				.ConfigureServices(services =>
				{
					services.AddSingleton<IConsole, RealConsole>();
					services.AddSingleton<IBookRepository, InMemoryBookRepository>();
					services.AddSingleton<IAuthorRepository, InMemoryAuthorRepository>();
					services.AddSingleton<Bookstore>();
				})
				.Build();

			host.Services.GetRequiredService<Bookstore>().Run();
		}

		public Bookstore(IConsole console)
			: this(console, new InMemoryBookRepository(), new InMemoryAuthorRepository())
		{
		}

		public Bookstore(IConsole console, IBookRepository repository, IAuthorRepository authorRepository)
		{
			this.console = console;
			this.repository = repository;
			this.authorRepository = authorRepository;
		}

		public void Run()
		{
			while (true) {
				try
				{
					console.Write("> ");
					var command = console.ReadLine().Trim();
					if (command == QUIT) {
						break;
					}
					if (string.IsNullOrWhiteSpace(command)) {
						continue;
					}
					Execute(command);
				} catch (Exception e)
				{
					console.WriteLine("Error: {0}", e.Message);
				}
			}
		}

		private void Execute(string commandLine)
		{
			var command = CommandParser.Parse(commandLine);
			Handle(command);
		}

		private void Handle(ICommand command)
		{
			switch (command)
			{
				case EmptyCommand:
					return;
				case InvalidUsageCommand invalidUsage:
					console.WriteLine(invalidUsage.Message);
					return;
				case UnknownCommand unknown:
					Error(unknown.Name);
					return;
				case ShowCommand:
					HandleShow();
					return;
				case AddCommand add:
					HandleAdd(add);
					return;
				case AddAuthorCommand addAuthor:
					HandleAddAuthor(addAuthor);
					return;
				case ShowAuthorsCommand:
					HandleShowAuthors();
					return;
				case DiscontinueBookCommand discontinueBook:
					HandleDiscontinueBook(discontinueBook);
					return;
				case DiscontinueAuthorCommand discontinueAuthor:
					HandleDiscontinueAuthor(discontinueAuthor);
					return;
				case HelpCommand:
					HandleHelp();
					return;
				default:
					throw new InvalidOperationException($"Unsupported command type: {command.GetType().Name}");
			}
		}

		private void HandleShow()
		{
			var books = repository.GetAll();

			if (!books.Any())
			{
				console.WriteLine("No books in catalog.");
				return;
			}

			foreach (var book in books) {
				var author = authorRepository.FindById(book.AuthorId) ?? authorRepository.EnsureUnknownAuthor();
				book.AuthorId = author.Id;
				book.Author = author.Name;
				console.WriteLine("[{0}] {1}: {2} {3}", book.Id, book.Title, author.Name, (book.IsDiscontinued ? "(discontinued)" : ""));
			}
			console.WriteLine();
		}

		private void HandleAdd(AddCommand command)
		{
			var subcommandRest = command.Arguments.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			if (subcommandRest.Length < 4)
			{
				console.WriteLine("Usage: add <title> <author> <category> <description>");
				return;
			}

			var title = subcommandRest[0];
			var author = subcommandRest[1];
			var remaining = string.Join(" ", subcommandRest.Skip(2));

			var matchedCategory = CategoryDictionary
				.Values
				.OrderByDescending(value => value.Length)
				.FirstOrDefault(value => remaining.StartsWith(value + " ", StringComparison.OrdinalIgnoreCase)
					|| string.Equals(remaining, value, StringComparison.OrdinalIgnoreCase));

			if (matchedCategory is null)
			{
				console.WriteLine("Invalid category. Supported values: {0}", string.Join(", ", CategoryDictionary.Values));
				return;
			}

			var description = remaining.Length == matchedCategory.Length
				? string.Empty
				: remaining.Substring(matchedCategory.Length).TrimStart();

			if (string.IsNullOrWhiteSpace(description))
			{
				console.WriteLine("Usage: add <title> <author> <category> <description>");
				return;
			}

			AddBook(title, author, matchedCategory, description);
		}

		private void AddBook(string book, string author, string category, string description)
		{
			if (repository.TitleExists(book))
			{
				console.WriteLine("Duplicate book with the name \"{0}\".", book);
				return;
			}

			var categoryMatch = CategoryDictionary
				.FirstOrDefault(entry => string.Equals(entry.Value, category, StringComparison.OrdinalIgnoreCase));
			var categoryId = categoryMatch.Key;

			if (categoryId == 0)
			{
				console.WriteLine("Invalid category");
				return;
			}

			var referencedAuthor = authorRepository.FindByName(author);
			if (referencedAuthor is null)
			{
				referencedAuthor = authorRepository.EnsureUnknownAuthor();
			}

			repository.Add(book, referencedAuthor.Id, referencedAuthor.Name, categoryId, description);
		}

		private void HandleAddAuthor(AddAuthorCommand command)
		{
			var parts = command.Arguments.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length < 3)
			{
				console.WriteLine("Usage: addAuthor <id> <name> <bornDateYYYY-MM-DD> [awardsCommaSeparated]");
				return;
			}

			if (!int.TryParse(parts[0], out var id))
			{
				console.WriteLine("Invalid author id: {0}", parts[0]);
				return;
			}

			if (id == InMemoryAuthorRepository.UnknownAuthorId)
			{
				console.WriteLine("Author id 0 is reserved for Unknown Author.");
				return;
			}

			var name = parts[1].Trim();
			if (string.IsNullOrWhiteSpace(name))
			{
				console.WriteLine("Author name is required.");
				return;
			}

			var bornDateText = parts[2];
			if (!DateOnly.TryParseExact(bornDateText, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var bornDate))
			{
				console.WriteLine("Invalid born date. Use YYYY-MM-DD.");
				return;
			}

			if (authorRepository.IdExists(id))
			{
				console.WriteLine("Duplicate author id: {0}", id);
				return;
			}

			var awards = Array.Empty<string>();
			if (parts.Length >= 4)
			{
				awards = parts[3]
					.Split(',')
					.Select(value => value.Trim())
					.Where(value => !string.IsNullOrWhiteSpace(value))
					.ToArray();
			}

			var created = authorRepository.Create(new Author
			{
				Id = id,
				Name = name,
				BornDate = bornDate,
				Awards = awards
			});

			if (!created)
			{
				console.WriteLine("Duplicate author id: {0}", id);
				return;
			}

			console.WriteLine("Author created: {0} - {1}", id, name);
		}

		private void HandleShowAuthors()
		{
			var authors = authorRepository.GetAll();
			if (!authors.Any())
			{
				console.WriteLine("No authors in catalog.");
				return;
			}

			foreach (var author in authors)
			{
				var awards = author.Awards.Any() ? string.Join(",", author.Awards) : "none";
				console.WriteLine("[{0}] {1} | Born: {2:yyyy-MM-dd} | Awards: {3}", author.Id, author.Name, author.BornDate, awards);
			}
			console.WriteLine();
		}

		private void HandleDiscontinueBook(DiscontinueBookCommand command)
		{
			var idString = command.IdText;
			if (!long.TryParse(idString, out var id))
			{
				console.WriteLine("Invalid id: {0}", idString);
				return;
			}

			var identifiedBook = repository.FindById(id);
			if (identifiedBook is null)
			{
				console.WriteLine("Book not found with id: {0}", id);
				return;
			}

			if (identifiedBook.IsDiscontinued)
			{
				console.WriteLine("Book already discontinued: {0}", identifiedBook.Title);
				return;
			}

			identifiedBook.IsDiscontinued = true;
			console.WriteLine("Discontinued: '{0}' (ID: {1})", identifiedBook.Title, identifiedBook.Id);
		}

		private void HandleDiscontinueAuthor(DiscontinueAuthorCommand command)
		{
			var authorName = command.AuthorName;
			authorName = authorName.Trim();
			if (string.IsNullOrWhiteSpace(authorName))
			{
				console.WriteLine("Usage: discontinueAuthor <author>");
				return;
			}

			var books = repository.GetAll();
			bool foundAny = false;

			var author = authorRepository.FindByName(authorName);

			for (int j = 0; j < books.Count; j++)
			{
				Book currentBook = books[j];
				if (author is null)
				{
					continue;
				}

				if (currentBook.AuthorId == author.Id)
				{
					if (currentBook.IsDiscontinued == false)
					{
						books[j].IsDiscontinued = true;
						foundAny = true;

						console.WriteLine("Discontinued: '{0}' from author {1} (ID: {2})", currentBook.Title, author.Name, currentBook.Id);
					}
				}
			}

			if (foundAny == false)
			{
				console.WriteLine("Could not find any active books by author: {0}", authorName);
			}
		}

		private void HandleHelp()
		{
			console.WriteLine("Commands:");
			console.WriteLine("  show");
			console.WriteLine("  showAuthors");
			console.WriteLine("  add <title> <author> <category> <description>");
			console.WriteLine("  addAuthor <id> <name> <bornDateYYYY-MM-DD> [awardsCommaSeparated]");
			console.WriteLine("  discontinueBook <id>");
			console.WriteLine("  discontinueAuthor <author>");
			console.WriteLine("  help");
			console.WriteLine("  quit");
			console.WriteLine();
		}

		private void Error(string command)
		{
			console.WriteLine("I don't know what the command \"{0}\" is.", command);
		}

		private interface ICommand;

		private sealed record EmptyCommand : ICommand;

		private sealed record ShowCommand : ICommand;

		private sealed record HelpCommand : ICommand;

		private sealed record AddCommand(string Arguments) : ICommand;

		private sealed record AddAuthorCommand(string Arguments) : ICommand;

		private sealed record ShowAuthorsCommand : ICommand;

		private sealed record DiscontinueBookCommand(string IdText) : ICommand;

		private sealed record DiscontinueAuthorCommand(string AuthorName) : ICommand;

		private sealed record UnknownCommand(string Name) : ICommand;

		private sealed record InvalidUsageCommand(string Message) : ICommand;

		private static class CommandParser
		{
			public static ICommand Parse(string commandLine)
			{
				if (string.IsNullOrWhiteSpace(commandLine))
				{
					return new EmptyCommand();
				}

				var commandRest = commandLine.Split(" ".ToCharArray(), 2);
				var command = commandRest[0];

				switch (command)
				{
					case "show":
						return new ShowCommand();
					case "showAuthors":
						return new ShowAuthorsCommand();
					case "add":
						return commandRest.Length < 2
							? new InvalidUsageCommand("Usage: add <title> <author> <category> <description>")
							: new AddCommand(commandRest[1]);
					case "addAuthor":
						return commandRest.Length < 2
							? new InvalidUsageCommand("Usage: addAuthor <id> <name> <bornDateYYYY-MM-DD> [awardsCommaSeparated]")
							: new AddAuthorCommand(commandRest[1]);
					case "discontinueBook":
						return commandRest.Length < 2
							? new InvalidUsageCommand("Usage: discontinueBook <id>")
							: new DiscontinueBookCommand(commandRest[1]);
					case "discontinueAuthor":
						return commandRest.Length < 2
							? new InvalidUsageCommand("Usage: discontinueAuthor <author>")
							: new DiscontinueAuthorCommand(commandRest[1]);
					case "help":
						return new HelpCommand();
					default:
						return new UnknownCommand(command);
				}
			}
		}
	}
}
