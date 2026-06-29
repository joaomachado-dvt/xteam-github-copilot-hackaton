# Research: Author Actions

## Decision 1: Add dedicated Author domain model and repository

- Decision: Introduce `Author` entity and `IAuthorRepository`/`InMemoryAuthorRepository` instead of embedding author metadata in existing book repository.
- Rationale: Spec requires standalone author actions (`addAuthor`, `showAuthors`) and stable fallback identity (`Unknown Author` with id 0). Dedicated repository keeps author lifecycle coherent and testable.
- Alternatives considered:
  - Keep authors as derived data from books only: rejected because authors must be created independently of books.
  - Store authors as ad hoc dictionary in `Program.cs`: rejected due to maintainability and testability concerns.

## Decision 2: Represent book-author relation by `AuthorId` and resolve display name through repository

- Decision: Extend `Book` model to include `AuthorId` while preserving console output requirement that show output includes author name.
- Rationale: Spec explicitly requires book reference by Author ID and fallback behavior for missing references.
- Alternatives considered:
  - Continue storing only author name on `Book`: rejected because no stable referential link to author entity.
  - Replace all book operations with author-name-based lookup only: rejected because conflicts with required ID-based relationship.

## Decision 3: Fallback unknown-author policy centralized in repository/service helper

- Decision: Implement a reusable path that ensures `AuthorId = 0`, `Name = "Unknown Author"` exists and is reused whenever a referenced author id is missing.
- Rationale: Spec requires deterministic fallback creation/reuse across all book-author resolution points.
- Alternatives considered:
  - Inline fallback creation each command handler: rejected because duplicates logic and risks inconsistent behavior.
  - Fail command on missing author: rejected by spec requirement FR-008.

## Decision 4: CLI command shape for author actions

- Decision: Add commands:
  - `addAuthor <id> <name> <bornDate(YYYY-MM-DD)> [awardsCommaSeparated]`
  - `showAuthors`
- Rationale: Fits existing REPL parser pattern while preserving mandatory fields and allowing optional empty awards list.
- Alternatives considered:
  - Multi-word quoted parser redesign: deferred, out of scope for this feature.
  - Interactive prompt mode for author creation: rejected to keep behavior consistent with existing command style.

## Decision 5: Validation strategy for born date and reserved ID

- Decision: Validate born date with strict `yyyy-MM-dd` parsing and reject invalid calendar values; reject explicit `addAuthor` use of id 0.
- Rationale: Directly implements spec edge cases and FR-002/FR-004.
- Alternatives considered:
  - Permit flexible date formats: rejected due to explicit format requirement.
  - Allow custom author at id 0: rejected because id 0 is reserved fallback identity.

## Decision 6: Testing approach with existing xUnit behavior tests

- Decision: Add behavior-level tests in `tests/Bookstore.Tests/UnitTest1.cs` for create/show authors and unknown-author fallback on book flows.
- Rationale: Repository already uses REPL-style behavior tests; extending same style gives regression safety with minimal tooling changes.
- Alternatives considered:
  - Introduce new test project structure now: deferred; unnecessary for scoped feature delivery.
