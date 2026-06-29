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
					var command = console.ReadLine();
					if (command == QUIT) {
						break;
					}
					Execute(command);
				} catch (Exception e)
				{
					Console.Error.WriteLine("Error:" + e.Message);
				}
			}
		}

		private void Execute(string commandLine)
		{
			var commandRest = commandLine.Split(" ".ToCharArray(), 2);
			var command = commandRest[0];
			switch (command) {
			case "show":
				Show();
				break;
			case "add":
				Add(commandRest[1]);
				break;
			case "discontinueBook":
				Discontinue(commandRest[1]);
				break;
			case "discontinueAuthor":
				Discontinue(commandRest[1]);
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
			foreach (var project in Books) {
				foreach (var Book in Books) {
					Console.WriteLine("[{0}] {1}: {2} {3}", Book.Id, Book.Title, Book.Author, (Book.IsDiscontinued ? "(discontinued)" : ""));
				}
				Console.WriteLine();
			}
		}

		private void Add(string commandLine)
		{
			var subcommandRest = commandLine.Split(" ".ToCharArray());
			AddBook(subcommandRest[0], subcommandRest[1], subcommandRest[2], subcommandRest[3]);
		}

		private void AddBook(string book, string author, string category, string description)
		{
			if (Books.Any(b => b.Title == book))
			{
				Console.WriteLine("Duplicate book with the name \"{0}\".", book);
				return;
			}

			int categoryId = 0;
			
			if (category == "Fiction")
				categoryId = 1;
			if(category == "Romance")
				categoryId = 2;
			if(category == "Science Fiction")
				categoryId = 3;
			if(category == "Fantasy")
				categoryId = 4;
			if (category == "Mystery")
				categoryId = 5;
			if (category == "Biography")
				categoryId = 6;

			if (categoryId == 0)
			{
				Console.Error.WriteLine("Invalid category");
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

		private void Discontinue(string id)
		{
			if (Books.Any(book => book.Author.Equals(id)))
			{
				DiscontinueByAuthor(id);	
			}
			else
			{
				DiscontinueBook(id);	
			}
		}
		
		private void DiscontinueBook(string idString)
		{
			int id = int.Parse(idString);
			var identifiedBook = Books
				.Select(book => book)
				.Where(b => b.Id == id)
				.FirstOrDefault();
			

			identifiedBook.IsDiscontinued = true;
		}
		
		public void DiscontinueByAuthor(string authorName)
		{
			bool foundAny = false;

			for (int j = 0; j <Books.Count; j++)
			{
				Book currentBook = Books[j];

				if (currentBook.Author.Trim().ToLower() == authorName.Trim().ToLower())
				{
					if (currentBook.IsDiscontinued == false)
					{
						Books[j].IsDiscontinued = true;
						foundAny = true;

						Console.WriteLine("Discontinued: '" + currentBook.Title + "' from author " + currentBook.Author
						                  + " (ID: {currentBook.Id})");
					}
				}
			}
			
			if (foundAny == false)
			{
				Console.WriteLine($"Could not find any active books by author: {authorName}");
			}
		}

		private void Help()
		{
			Console.WriteLine("Commands:");
			Console.WriteLine("  show");
			Console.WriteLine("  add project <project name>");
			Console.WriteLine("  add Book <project name> <Book description>");
			Console.WriteLine("  check <Book ID>");
			Console.WriteLine("  uncheck <Book ID>");
			Console.WriteLine();
		}

		private void Error(string command)
		{
			Console.WriteLine("I don't know what the command \"{0}\" is.", command);
		}

		private long NextId()
		{
			return ++lastId;
		}
	}
}
