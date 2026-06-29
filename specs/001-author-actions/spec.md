# Feature Specification: Author Actions

**Feature Branch**: `[001-author-actions]`

**Created**: 2026-06-29

**Status**: Draft

**Input**: User description: "Create two new actions one to create an Author and another to show authors. An Author as ID, Name, Born Date (YYYY-MM-DD), and Awards List. Book as a reference for Author by Author ID, if an author doesn’t exist it should create with ID 0 - Unknown Author. The show command it should include Author Name."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Create Author Records (Priority: P1)

As a bookstore operator, I can create an author with ID, name, born date, and awards so author data is available for book and reporting flows.

**Why this priority**: Without author creation, no valid author catalog exists and downstream book association cannot be relied on.

**Independent Test**: Can be fully tested by creating multiple authors with valid and invalid field combinations and confirming only valid authors are stored and retrievable.

**Acceptance Scenarios**:

1. **Given** no author exists with ID 12, **When** operator creates an author with ID 12, name "Jane Doe", born date "1980-11-02", and awards ["National Prize"], **Then** author is stored with all provided values.
2. **Given** an author already exists with ID 12, **When** operator tries to create another author with ID 12, **Then** system rejects duplicate ID and keeps existing author unchanged.
3. **Given** operator provides born date not in YYYY-MM-DD format, **When** create action is submitted, **Then** system rejects input and explains required date format.

---

### User Story 2 - Show Authors with Names (Priority: P2)

As a bookstore operator, I can list authors and see author names so I can verify catalog contents and identify author references quickly.

**Why this priority**: Visibility of author names is required for day-to-day operations and confirms author setup quality.

**Independent Test**: Can be fully tested by creating authors, running show action, and confirming each displayed record includes author name.

**Acceptance Scenarios**:

1. **Given** authors exist in catalog, **When** operator runs show authors action, **Then** output includes each author name.
2. **Given** no authors exist in catalog, **When** operator runs show authors action, **Then** output clearly indicates no authors are currently registered.

---

### User Story 3 - Resolve Missing Book Author Reference (Priority: P3)

As a bookstore operator, when a book references a non-existent author ID, the system assigns and uses a default Unknown Author record so book operations do not fail on missing author data.

**Why this priority**: Prevents broken book flows caused by missing author references while preserving a consistent fallback behavior.

**Independent Test**: Can be fully tested by creating or updating a book with a non-existent author ID and verifying the Unknown Author (ID 0) record is used.

**Acceptance Scenarios**:

1. **Given** a book is created or processed with author ID 999 and no such author exists, **When** system resolves book author reference, **Then** system creates or uses author ID 0 with name "Unknown Author" and associates the book to that author.
2. **Given** Unknown Author (ID 0) already exists, **When** another missing author reference is processed, **Then** system reuses existing Unknown Author record.

---

### Edge Cases

- What happens when awards list is empty? System accepts author and stores empty awards list.
- What happens when born date is a valid format but impossible date (for example 2026-02-30)? System rejects author creation and reports invalid calendar date.
- What happens when create action tries to use ID 0 for a custom author? System rejects request because ID 0 is reserved for Unknown Author fallback.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST provide an action to create an author with required fields: ID, Name, Born Date (YYYY-MM-DD), and Awards List.
- **FR-002**: System MUST validate Born Date format as YYYY-MM-DD and reject invalid dates.
- **FR-003**: System MUST enforce unique author IDs for non-fallback authors.
- **FR-004**: System MUST reserve author ID 0 for fallback Unknown Author behavior.
- **FR-005**: System MUST provide an action to show authors.
- **FR-006**: System MUST include Author Name in show authors output for every displayed author entry.
- **FR-007**: System MUST support book-to-author reference through Author ID.
- **FR-008**: When a book references an author ID that does not exist, system MUST create or reuse fallback author record with ID 0 and Name "Unknown Author".
- **FR-009**: When fallback author record is created automatically, system MUST ensure it is available for subsequent missing-author references.

### Key Entities *(include if feature involves data)*

- **Author**: Represents a book author with attributes ID, Name, Born Date, and Awards List.
- **Book**: Represents a book record that references an Author via Author ID.
- **Unknown Author**: Reserved fallback author identity with ID 0 and Name "Unknown Author" used when a referenced author does not exist.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of valid author creation requests result in a new author record containing all required fields.
- **SC-002**: 100% of invalid born-date inputs are rejected with a clear validation message.
- **SC-003**: In author listings, 100% of displayed author entries include author name.
- **SC-004**: 100% of book operations with missing author references complete successfully using the Unknown Author fallback, without manual recovery steps.

## Assumptions

- Author management is performed by bookstore staff with permission to manage catalog records.
- Existing book workflows already use or can accept an Author ID reference.
- Awards List is stored as a list of text values and may be empty.
- Fallback Unknown Author uses fixed ID 0 and fixed name "Unknown Author" across the feature scope.
