using Tasks;

namespace Bookstore
{
	public sealed class Bookstore
	{
		private const string QUIT = "quit";
		private readonly IConsole console;

		private IList<Book> Books = new List<Book>();

		private IReadOnlyDictionary<int, string> CategoryDictionary = new Dictionary<int, string>
		{
			{ 1, "Fiction" },
			{ 2, "Romance" },
			{ 3, "Science Fiction" },
			{ 4, "Fantasy" },
			{ 5, "Mystery" },
			{ 6, "Biography" }
		};

		private long lastId = 0;

		public static void Main(string[] args)
		{
			new Bookstore(new RealConsole()).Run();
		}

		public Bookstore(IConsole console)
		{
			this.console = console;
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
			if (string.IsNullOrWhiteSpace(commandLine))
			{
				return;
			}

			var commandRest = commandLine.Split(" ".ToCharArray(), 2);
			var command = commandRest[0];
			switch (command) {
			case "show":
				Show();
				break;
			case "add":
				if (commandRest.Length < 2)
				{
					console.WriteLine("Usage: add <title> <author> <category> <description>");
					break;
				}
				Add(commandRest[1]);
				break;
			case "discontinueBook":
				if (commandRest.Length < 2)
				{
					console.WriteLine("Usage: discontinueBook <id>");
					break;
				}
				DiscontinueBook(commandRest[1]);
				break;
			case "discontinueAuthor":
				if (commandRest.Length < 2)
				{
					console.WriteLine("Usage: discontinueAuthor <author>");
					break;
				}
				DiscontinueByAuthor(commandRest[1]);
				break;
			case "help":
				Help();
				break;
			default:
				Error(command);
				break;
			}
		}

		private void Show()
		{
			if (!Books.Any())
			{
				console.WriteLine("No books in catalog.");
				return;
			}

			foreach (var book in Books) {
				console.WriteLine("[{0}] {1}: {2} {3}", book.Id, book.Title, book.Author, (book.IsDiscontinued ? "(discontinued)" : ""));
			}
			console.WriteLine();
		}

		private void Add(string commandLine)
		{
			var subcommandRest = commandLine.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
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
			if (Books.Any(b => b.Title == book))
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

			Books.Add(new Book
			{
				Id = NextId(),
				Title = book,
				CategoryId = categoryId,
				Description = description,
				Author = author,
				IsDiscontinued = false
			});
		}

		private void DiscontinueBook(string idString)
		{
			if (!long.TryParse(idString, out var id))
			{
				console.WriteLine("Invalid id: {0}", idString);
				return;
			}

			var identifiedBook = Books.FirstOrDefault(book => book.Id == id);
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

		public void DiscontinueByAuthor(string authorName)
		{
			authorName = authorName.Trim();
			if (string.IsNullOrWhiteSpace(authorName))
			{
				console.WriteLine("Usage: discontinueAuthor <author>");
				return;
			}

			bool foundAny = false;

			for (int j = 0; j <Books.Count; j++)
			{
				Book currentBook = Books[j];

				if (string.Equals(currentBook.Author.Trim(), authorName, StringComparison.OrdinalIgnoreCase))
				{
					if (currentBook.IsDiscontinued == false)
					{
						Books[j].IsDiscontinued = true;
						foundAny = true;

						console.WriteLine("Discontinued: '{0}' from author {1} (ID: {2})", currentBook.Title, currentBook.Author, currentBook.Id);
					}
				}
			}

			if (foundAny == false)
			{
				console.WriteLine("Could not find any active books by author: {0}", authorName);
			}
		}

		private void Help()
		{
			console.WriteLine("Commands:");
			console.WriteLine("  show");
			console.WriteLine("  add <title> <author> <category> <description>");
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

		private long NextId()
		{
			return ++lastId;
		}
	}
}
