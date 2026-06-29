# Data Model: Author Actions

## Entity: Author

- Purpose: Canonical author profile managed independently from books.
- Fields:
  - `Id` (int): Unique author identifier. `0` reserved for fallback Unknown Author.
  - `Name` (string): Author display name. Required, non-empty.
  - `BornDate` (date): Required, stored in `YYYY-MM-DD` semantic format.
  - `Awards` (list<string>): Optional/empty-allowed list of award labels.
- Validation Rules:
  - `Id` must be unique for non-fallback authors.
  - Explicit user creation with `Id = 0` is not allowed.
  - `Name` must be non-whitespace.
  - `BornDate` must parse as valid calendar date in strict `YYYY-MM-DD` format.

## Entity: Book

- Purpose: Catalog item for bookstore inventory and lifecycle operations.
- Existing fields (retained): `Id`, `Title`, `CategoryId`, `Description`, `IsDiscontinued`.
- Field evolution for this feature:
  - Add `AuthorId` (int): Foreign-key-like reference to `Author.Id`.
  - Keep display requirement: show output includes author name (resolved from author repository via `AuthorId`).

## Derived Entity: Unknown Author

- Purpose: Fallback author identity for missing references.
- Canonical values:
  - `Id = 0`
  - `Name = "Unknown Author"`
  - `BornDate` implementation default (application-chosen, non-user-entered)
  - `Awards = []`
- Invariants:
  - Must be created automatically when first missing reference occurs.
  - Must be reused for all subsequent missing-author resolutions.

## Relationships

- `Book.AuthorId` -> `Author.Id` (many-to-one).
- Multiple books may point to one author.
- Missing `Author.Id` reference resolves to Unknown Author (`Id = 0`).

## State Transitions

### Author

1. Non-existent -> Created via `addAuthor` (for ids > 0).
2. Non-existent fallback -> Auto-created as Unknown Author when missing author reference detected.
3. Existing -> Listed via `showAuthors`.

### Book Author Reference Resolution

1. Book operation receives/uses `AuthorId`.
2. If `AuthorId` exists: keep as-is.
3. If `AuthorId` missing: ensure Unknown Author exists and set/use `AuthorId = 0`.
