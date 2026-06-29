# Quickstart: Author Actions Validation

## Prerequisites

- .NET SDK 10.x installed.
- Repository root as working directory.

## Setup

1. Build solution:

```bash
dotnet build Bookstore.sln
```

2. (Optional) run automated tests:

```bash
dotnet test Bookstore.sln
```

## Scenario 1: Create author and list authors

Run app:

```bash
dotnet run --project Bookstore/Bookstore.csproj
```

In REPL, execute:

```text
addAuthor 12 Jane_Doe 1980-11-02 National Prize,Booker
showAuthors
quit
```

Expected outcomes:

- Author creation confirms id and name.
- `showAuthors` output includes `Jane_Doe` (author name visible).
- Example output includes:
	- `Author created: 12 - Jane_Doe`
	- `[12] Jane_Doe | Born: 1980-11-02 | Awards: National Prize,Booker`

## Scenario 2: Date validation rejects invalid date

In REPL:

```text
addAuthor 13 BadDate 2026-02-30 SomeAward
quit
```

Expected outcomes:

- Command rejected with invalid-date message.
- No author created for id `13`.

## Scenario 3: Reserved id 0 cannot be user-created

In REPL:

```text
addAuthor 0 CustomUnknown 1990-01-01 Award
quit
```

Expected outcomes:

- Command rejected with reserved-id message.

## Scenario 4: Missing author reference resolves to Unknown Author

Run app and execute:

```text
add Dune MissingAuthor Fiction Classic
show
showAuthors
quit
```

Expected outcomes:

- Unknown Author is auto-created or reused with `Id = 0` and `Name = Unknown Author`.
- Book flow completes successfully.
- `show` output includes author name for book entries (resolved via author reference).
- `showAuthors` includes single fallback row for unknown author.

## Traceability

- Data model details: [data-model.md](./data-model.md)
- CLI contract details: [contracts/cli-contract.md](./contracts/cli-contract.md)
- Feature spec: [spec.md](./spec.md)
