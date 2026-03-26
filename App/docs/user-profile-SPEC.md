# Technical Specification
## Feature: User Profile Management

**Document Version:** 1.0  
**Date:** March 25, 2026  
**Status:** Draft  
**References:** `user-profile-PRD.md`  
**Stack:** ASP.NET Core 7 MVC · Entity Framework Core · SQLite  

---

## 1. Architecture Overview

The User Profile feature follows the existing **MVC + EF Core** pattern of the application.

```
Models/
  UserProfile.cs          ← EF Core entity + Data Annotations
  UserProfileStatus.cs    ← Enum for status values
Controllers/
  ProfileController.cs    ← CRUD actions (mirrors ItemsController)
Views/
  Profile/
    Index.cshtml          ← List all profiles
    Details.cshtml        ← Read-only profile view
    Create.cshtml         ← Create form
    Edit.cshtml           ← Edit form
    Delete.cshtml         ← Delete confirmation
Data/
  AppDbContext.cs         ← Add DbSet<UserProfile>
```

---

## 2. Data Model

### 2.1 `UserProfileStatus` Enum

**File:** `App/Models/UserProfileStatus.cs`

```csharp
namespace App.Models
{
    public enum UserProfileStatus
    {
        Active,
        Away,
        Busy,
        Offline
    }
}
```

### 2.2 `UserProfile` Entity

**File:** `App/Models/UserProfile.cs`

```csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        // ── Identity ──────────────────────────────────────────────
        [Required]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        // ── Status & Bio ──────────────────────────────────────────
        [Required]
        public UserProfileStatus Status { get; set; } = UserProfileStatus.Active;

        [StringLength(160)]
        [Display(Name = "Status Message")]
        public string? StatusMessage { get; set; }

        [StringLength(1000)]
        public string? Bio { get; set; }

        // ── Social Links ──────────────────────────────────────────
        [StringLength(512)]
        [Display(Name = "Website")]
        public string? WebsiteUrl { get; set; }

        [StringLength(512)]
        [Display(Name = "GitHub")]
        public string? GitHubUrl { get; set; }

        [StringLength(512)]
        [Display(Name = "LinkedIn")]
        public string? LinkedInUrl { get; set; }

        [StringLength(512)]
        [Display(Name = "Twitter / X")]
        public string? TwitterUrl { get; set; }

        [StringLength(512)]
        [Display(Name = "Facebook")]
        public string? FacebookUrl { get; set; }

        [StringLength(512)]
        [Display(Name = "Instagram")]
        public string? InstagramUrl { get; set; }

        // ── Audit ─────────────────────────────────────────────────
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
```

### 2.3 `AppDbContext` Changes

Add a `DbSet` and a unique index on `Email`:

```csharp
// App/Data/AppDbContext.cs  — additions only

public DbSet<UserProfile> UserProfiles { get; set; } = null!;

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<UserProfile>()
        .HasIndex(u => u.Email)
        .IsUnique();
}
```

### 2.4 EF Core Migration

Run the following after implementing the model:

```bash
dotnet ef migrations add AddUserProfile
dotnet ef database update
```

---

## 3. Controller

**File:** `App/Controllers/ProfileController.cs`

### 3.1 URL Domain Allow-list Helper

```csharp
private static readonly Dictionary<string, string[]> _allowedDomains = new()
{
    { nameof(UserProfile.GitHubUrl),    new[] { "github.com" } },
    { nameof(UserProfile.LinkedInUrl),  new[] { "linkedin.com", "www.linkedin.com" } },
    { nameof(UserProfile.TwitterUrl),   new[] { "twitter.com", "x.com", "www.twitter.com", "www.x.com" } },
    { nameof(UserProfile.FacebookUrl),  new[] { "facebook.com", "www.facebook.com" } },
    { nameof(UserProfile.InstagramUrl), new[] { "instagram.com", "www.instagram.com" } },
};

private bool ValidateSocialUrl(string? url, string fieldName)
{
    if (string.IsNullOrWhiteSpace(url)) return true; // optional field

    if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        return false;

    // Enforce HTTPS only
    if (uri.Scheme != Uri.UriSchemeHttps)
        return false;

    if (!_allowedDomains.TryGetValue(fieldName, out var allowed))
        return true; // WebsiteUrl has no domain restriction

    return allowed.Contains(uri.Host, StringComparer.OrdinalIgnoreCase);
}
```

### 3.2 Action Methods

