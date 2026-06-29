# Bookstore Modernization Analysis

Date: 2026-06-29

## Context

Current application is functional but still monolithic in structure. Most command handling, parsing, business rules, and state mutation are concentrated in a single file:
- [Bookstore/Program.cs](Bookstore/Program.cs)

Model and console abstractions are simple and serviceable:
- [Bookstore/Book.cs](Bookstore/Book.cs)
- [Bookstore/IConsole.cs](Bookstore/IConsole.cs)
- [Bookstore/RealConsole.cs](Bookstore/RealConsole.cs)

The recommendations below are based on available .NET skill guidance:
- dotnet-backend-patterns
- dotnet-best-practices

## Modernization Recommendations

### 1. Separate concerns into layers (highest priority)

Problem:
- Command parsing, validation, business logic, and data storage are tightly coupled in [Bookstore/Program.cs](Bookstore/Program.cs).

Recommendation:
- Split into layered modules:
  - Domain: entities/value objects and domain rules
  - Application: command/query handlers and orchestration
  - Infrastructure: repository implementations (in-memory now, file/db later)
  - UI/Console: parser + presentation only

Expected outcome:
- Better maintainability, easier tests, lower risk when adding features.

### 2. Introduce repository abstraction

Problem:
- Storage and id generation are embedded in app runner.

Recommendation:
- Add `IBookRepository` interface and `InMemoryBookRepository` implementation.
- Move list and identity concerns out of command runner.

Expected outcome:
- Future persistence swap (JSON/SQLite/Postgres) without rewriting business logic.

### 3. Replace string parsing with typed commands

Problem:
- Parser still token-position driven and somewhat brittle.

Recommendation:
- Parse input into typed command records:
  - `AddBookCommand`
  - `DiscontinueBookCommand`
  - `DiscontinueAuthorCommand`
  - `ShowBooksQuery`
- Validate command shape before execution.

Expected outcome:
- Cleaner error handling and easier extension.

### 4. Adopt Generic Host + DI for console app

Problem:
- Manual object wiring in `Main`.

Recommendation:
- Use `Host.CreateDefaultBuilder(args)` and register services/handlers/repositories in DI.

Expected outcome:
- Modern app composition, standardized configuration/logging.

### 5. Add structured logging

Problem:
- User output and diagnostics are mixed.

Recommendation:
- Keep user-facing messages through console abstraction.
- Add `ILogger<T>` in handlers/services for diagnostics.

Expected outcome:
- Better observability with no UX regression.

### 6. Strengthen domain model invariants

Problem:
- [Bookstore/Book.cs](Bookstore/Book.cs) is mutable DTO style.

Recommendation:
- Move behavior into domain methods (for example, `Discontinue()`).
- Enforce invariants in constructors/factories.
- Consider category enum/value object instead of raw strings in flow.

Expected outcome:
- Fewer invalid states and clearer domain behavior.

### 7. Improve namespace and documentation consistency

Problem:
- Namespace naming is mixed (`Bookstore` and `Tasks`).

Recommendation:
- Align namespaces around consistent app structure.
- Add XML docs for public APIs as project grows.

Expected outcome:
- Better long-term readability and IDE support.

## Testing Strategy (Recommended Baseline)

Create two test projects:
- `Bookstore.UnitTests`
- `Bookstore.IntegrationTests`

Use:
- xUnit
- FluentAssertions
- Moq or NSubstitute

Core tests:
1. Add book happy path + duplicate title
2. Invalid category handling
3. Discontinue by id: invalid id, missing id, already discontinued, success
4. Discontinue by author: none found + success
5. Show command: empty and populated catalog
6. Help command smoke output

## Docker + Makefile for Test and Coverage

Yes, it is possible and recommended.

Suggested Makefile targets:
- `make build`
- `make test`
- `make test-coverage`
- `make test-docker`
- `make coverage-docker`

Concept:
- Run tests in .NET SDK container with mounted workspace.
- Export coverage artifacts to host (`TestResults/coverage.cobertura.xml`).
- Keep runtime app image separate from test image.

## Suggested Refactor Phases

Phase 1:
- Add test project and baseline tests around current behavior.

Phase 2:
- Extract parser + handlers while preserving behavior.

Phase 3:
- Introduce repository abstraction and move in-memory storage.

Phase 4:
- Add Generic Host, DI, and structured logging.

Phase 5:
- Add Docker-based test/coverage workflows and Makefile.

## Risks and Notes

- Biggest risk is large rewrite without tests; prefer incremental extraction.
- Preserve current command behavior while refactoring to avoid regressions.
- Keep docs synchronized as commands evolve.
