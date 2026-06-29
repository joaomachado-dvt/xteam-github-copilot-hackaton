# Implementation Plan: Author Actions

**Branch**: `[main]` | **Date**: 2026-06-29 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/001-author-actions/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Add two CLI actions for author management (`addAuthor` and `showAuthors`), introduce Author domain/repository support, and ensure book-author references resolve through `AuthorId` with automatic fallback to `Unknown Author` (`Id = 0`) when referenced author is missing.

## Technical Context

**Language/Version**: C# with .NET 10 (`net10.0`)

**Primary Dependencies**: `Microsoft.Extensions.Hosting`, `Microsoft.Extensions.DependencyInjection`, xUnit test stack

**Storage**: In-memory repositories (`InMemoryBookRepository`, new `InMemoryAuthorRepository`)

**Testing**: xUnit (`tests/Bookstore.Tests`)

**Target Platform**: Cross-platform CLI (local shell, Docker Linux container)

**Project Type**: Console CLI application

**Performance Goals**: Command handling under interactive CLI expectations (single-command execution perceived as immediate)

**Constraints**: Preserve existing command behavior, keep in-memory model, avoid persistence scope expansion

**Scale/Scope**: Single-process, small catalog use case (dozens to low thousands of entries)

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

Current constitution file is a template with no enforceable project rules. No blocking gates detected.

- Gate status (pre-research): PASS
- Risks: None from constitution governance; rely on repository conventions and spec constraints

## Project Structure

### Documentation (this feature)

```text
specs/001-author-actions/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── cli-contract.md
└── tasks.md             # Created later by /speckit.tasks
```

### Source Code (repository root)

```text
Bookstore/
├── Program.cs
├── Book.cs
├── IBookRepository.cs
├── InMemoryBookRepository.cs
├── IConsole.cs
└── RealConsole.cs

tests/
└── Bookstore.Tests/
    └── UnitTest1.cs
```

**Structure Decision**: Keep single-project CLI structure. Extend current domain/repository style with new author model + repository files under `Bookstore/`, and expand behavior tests in `tests/Bookstore.Tests/UnitTest1.cs`.

## Complexity Tracking

No constitution violations requiring justification.

## Phase 0 Research Output

Research decisions documented in [research.md](./research.md).

## Phase 1 Design Output

Design artifacts documented in:

- [data-model.md](./data-model.md)
- [contracts/cli-contract.md](./contracts/cli-contract.md)
- [quickstart.md](./quickstart.md)

## Constitution Check (Post-Design)

Post-design review complete. No enforceable constitution rules violated.

- Gate status (post-design): PASS
