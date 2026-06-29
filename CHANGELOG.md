# Changelog

All notable changes to this project are documented in this file.

## [2026-06-29]

### Added

- Author management actions:
	- Added `addAuthor <id> <name> <bornDateYYYY-MM-DD> [awardsCommaSeparated]`.
	- Added `showAuthors` with id/name/born date/awards output.
	- Added `Author` model and `IAuthorRepository` + `InMemoryAuthorRepository`.
	- Added unknown-author fallback identity (`Id = 0`, `Name = "Unknown Author"`).
	- Where:
		- [Bookstore/Author.cs](Bookstore/Author.cs)
		- [Bookstore/IAuthorRepository.cs](Bookstore/IAuthorRepository.cs)
		- [Bookstore/InMemoryAuthorRepository.cs](Bookstore/InMemoryAuthorRepository.cs)
		- [Bookstore/Program.cs](Bookstore/Program.cs)

### Changed

- Book-author relation now includes `AuthorId` reference:
	- Extended `Book` with `AuthorId`.
	- Updated `IBookRepository`/`InMemoryBookRepository` add contract to store `AuthorId` and author name.
	- `show` now resolves displayed author name from author repository and ensures fallback for missing references.
	- `discontinueAuthor` now matches via repository-backed author identity.
	- Help output includes new author commands.
	- Where:
		- [Bookstore/Book.cs](Bookstore/Book.cs)
		- [Bookstore/IBookRepository.cs](Bookstore/IBookRepository.cs)
		- [Bookstore/InMemoryBookRepository.cs](Bookstore/InMemoryBookRepository.cs)
		- [Bookstore/Program.cs](Bookstore/Program.cs)

### Tests

- Added/updated behavior tests for:
	- `addAuthor` success, duplicate id, invalid date, and reserved id 0.
	- `showAuthors` empty and populated output.
	- Missing-book-author fallback creation and reuse.
	- Existing command behavior alignment after author reference changes.
	- Where:
		- [tests/Bookstore.Tests/UnitTest1.cs](tests/Bookstore.Tests/UnitTest1.cs)

### Changed