```csharp
// GET /Profile/Index
public async Task<IActionResult> Index()
{
    var profiles = await _context.UserProfiles.AsNoTracking().ToListAsync();
    return View(profiles);
}

// GET /Profile/Details/{id}
public async Task<IActionResult> Details(int? id)
{
    if (id == null) return NotFound();
    var profile = await _context.UserProfiles.AsNoTracking()
                                             .FirstOrDefaultAsync(p => p.Id == id);
    if (profile == null) return NotFound();
    return View(profile);
}

// GET /Profile/Create
public IActionResult Create() => View();

// POST /Profile/Create
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(
    [Bind("DisplayName,Email,Status,StatusMessage,Bio,WebsiteUrl,GitHubUrl,LinkedInUrl,TwitterUrl,FacebookUrl,InstagramUrl")]
    UserProfile profile)
{
    ValidateAllSocialUrls(profile);

    if (!ModelState.IsValid) return View(profile);

    profile.CreatedAt = DateTime.UtcNow;
    profile.UpdatedAt = DateTime.UtcNow;

    _context.UserProfiles.Add(profile);
    await _context.SaveChangesAsync();
    return RedirectToAction(nameof(Index));
}

// GET /Profile/Edit/{id}
public async Task<IActionResult> Edit(int? id)
{
    if (id == null) return NotFound();
    var profile = await _context.UserProfiles.FindAsync(id);
    if (profile == null) return NotFound();
    return View(profile);
}

// POST /Profile/Edit/{id}
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id)
{
    var profileToUpdate = await _context.UserProfiles.FindAsync(id);
    if (profileToUpdate == null) return NotFound();

    if (await TryUpdateModelAsync<UserProfile>(
            profileToUpdate, "",
            p => p.DisplayName, p => p.Email, p => p.Status,
            p => p.StatusMessage, p => p.Bio,
            p => p.WebsiteUrl, p => p.GitHubUrl, p => p.LinkedInUrl,
            p => p.TwitterUrl, p => p.FacebookUrl, p => p.InstagramUrl))
    {
        ValidateAllSocialUrls(profileToUpdate);

        if (!ModelState.IsValid) return View(profileToUpdate);

        profileToUpdate.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id });
    }

    return View(profileToUpdate);
}

// GET /Profile/Delete/{id}
public async Task<IActionResult> Delete(int? id)
{
    if (id == null) return NotFound();
    var profile = await _context.UserProfiles.AsNoTracking()
                                             .FirstOrDefaultAsync(p => p.Id == id);
    if (profile == null) return NotFound();
    return View(profile);
}

// POST /Profile/Delete/{id}
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    var profile = await _context.UserProfiles.FindAsync(id);
    if (profile != null)
    {
        _context.UserProfiles.Remove(profile);
        await _context.SaveChangesAsync();
    }
    return RedirectToAction(nameof(Index));
}

// ── Private helpers ───────────────────────────────────────────────

private void ValidateAllSocialUrls(UserProfile profile)
{
    var urlFields = new[]
    {
        (nameof(UserProfile.WebsiteUrl),   profile.WebsiteUrl),
        (nameof(UserProfile.GitHubUrl),    profile.GitHubUrl),
        (nameof(UserProfile.LinkedInUrl),  profile.LinkedInUrl),
        (nameof(UserProfile.TwitterUrl),   profile.TwitterUrl),
        (nameof(UserProfile.FacebookUrl),  profile.FacebookUrl),
        (nameof(UserProfile.InstagramUrl), profile.InstagramUrl),
    };

    foreach (var (field, value) in urlFields)
    {
        if (!ValidateSocialUrl(value, field))
            ModelState.AddModelError(field,
                $"Please enter a valid HTTPS URL for {field.Replace("Url", "")}.");
    }
}
```

---

## 4. Views

### 4.1 `Edit.cshtml` — Form Structure

The edit view should be divided into three `<fieldset>` sections:

```
┌─────────────────────────────────────────┐
│  IDENTITY                               │
│  Display Name  [_________________________] │
│  Email         [_________________________] │
├─────────────────────────────────────────┤
│  STATUS & BIO                           │
│  Status        [Active ▼]              │
│  Status Msg    [_________________________] │
│  Bio           [                        ] │
│                [  (textarea 4 rows)     ] │
├─────────────────────────────────────────┤
│  SOCIAL LINKS                           │
│  🌐 Website    [_________________________] │
│  🐙 GitHub     [_________________________] │
│  💼 LinkedIn   [_________________________] │
│  🐦 Twitter/X  [_________________________] │
│  📘 Facebook   [_________________________] │
│  📸 Instagram  [_________________________] │
├─────────────────────────────────────────┤
│          [Save Changes]  [Cancel]       │
└─────────────────────────────────────────┘
```

### 4.2 Key Razor snippets

**Status dropdown:**
```html
<div class="mb-3">
    <label asp-for="Status" class="form-label"></label>
    <select asp-for="Status"
            asp-items="Html.GetEnumSelectList<UserProfileStatus>()"
            class="form-select"></select>
    <span asp-validation-for="Status" class="text-danger"></span>
</div>
```

**Bio textarea:**
```html
<div class="mb-3">
    <label asp-for="Bio" class="form-label"></label>
    <textarea asp-for="Bio" class="form-control" rows="4"
              placeholder="Tell us a little about yourself…"></textarea>
    <div class="form-text">Max 1,000 characters.</div>
    <span asp-validation-for="Bio" class="text-danger"></span>
</div>
```

