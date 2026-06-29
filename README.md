# Bookstore

Simple .NET console application to manage a small in-memory catalog of books.

## Project Scope

This project provides a command-line interface where you can:

- Add books
- List books
- Mark books as discontinued (by book id or author)

It is intentionally small and currently keeps data only in memory.

## Technology Stack

- C#
- .NET 10 (`net10.0`)
- Console application (`Microsoft.NET.Sdk`)
- Docker multi-stage build
- Docker Compose service definition

## Repository Structure

- `Bookstore/Program.cs`: app entry point and command loop
- `Bookstore/Book.cs`: book domain model
- `Bookstore/IConsole.cs`: console abstraction
- `Bookstore/RealConsole.cs`: concrete console implementation
- `Bookstore/Bookstore.csproj`: project settings and target framework
- `Bookstore/Dockerfile`: container build and runtime image
- `Bookstore.sln`: solution file
- `compose.yaml`: compose service for the app image

## How It Works

At startup, the app enters a REPL-style loop:

1. Writes prompt (`> `)
2. Reads one command line
3. Executes supported command
4. Repeats until `quit`

Books are stored in an in-memory list and assigned incremental ids.

## Prerequisites

- .NET SDK 10.x
- (Optional) Docker and Docker Compose

## Build

From repository root:

```bash
dotnet build Bookstore.sln
```

## Run

From repository root:

```bash
dotnet run --project Bookstore/Bookstore.csproj
```

You will see a prompt:

```text
>
```

Type `quit` to exit.

## Command Reference

### Implemented command names

- `show`
- `showAuthors`
- `add <title> <author> <category> <description>`
- `addAuthor <id> <name> <bornDateYYYY-MM-DD> [awardsCommaSeparated]`
- `discontinueBook <id>`
- `discontinueAuthor <author>`
- `help`
- `quit`

### Examples

```text
add Dune Herbert Fiction Classic
addAuthor 12 Herbert 1980-11-02 Nobel,Booker
showAuthors
show
discontinueBook 1
discontinueAuthor Herbert
quit
```

Notes:

- Category must match one of: `Fiction`, `Romance`, `Science Fiction`, `Fantasy`, `Mystery`, `Biography`
- Category and description can contain spaces (for example: `Science Fiction` and `Classic space saga`)
- Current parser treats the first token as title and second token as author
- If a book author is not found, the app automatically uses fallback author `Unknown Author` (id `0`)

## Docker

Build image via compose:

```bash
docker compose build
```

Run service:

```bash
docker compose up
```

Or build directly with Dockerfile:

```bash
docker build -f Bookstore/Dockerfile -t bookstore .
docker run --rm -it bookstore
```

## Current Limitations and Known Issues

- Data is not persisted; restarting the process loses all books
- Title and author are currently single-token inputs in `add`
- Command parsing is still basic and not quote-aware

## Future Improvements

- Persist books to file or database
- Improve command parser (quoted title/author and richer validation)
- Add automated tests
- Add data import/export and search/filter commands