- Command handling made safe (no index crashes):
	- Added argument-length validation before reading command parts.
	- Added usage messages for invalid command forms.
	- Empty input is ignored safely.
	- Where:
		- [Bookstore/Program.cs](Bookstore/Program.cs#L50)
		- [Bookstore/Program.cs](Bookstore/Program.cs#L60)

- `discontinueBook` made crash-proof:
	- Replaced unsafe parse path with `long.TryParse`.
	- Added not-found handling for unknown ids.
	- Added already-discontinued handling.
	- Where:
		- [Bookstore/Program.cs](Bookstore/Program.cs#L168)

- `show` output corrected:
	- Removed duplicate nested iteration.
	- Added explicit empty-catalog message.
	- Where:
		- [Bookstore/Program.cs](Bookstore/Program.cs#L91)

- Console abstraction used consistently:
	- Replaced direct `Console.*` output in command flow with `IConsole` output path.
	- Unified error and info output in command handling.
	- Where:
		- [Bookstore/Program.cs](Bookstore/Program.cs#L44)

- Author-discontinue output fixed:
	- Corrected id rendering in output message.
	- Kept case-insensitive author matching and added trim handling.
	- Where:
		- [Bookstore/Program.cs](Bookstore/Program.cs#L197)

- Help text aligned with real commands:
	- Updated help to match implemented command names and signatures.
	- Where:
		- [Bookstore/Program.cs](Bookstore/Program.cs#L220)

- Category parsing and mapping improved:
	- Reused category dictionary for matching instead of hardcoded `if` chain.
	- Added case-insensitive matching.
	- Supports multi-word category plus multi-word description.
	- Where:
		- [Bookstore/Program.cs](Bookstore/Program.cs#L111)
		- [Bookstore/Program.cs](Bookstore/Program.cs#L143)

- Nullability improvements:
	- Added `required` to `Book` string properties.
	- `ReadLine()` now returns non-null string fallback.
	- Where:
		- [Bookstore/Book.cs](Bookstore/Book.cs#L6)
		- [Bookstore/RealConsole.cs](Bookstore/RealConsole.cs#L7)

- README synchronized with implementation:
	- Updated command signatures (`discontinueBook <id>`, `discontinueAuthor <author>`).
	- Updated notes to reflect parser behavior.
	- Removed stale issues that were already fixed.
	- Where:
		- [README.md](README.md#L63)
		- [README.md](README.md#L83)
		- [README.md](README.md#L105)

### Added

- Phase 1: Testing foundation and automation:
	- Added xUnit test project and solution wiring.
	- Added behavior tests covering quit, show-empty, add/show, duplicate title, discontinue by id (invalid + success), discontinue by author, and help output.
	- Added root `Makefile` automation targets:
		- `build`
		- `build-docker`
		- `test`
		- `test-coverage`
		- `test-docker`
		- `coverage-docker`
	- Added Docker-based test and coverage execution using .NET SDK container.
	- Where:
		- [tests/Bookstore.Tests/UnitTest1.cs](tests/Bookstore.Tests/UnitTest1.cs)
		- [tests/Bookstore.Tests/Bookstore.Tests.csproj](tests/Bookstore.Tests/Bookstore.Tests.csproj)
		- [Bookstore.sln](Bookstore.sln)
		- [Makefile](Makefile)

- Phase 2: Typed command extraction from command loop:
	- Extracted command parsing into `CommandParser.Parse(string)`.
	- Introduced typed command objects:
		- `ShowCommand`
		- `AddCommand`
		- `DiscontinueBookCommand`
		- `DiscontinueAuthorCommand`
		- `HelpCommand`
		- `UnknownCommand`
		- `InvalidUsageCommand`
		- `EmptyCommand`
	- Added centralized dispatch via `Handle(ICommand)` and moved execution into dedicated handlers.
	- Preserved user-facing behavior and messages while reducing parsing/handling coupling.
	- Where:
		- [Bookstore/Program.cs](Bookstore/Program.cs)

- Phase 3: Repository abstraction for storage and identity:
	- Added repository contract `IBookRepository` for book storage operations.
	- Added `InMemoryBookRepository` implementation managing:
		- In-memory collection lifecycle
		- ID generation (`NextId`)
		- Lookup and duplicate-title checks
	- Refactored `Bookstore` to depend on `IBookRepository` instead of owning direct storage state.
	- Added constructor overload for repository injection while preserving default runtime behavior.
	- Moved storage and id concerns out of command host flow in `Program.cs`.
	- Where:
		- [Bookstore/IBookRepository.cs](Bookstore/IBookRepository.cs)
		- [Bookstore/InMemoryBookRepository.cs](Bookstore/InMemoryBookRepository.cs)
		- [Bookstore/Program.cs](Bookstore/Program.cs)

- Phase 4: Generic Host + dependency injection container wiring:
	- Replaced direct app construction in `Main` with `Host.CreateDefaultBuilder(args)`.
	- Registered services through container wiring:
		- `IConsole` -> `RealConsole`
		- `IBookRepository` -> `InMemoryBookRepository`
		- `Bookstore` application service
	- Resolved `Bookstore` from host service provider and executed command loop.
	- Added required hosting and DI package references in project file.
	- Where:
		- [Bookstore/Program.cs](Bookstore/Program.cs)
		- [Bookstore/Bookstore.csproj](Bookstore/Bookstore.csproj)

### Validation

- The updated behavior was validated with:

```bash
dotnet build Bookstore/Bookstore.csproj -nologo
printf 'show\nadd Dune Herbert Fiction Classic scifi\nshow\ndiscontinueBook 1\nshow\ndiscontinueAuthor Herbert\nhelp\nquit\n' | dotnet run --project Bookstore/Bookstore.csproj
dotnet test Bookstore.sln -nologo
make test
make test-coverage
make test-docker
make coverage-docker
make build-docker
```

- Phase 3 verification:

```bash
dotnet build Bookstore.sln -nologo
dotnet test Bookstore.sln -nologo
```

- Phase 4 verification:

```bash
dotnet build Bookstore.sln -nologo
dotnet test Bookstore.sln -nologo
```

### Open

- Persistence remains in-memory only (`InMemoryBookRepository`); no durable backend yet.
- Title and author parsing is still single-token (no quoted parsing yet).
