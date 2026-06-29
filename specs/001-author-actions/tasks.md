# Tasks: Author Actions

**Input**: Design documents from `/specs/001-author-actions/`

**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: No explicit TDD/testing-first request in spec. No standalone test-creation tasks are included.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Every task includes exact file path(s)

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare domain and repository scaffolding for author capabilities.

- [X] T001 Create `Author` domain model with `Id`, `Name`, `BornDate`, and `Awards` in `Bookstore/Author.cs`
- [X] T002 Create author repository contract for create/list/find operations in `Bookstore/IAuthorRepository.cs`
- [X] T003 Create in-memory author repository skeleton in `Bookstore/InMemoryAuthorRepository.cs`
- [X] T004 Register `IAuthorRepository` and `InMemoryAuthorRepository` in DI container setup in `Bookstore/Program.cs`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Introduce core author-book relationship and fallback mechanics required by all stories.

**⚠️ CRITICAL**: No user story work should be finalized before this phase completes.

- [X] T005 Add `AuthorId` to `Book` model while keeping backward-compatible fields in `Bookstore/Book.cs`
- [X] T006 Update book repository add contract to accept `authorId` in `Bookstore/IBookRepository.cs`
- [X] T007 Update in-memory book repository implementation for new `authorId` field in `Bookstore/InMemoryBookRepository.cs`
- [X] T008 Implement shared fallback helper to create/reuse Unknown Author (`Id=0`) in `Bookstore/InMemoryAuthorRepository.cs`
- [X] T009 Wire `Bookstore` constructor/state to keep both repositories available in `Bookstore/Program.cs`

**Checkpoint**: Foundation complete. User stories can be implemented.

---

## Phase 3: User Story 1 - Create Author Records (Priority: P1) 🎯 MVP

**Goal**: Add `addAuthor` command to create valid authors with required fields and validation.

**Independent Test**: From CLI, run valid and invalid `addAuthor` commands and verify stored/rejected outcomes without relying on other stories.

### Implementation for User Story 1

- [X] T010 [US1] Add `addAuthor` command parsing branch with usage guidance in `Bookstore/Program.cs`
- [X] T011 [US1] Implement strict `YYYY-MM-DD` date parsing/validation for `addAuthor` in `Bookstore/Program.cs`
- [X] T012 [US1] Enforce duplicate ID rejection and reserved `Id=0` rule in `Bookstore/InMemoryAuthorRepository.cs`
- [X] T013 [US1] Implement author creation flow and success/error output for `addAuthor` in `Bookstore/Program.cs`

**Checkpoint**: User Story 1 should be fully functional and independently testable.

---

## Phase 4: User Story 2 - Show Authors with Names (Priority: P2)

**Goal**: Add `showAuthors` command that lists authors including author names.

**Independent Test**: Create one or more authors, run `showAuthors`, and verify output lines include names; verify empty-state message when none exist.

### Implementation for User Story 2

- [X] T014 [US2] Add `showAuthors` command parsing and handler routing in `Bookstore/Program.cs`
- [X] T015 [US2] Implement author listing retrieval and deterministic ordering in `Bookstore/InMemoryAuthorRepository.cs`
- [X] T016 [US2] Implement `showAuthors` output formatting with mandatory author name in `Bookstore/Program.cs`
- [X] T017 [US2] Update help command output to include `addAuthor` and `showAuthors` in `Bookstore/Program.cs`

**Checkpoint**: User Story 2 should be independently functional and testable.

---

## Phase 5: User Story 3 - Resolve Missing Book Author Reference (Priority: P3)

**Goal**: Ensure missing book author references resolve to Unknown Author (`Id=0`) and `show` includes author name via author reference.

**Independent Test**: Execute book flow with non-existent author reference and verify Unknown Author creation/reuse and successful `show` output with author name.

### Implementation for User Story 3

- [X] T018 [US3] Update add-book flow to resolve/assign `AuthorId` through author repository fallback in `Bookstore/Program.cs`
- [X] T019 [US3] Implement missing-author resolution contract (create/reuse Unknown Author) for book operations in `Bookstore/InMemoryAuthorRepository.cs`
- [X] T020 [US3] Update `show` command to render author name from `AuthorId` lookup in `Bookstore/Program.cs`
- [X] T021 [US3] Update discontinue-by-author logic to match author identity via repository-backed names in `Bookstore/Program.cs`

**Checkpoint**: User Story 3 should be independently functional and testable.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Final consistency, docs alignment, and end-to-end validation.

- [X] T022 [P] Update command reference and examples for author commands in `README.md`
- [X] T023 [P] Update feature progress/release notes for author actions in `CHANGELOG.md`
- [X] T024 Run quickstart validation scenarios and align expected outputs in `specs/001-author-actions/quickstart.md`
- [X] T025 Record implementation follow-up notes in `docs/bookstore-review.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: no dependencies.
- **Foundational (Phase 2)**: depends on Setup and blocks all user stories.
- **User Stories (Phase 3-5)**: depend on Foundational completion.
- **Polish (Phase 6)**: depends on completion of desired user stories.

### User Story Dependencies

- **US1 (P1)**: starts after Foundational; no dependency on other stories.
- **US2 (P2)**: starts after Foundational; independent from US1 for core listing behavior.
- **US3 (P3)**: starts after Foundational; uses shared repository contracts and fallback helper.

### Within Each User Story

- Parsing and contract wiring before handler logic.
- Handler logic before output/help updates.
- Behavior coverage after implementation is stable.

### Parallel Opportunities

- `T022` and `T023` can run in parallel (different files).
- Cross-story implementation can proceed in parallel after Phase 2 with separate contributors.

---

## Parallel Example: User Story 1

```bash
# Parallelizable after foundational phase with multiple contributors:
Task: "T010 Add addAuthor command parsing branch in Bookstore/Program.cs"
Task: "T012 Enforce duplicate/reserved ID rules in Bookstore/InMemoryAuthorRepository.cs"
```

## Parallel Example: User Story 2

```bash
# Parallelizable story-level work after foundational phase:
Task: "T016 Implement author listing retrieval in Bookstore/InMemoryAuthorRepository.cs"
Task: "T015 Add showAuthors parser routing in Bookstore/Program.cs"
```

## Parallel Example: User Story 3

```bash
# Parallelizable story-level work after foundational phase:
Task: "T019 Implement unknown-author fallback contract in Bookstore/InMemoryAuthorRepository.cs"
Task: "T021 Update discontinueAuthor logic to use repository-backed names in Bookstore/Program.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1 (Setup).
2. Complete Phase 2 (Foundational).
3. Complete Phase 3 (US1).
4. Validate US1 independently via CLI and behavior tests.

### Incremental Delivery

1. Deliver US1 (`addAuthor`) as MVP.
2. Deliver US2 (`showAuthors`) next for visibility.
3. Deliver US3 fallback integration to harden book-author references.
4. Finish with polish, docs, and full validation.

### Parallel Team Strategy

1. Team completes Phase 1-2 together.
2. Then split by story:
   - Dev A: US1 tasks (`T010-T013`)
   - Dev B: US2 tasks (`T014-T017`)
   - Dev C: US3 tasks (`T018-T021`)

---

## Notes

- All tasks follow checklist format and include file paths.
- Story labels are present only for user story phases.
- Task IDs are sequential (`T001` through `T025`).
- Suggested MVP scope: through Phase 3 (US1) only.
