# Product Requirements Document (PRD)
## Feature: User Profile Management

**Document Version:** 1.0  
**Date:** March 25, 2026  
**Status:** Draft  
**Application:** CRUDDemo ASP.NET MVC App  

---

## 1. Overview

### 1.1 Purpose
This document defines the product requirements for a **User Profile** feature to be added to the CRUDDemo application. The feature will allow users to maintain a personal profile with identity details, a status message, a biography, and social media links.

### 1.2 Background
The current application manages `Item` records via a standard CRUD interface. There is no concept of users, identity, or personalisation. This feature introduces a `UserProfile` entity to associate ownership, identity, and presence information with application users.

### 1.3 Goals
- Allow users to create and maintain a personal profile.
- Enable users to set a public status and a free-text biography.
- Allow users to provide links to their social media presence.
- Keep the implementation consistent with the existing MVC architecture and data layer (Entity Framework Core + SQLite).

### 1.4 Non-Goals
- Authentication/authorisation (login, registration) — not in scope for this iteration.
- Profile photo/avatar upload.
- Privacy/visibility controls per field.
- Real-time status updates or notifications.

---

## 2. User Stories

| ID | As a… | I want to… | So that… |
|----|--------|-----------|----------|
| US-01 | User | View my profile page | I can see all my stored profile information at a glance |
| US-02 | User | Edit my display name and email | My profile reflects my current identity |
| US-03 | User | Set a short status message | Others know my current availability or mood |
| US-04 | User | Write and update a bio | I can share background information about myself |
| US-05 | User | Add/edit social media profile URLs | Others can find me on external platforms |
| US-06 | User | Clear any optional field | I can remove information I no longer want to share |
| US-07 | Admin | View any user profile | I can moderate profile content |

---

## 3. Functional Requirements

### 3.1 Profile Data Fields

| Field | Type | Required | Constraints |
|-------|------|----------|-------------|
| `DisplayName` | String | Yes | 2–100 characters |
| `Email` | String | Yes | Valid email format, max 256 characters |
| `Status` | String (enum) | Yes | One of: `Active`, `Away`, `Busy`, `Offline` |
| `StatusMessage` | String | No | Max 160 characters (tweet-length) |
| `Bio` | String | No | Max 1,000 characters |
| `WebsiteUrl` | String | No | Valid URL, max 512 characters |
| `GitHubUrl` | String | No | Valid URL, must be `github.com` domain |
| `LinkedInUrl` | String | No | Valid URL, must be `linkedin.com` domain |
| `TwitterUrl` | String | No | Valid URL, must be `twitter.com` or `x.com` domain |
| `FacebookUrl` | String | No | Valid URL, must be `facebook.com` domain |
| `InstagramUrl` | String | No | Valid URL, must be `instagram.com` domain |
| `CreatedAt` | DateTime | System | Set on creation, UTC |
| `UpdatedAt` | DateTime | System | Updated on each save, UTC |

### 3.2 Pages / Views

| Page | Route | Description |
|------|-------|-------------|
| Profile Detail | `GET /Profile/Details/{id}` | Read-only view of a user profile |
| Edit Profile | `GET /Profile/Edit/{id}` | Form to edit profile details |
| Save Profile | `POST /Profile/Edit/{id}` | Persist edited profile |
| Create Profile | `GET /Profile/Create` | Form to create a new profile |
| Save New Profile | `POST /Profile/Create` | Persist new profile |
| Delete Profile | `GET /Profile/Delete/{id}` | Confirmation page |
| Confirm Delete | `POST /Profile/Delete/{id}` | Remove the profile record |
| List Profiles | `GET /Profile/Index` | List all profiles (admin use) |

### 3.3 Validation Rules
- `DisplayName`: Required, 2–100 chars, no HTML tags.
- `Email`: Required, valid RFC 5322 email format.
- `Status`: Required, must be one of the defined enum values.
- `StatusMessage`: Optional, max 160 chars.
- `Bio`: Optional, max 1,000 chars.
- All URL fields: Optional; when provided, must be a well-formed absolute HTTPS URL matching the expected domain for the platform.
- URL domain validation must be performed server-side (not solely client-side).

### 3.4 Business Rules
- Each user may have at most **one** profile record.
- `CreatedAt` is immutable after creation.
- `UpdatedAt` is always refreshed on every successful save.
- Social media URLs must use `https://` — plain `http://` URLs must be rejected.
- No user-supplied content may be rendered as raw HTML.

---

## 4. Non-Functional Requirements

| Category | Requirement |
|----------|-------------|
| Security | All inputs sanitised; no raw HTML rendered in views |
| Security | CSRF tokens on all POST forms (`[ValidateAntiForgeryToken]`) |
| Security | URL fields validated server-side against allowed domains |
| Performance | Profile page should load in < 500 ms under typical load |
| Usability | Edit form must surface validation errors inline |
| Maintainability | Follow existing MVC folder/naming conventions |
| Data Integrity | Unique constraint on `Email` at DB level |

---

## 5. UI/UX Requirements

- Consistent with the existing `_Layout.cshtml` shared layout.
- Edit form groups fields into logical sections: **Identity**, **Status & Bio**, **Social Links**.
- Status dropdown with human-readable labels.
- Bio field rendered as a `<textarea>` (minimum 4 rows).
- Social media URL fields prefixed with the platform icon/label.
- Inline validation error messages per field using Bootstrap validation classes.
- A "Save Changes" primary button and a "Cancel" secondary button on the edit form.

---

## 6. Out of Scope

- OAuth / SSO integration.
- Profile visibility / privacy settings.
- Follower/following social graph.
- Profile image upload.
- Email verification flow.

---

## 7. Success Metrics

- A user can complete a full profile (create → edit → view) in under 2 minutes.
- Zero stored XSS findings in security review against profile fields.
- All required validation rules enforced both client-side and server-side.

---

## 8. Dependencies

| Dependency | Details |
|------------|---------|
| ASP.NET Core MVC | Existing framework |
| Entity Framework Core + SQLite | Existing ORM/database |
| Bootstrap (via `_Layout.cshtml`) | UI styling |
| `[ValidateAntiForgeryToken]` | CSRF protection (already used in `ItemsController`) |

---

## 9. Approvals

| Role | Name | Date |
|------|------|------|
| Product Owner | — | — |
| Tech Lead | — | — |
| Security Review | — | — |
