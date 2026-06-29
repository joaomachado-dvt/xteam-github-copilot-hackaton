# Changelog

All notable changes to this project are documented in this file.

## [2026-06-29]

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

### Validation

- The updated behavior was validated with:

```bash
dotnet build Bookstore/Bookstore.csproj -nologo
printf 'show\nadd Dune Herbert Fiction Classic scifi\nshow\ndiscontinueBook 1\nshow\ndiscontinueAuthor Herbert\nhelp\nquit\n' | dotnet run --project Bookstore/Bookstore.csproj
```

### Open

- Automated tests are not added yet.
- Data persistence is still in-memory only.
- Title and author parsing is still single-token (no quoted parsing yet).
