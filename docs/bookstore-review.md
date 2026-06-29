# Bookstore Folder Review

Date: 2026-06-29

Scope: `Bookstore/Program.cs`, `Bookstore/Book.cs`, `Bookstore/IConsole.cs`, `Bookstore/RealConsole.cs`, `Bookstore/Bookstore.csproj`

## Findings (Ordered by Severity)

1. High - Crash risk on incomplete commands
- Location: `Bookstore/Program.cs`
- Problem: command arguments were indexed without length checks.
- Fix: validate command argument counts before parsing.

2. High - Crash risk on invalid/non-existent id
- Location: `Bookstore/Program.cs`
- Problem: `int.Parse` and null dereference could crash `discontinueBook`.
- Fix: use `TryParse`, null checks, and user-facing errors.

3. High - Duplicate output in `show`
- Location: `Bookstore/Program.cs`
- Problem: nested loop printed each book multiple times.
- Fix: use a single loop and add empty-catalog message.

4. Medium - Console abstraction bypassed
- Location: `Bookstore/Program.cs`
- Problem: direct `Console.*` calls made behavior less testable.
- Fix: route output through `IConsole`.

5. Medium - Incorrect id interpolation in discontinue-by-author message
- Location: `Bookstore/Program.cs`
- Problem: output had literal `{currentBook.Id}`.
- Fix: proper formatted output using console formatter.

6. Medium - Help text mismatched implementation
- Location: `Bookstore/Program.cs`
- Problem: help listed old commands unrelated to actual parser.
- Fix: align help content with implemented commands.

7. Low - Nullability issues
- Location: `Bookstore/Book.cs`, `Bookstore/RealConsole.cs`
- Problem: non-nullable string properties and `ReadLine` null path not handled.
- Fix: `required` properties in model and null-safe console read.

8. Low - Category mapping duplication
- Location: `Bookstore/Program.cs`
- Problem: dictionary existed but hardcoded if-chain did mapping.
- Fix: use dictionary-driven matching with case-insensitive lookup.

## Implemented Status

All findings above were implemented in the current working tree and validated with:

```bash
dotnet build Bookstore/Bookstore.csproj -nologo
printf 'show\nadd Dune Herbert Fiction Classic scifi\nshow\ndiscontinueBook 1\nshow\ndiscontinueAuthor Herbert\nhelp\nquit\n' | dotnet run --project Bookstore/Bookstore.csproj
```

## Remaining Improvements

1. Add automated tests for command parsing and edge cases.
2. Consider persistence (file or database) beyond in-memory list.
3. Improve parser UX for quoted args and richer command grammar.
4. Consider exposing categories in help output dynamically from dictionary.