**Social URL input (repeat pattern per platform):**
```html
<div class="mb-3">
    <label asp-for="GitHubUrl" class="form-label">
        <span>🐙</span> GitHub
    </label>
    <input asp-for="GitHubUrl" class="form-control"
           type="url" placeholder="https://github.com/username" />
    <span asp-validation-for="GitHubUrl" class="text-danger"></span>
</div>
```

### 4.3 `Details.cshtml` — Display Rules

- Render all text fields using `@Html.DisplayFor(…)` or `@Model.FieldName` (no `@Html.Raw`).
- Social media URLs rendered as anchor tags with `target="_blank" rel="noopener noreferrer"`.
- If a URL field is null/empty, suppress the row entirely.
- Status rendered as a styled badge (`Active` = green, `Away` = yellow, `Busy` = red, `Offline` = grey).

---



## 6. Validation Summary

### Server-side (Data Annotations + Controller)
| Field | Rules |
|-------|-------|
| `DisplayName` | `[Required]`, `[StringLength(100, MinimumLength = 2)]` |
| `Email` | `[Required]`, `[EmailAddress]`, `[StringLength(256)]` |
| `Status` | `[Required]`, valid enum value |
| `StatusMessage` | `[StringLength(160)]` |
| `Bio` | `[StringLength(1000)]` |
| All URL fields | `[StringLength(512)]` + custom `ValidateSocialUrl` in controller |

### Client-side (Bootstrap + jQuery Validation)
- Add `@section Scripts { @{ await Html.RenderPartialAsync("_ValidationScriptsPartial"); } }` to Create and Edit views.
- `type="url"` on URL inputs enables browser-native URL validation.
- `maxlength` attributes derived automatically from `[StringLength]` via tag helpers.

---

## 7. Database Schema

```sql
CREATE TABLE UserProfiles (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    DisplayName TEXT    NOT NULL,
    Email       TEXT    NOT NULL UNIQUE,
    Status      INTEGER NOT NULL DEFAULT 0,
    StatusMessage TEXT,
    Bio         TEXT,
    WebsiteUrl  TEXT,
    GitHubUrl   TEXT,
    LinkedInUrl TEXT,
    TwitterUrl  TEXT,
    FacebookUrl TEXT,
    InstagramUrl TEXT,
    CreatedAt   TEXT    NOT NULL,
    UpdatedAt   TEXT    NOT NULL
);
```

---

## 8. Implementation Checklist

- [ ] Create `App/Models/UserProfileStatus.cs`
- [ ] Create `App/Models/UserProfile.cs`
- [ ] Update `App/Data/AppDbContext.cs` — add `DbSet<UserProfile>` and unique index
- [ ] Run EF Core migration (`AddUserProfile`)
- [ ] Create `App/Controllers/ProfileController.cs`
- [ ] Create `App/Views/Profile/Index.cshtml`
- [ ] Create `App/Views/Profile/Details.cshtml`
- [ ] Create `App/Views/Profile/Create.cshtml`
- [ ] Create `App/Views/Profile/Edit.cshtml`
- [ ] Create `App/Views/Profile/Delete.cshtml`
- [ ] Add nav link to `App/Views/Shared/_Layout.cshtml`
- [ ] Verify CSRF tokens on all POST forms
- [ ] Verify no `@Html.Raw` usage in profile views
- [ ] Verify social URL domain validation (unit test recommended)
- [ ] Run EF Core migration on staging DB

---

## 9. Acceptance Criteria

| AC | Scenario | Expected Result |
|----|----------|-----------------|
| AC-01 | Submit Create form with valid data | Profile saved; redirected to Index |
| AC-02 | Submit Create form with missing DisplayName | Inline validation error shown; no save |
| AC-03 | Submit Create form with invalid email | Inline validation error shown; no save |
| AC-04 | Enter a GitHub URL from a non-github.com domain | Server-side error: domain not allowed |
| AC-05 | Enter a social URL using `http://` | Server-side error: HTTPS required |
| AC-06 | Enter a bio over 1,000 characters | Validation error; truncation NOT silent |
| AC-07 | View Details page with all fields populated | All text encoded; links open in new tab |
| AC-08 | Submit Edit form with valid data | Profile updated; `UpdatedAt` refreshed |
| AC-09 | Confirm Delete | Profile removed; redirected to Index |
| AC-10 | Attempt POST without CSRF token | 400 Bad Request returned |

---

## 10. Open Questions

| # | Question | Owner | Resolution |
|---|----------|-------|------------|
| OQ-01 | Should profiles be tied to ASP.NET Identity users in a future iteration? | Tech Lead | Pending |
| OQ-02 | Is a `YoutubeUrl` or `TikTokUrl` field required? | Product Owner | Pending |
| OQ-03 | Should `StatusMessage` support Markdown? | Product Owner | Recommend **No** to reduce XSS surface |
