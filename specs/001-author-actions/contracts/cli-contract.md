# CLI Contract: Author Actions

## Scope

Defines user-facing command contract for author creation and author listing, plus fallback behavior coupling with book-author references.

## Command: addAuthor

- Syntax:
  - `addAuthor <id> <name> <bornDateYYYY-MM-DD> [awardsCommaSeparated]`
- Inputs:
  - `id`: integer, must be greater than 0 for user-created authors.
  - `name`: non-empty token/string (current parser constraints apply).
  - `bornDateYYYY-MM-DD`: strict date string.
  - `awardsCommaSeparated`: optional comma-separated values (empty means no awards).
- Success behavior:
  - Creates author record with provided fields.
  - Returns confirmation message including author id and name.
- Error behavior:
  - Duplicate id -> reject with duplicate-id message.
  - `id = 0` -> reject with reserved-id message.
  - Invalid date format or impossible date -> reject with date validation message.
  - Missing required args -> print usage.

## Command: showAuthors

- Syntax:
  - `showAuthors`
- Success behavior:
  - Lists authors in deterministic order (implementation-defined, preferably id ascending).
  - Each line includes at minimum author id and author name.
- Empty behavior:
  - If no authors registered, print explicit empty-state message.

## Fallback Contract for Book Reference

- Rule:
  - When a book operation references non-existent author id, application must ensure Unknown Author exists and resolve to `AuthorId = 0`.
- Canonical fallback:
  - `Id = 0`
  - `Name = "Unknown Author"`
- Expected outcome:
  - Book flow continues without failure due to missing author.
  - Subsequent missing references reuse same fallback author.

## Backward Compatibility Notes

- Existing commands (`show`, `add`, `discontinueBook`, `discontinueAuthor`, `help`, `quit`) remain available.
- `show` output continues to include author name, now resolved from author reference.
