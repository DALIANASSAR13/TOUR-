# AGENTS.md

Instructions for AI coding agents working on this project.

## Project

A web app where tourists find verified tour guides and book them, either for some hours on one day or for several days.
Graduation project: 6 students, 3 weeks. Build the MVP only.

Actors: **Tourist**, **Guide**, **Admin**, plus a **System** scheduled job.

## Source of truth

- Class diagrams: `docs/class-diagram.md` (also `docs/class-diagram.pdf`). Method signatures, controllers, DTOs and ViewModels per use case.
- SRS: https://claude.ai/artifact/J8ogNzGY5QTfiTMhpUq1ea (FR-01 to FR-20, BR-01 to BR-15, NFR-01 to NFR-08, Assumptions, UI Screens)
- Class Specs: https://claude.ai/artifact/RZrQnDJ6tSNTwuGYTFAKWf (classes, properties, validation, allowed status changes)

If the documents don't cover something, do not guess. Ask the team.

## Work cycle (every session)

1. **Start:** read `AGENTS.md` (rules) and `STATE.md` (where the project is now).
2. **Work:** take the next task from `STATE.md` → "Next", or the task the team gives you.
3. **Build:** run `dotnet build` and fix every error before you finish.
4. **End:** update `STATE.md`:
   - move finished items to "Done" and update "In progress" and "Next"
   - add any new decision the team made to "Decisions log"
   - add bugs or open problems to "Known issues"
   - add one row at the top of "Cycle log" (date, what changed, build result)

`AGENTS.md` changes only when the team changes a rule. `STATE.md` changes every cycle.

## Tech stack

- ASP.NET Core MVC on **.NET 8**
- EF Core, **Code First**, with migrations
- **SQL Server**
- ASP.NET Core Identity for accounts and roles
- A `BackgroundService` for the scheduled job (NFR-06)
- English-only interface (NFR-07). Desktop is required; a phone layout is a nice-to-have.

### Packages (pin these versions)

| Project | Package | Version |
| --- | --- | --- |
| DAL | Microsoft.EntityFrameworkCore.SqlServer | 8.0.32 |
| DAL | Microsoft.AspNetCore.Identity.EntityFrameworkCore | 8.0.32 |
| BLL | AutoMapper | 14.0.0 |
| BLL | FluentValidation | 11.11.0 |
| BLL | FluentValidation.DependencyInjectionExtensions | 11.11.0 |
| BLL | Microsoft.Extensions.Options.ConfigurationExtensions | 8.0.0 |
| PL | Microsoft.EntityFrameworkCore.Design + Tools | 8.0.32 |
| PL | jquery-validation + jquery-validation-unobtrusive | from the MVC template (`wwwroot/lib`) |

- **Do not upgrade AutoMapper to 15 or later.** From version 15 it needs a paid commercial license. Version 14 is free (MIT).
  AutoMapper 14.0.0 has a known DoS advisory (GHSA-rvv3-g6hj-g44x, deep recursion). The team accepted it; the `NU1903` build warning is expected. Never map self-referencing or very deep object graphs.
- **Do not add `FluentValidation.AspNetCore`.** Its auto-validation is deprecated. Validators are called explicitly (see "Validation").

## Architecture

N-Tier with 3 projects. Do **not** introduce microservices or Clean Architecture.

```
TourGuide/
  TourGuide.sln
  TourGuide.PL/    Presentation layer   (ASP.NET Core MVC; "Web" in the rest of this file)
  TourGuide.BLL/   Business logic layer (class library)
  TourGuide.DAL/   Data access layer    (class library, EF Core)
```

Run `dotnet` commands from the `TourGuide/` folder (where `TourGuide.sln` is).

### Who calls whom

```
TourGuide.PL  ──►  TourGuide.BLL  ──►  TourGuide.DAL  ──►  SQL Server
```

- Web references BLL and DAL. It uses DAL **only** for DI registration (`AddDataAccess`) and Identity types (`ApplicationUser`, `SignInManager`, role names).
- BLL references DAL and talks to it **only** through `IUnitOfWork` and repository interfaces.
- DAL references nothing in the solution.
- Every dependency is an interface injected through the constructor. Nobody calls `new` on a service, repository, or `AppDbContext`.

### TourGuide.DAL — data access

```
Entities/  Enums/  Constants/Roles.cs
Data/AppDbContext.cs  Data/Configurations/  Data/Seed/  Migrations/
Repositories/Interfaces/   IRepository<T>, IBookingRepository, IGuideProfileRepository, ...
Repositories/              Repository<T>, BookingRepository, GuideProfileRepository, ...
UnitOfWork/                IUnitOfWork, UnitOfWork
Common/PagedList.cs        PagedList<T> (used by DAL and BLL)
Queries/GuideSearchQuery.cs  search criteria passed from BLL to IGuideProfileRepository.SearchAsync
DependencyInjection.cs     AddDataAccess(this IServiceCollection, IConfiguration)
```

- **Does:** the table shape (keys, max lengths, indexes, unique and check constraints, relationships, `RowVersion`), and all queries.
- **Generic repository** `IRepository<T>`: `GetByIdAsync`, `AddAsync`, `Update`, `FindAsync(predicate)`, `ExistsAsync(predicate)`. No `Delete`, because nothing is deleted.
- **Specific repositories** hold every query that is more than a lookup by id. Examples: `IBookingRepository.HasOverlapAsync(guideId, startUtc, endUtc)` and `GetDueForAutoCancelAsync(nowUtc)`, `IGuideProfileRepository.SearchAsync(filter)`.
- Repositories return entities or `List<T>`, never `IQueryable<T>`. They never call `SaveChanges`.
- **Unit of Work** `IUnitOfWork` exposes every repository plus `SaveChangesAsync()` and `BeginTransactionAsync()`. It is the only place that saves.
- **Forbidden:** business rules (status changes, prices, deadlines), and any reference to BLL or Web.

### TourGuide.BLL — business logic

```
Services/Interfaces/   one interface per service (see "Catalog")
Services/              one class per interface
Domain/Interfaces/     IBookingStateMachine, IPriceCalculator
Domain/                BookingStateMachine, PriceCalculator, BookingRules (limits as constants)
Infrastructure/        IFileStorage + LocalFileStorage, IPaymentGateway + SimulatedPaymentGateway,
                       ICountryProvider + CountryProvider (ISO country list)
DTOs/                  request DTOs (input) and response DTOs (output), grouped by feature
Validators/            one FluentValidation AbstractValidator<T> per request DTO, plus shared FileUploadValidator
Mappings/              AutoMapper profiles: entity ↔ DTO, one profile per feature
Common/                Result, Result<T>, ErrorCodes, ValidationError
Options/               FileStorageOptions, BookingJobOptions
DependencyInjection.cs AddBusinessLogic(this IServiceCollection, IConfiguration)
```

- **Does:** every BR, permission checks ("does this booking belong to this user?"), and transactions through `IUnitOfWork`.
- **State machine:** `BookingStateMachine` holds the allowed status-change table, and it is the only code that changes `Booking.Status`.
- **Single place for each rule:** prices only in `PriceCalculator`, and limits (2–10 hours, 2–14 days, 24 h, 5 MB, file types) only in `BookingRules` and `FileStorageOptions`. Never copy a number into another file.
- **Swappable services:** `IPaymentGateway` (simulated now, so a real gateway later is a new class, not an edit) and `IFileStorage` (writes outside `wwwroot`).
- **Time:** inject .NET 8 `TimeProvider`. Never call `DateTime.UtcNow` directly.
- **Result pattern:** a business failure (overlap, deadline passed, wrong owner) returns `Result.Fail(ErrorCode, message)`. Exceptions are only for unexpected errors.
- **Every service method that takes a request DTO** first runs its `IValidator<T>`. On failure it returns `Result.Fail` with the field errors, before any business logic or database call.
- **AutoMapper** (`IMapper`) maps entities to response DTOs, and request DTOs to new entities. Profiles only copy and rename fields. Prices, statuses and dates are set by the domain classes, never inside a profile.
- `UserManager<ApplicationUser>` is used here (in `AccountService`) to create users and assign roles.
- Services take the current user's id as a parameter, and take and return DTOs, never entities.
- **Forbidden:** `AppDbContext`, `HttpContext`, `SignInManager`, ViewModels, MVC types, or any reference to Web.

### TourGuide.PL — presentation

```
Areas/Admin  Areas/Guide  Areas/Tourist
Controllers/  (Home, Account, Search, GuideProfile)
ViewModels/  Views/  wwwroot/ (the team's UI design)
Mappings/          AutoMapper profiles: DTO ↔ ViewModel, one profile per area
Validation/        custom attributes with client-side adapters (MaxFileSize, AllowedFileTypes, FutureDate)
Services/          ICurrentUserService + CurrentUserService (user id and role from the claims)
Extensions/        ModelStateExtensions.AddResultErrors(Result)
BackgroundJobs/    BookingStatusJob : BackgroundService
Filters/  Middleware/   global error handling
Program.cs         calls AddDataAccess(), AddBusinessLogic() and AddAutoMapper(), then Identity and MVC
```

- **Thin controllers:** check `ModelState`, map the ViewModel to a request DTO, call one service, put any `Result` errors back into `ModelState`, then show a view or redirect. No business logic.
- Controllers get the user id from `ICurrentUserService`, never by reading claims themselves.
- `[Authorize(Roles = Roles.X)]` on every area controller (NFR-01).
- `BookingStatusJob` creates a DI scope (`IServiceScopeFactory`) on each run and calls `IBookingStatusJobService`. The rules stay in BLL.
- One global error handler: log the error and show a friendly error page. Never show stack traces to users.
- **Forbidden:** `AppDbContext`, repositories, or LINQ queries in controllers and views; passing entities to views; business rules in controllers.

### DI lifetimes

| Type | Lifetime |
| --- | --- |
| `AppDbContext`, `IUnitOfWork`, repositories, BLL services, `IFileStorage`, `IPaymentGateway`, validators, `ICurrentUserService` | Scoped |
| `TimeProvider`, `IBookingStateMachine`, `IPriceCalculator`, `ICountryProvider` | Singleton (stateless) |
| `IMapper` | As registered by `AddAutoMapper` |
| `BookingStatusJob` | Hosted service; creates its own scope on every run |

- Validators are registered with `AddValidatorsFromAssemblyContaining<...>()` in `AddBusinessLogic`.
- AutoMapper profiles from both BLL and Web are registered with one `AddAutoMapper(...)` call in `Program.cs`. In Development, call `AssertConfigurationIsValid()` at startup so a missing mapping fails right away.
- Options classes bind from `appsettings.json` with `services.Configure<T>(config.GetSection(...))`.

### Principles

- **SOLID:** one reason to change per class, depend on interfaces, open for new implementations (payment, storage) without editing callers.
- **DRY:** each rule and number lives in one place (see BLL).
- **async all the way:** `async`/`await` with `Async`-suffixed methods and a `CancellationToken` from controller to repository. No `.Result` or `.Wait()`.
- **Naming:** interfaces start with `I`; one public class per file; the file name matches the class name.
- **Mapping:** AutoMapper profiles only (BLL: entity ↔ DTO, Web: DTO ↔ ViewModel). No hand-written mapping code in services or controllers.

## Validation

Validation runs in 4 layers. Each layer catches what the one before it can't, and the server never trusts the client.

| # | Layer | Where | Tool | Checks | On failure |
| --- | --- | --- | --- | --- | --- |
| 1 | Client side | Browser | jQuery Validation + Unobtrusive, from ViewModel attributes | Required, length, range, email, password match, file size and type, future date | The form is not sent; the error shows under the field |
| 2 | Input format | Web controller | DataAnnotations on ViewModels → `ModelState.IsValid` | The same checks as layer 1, run again on the server | Return the same view with errors |
| 3 | Business input | BLL service | **FluentValidation** validator for the request DTO | Limits and rules that need logic or data: hours vs. booking type, start ≥ now + 24 h, same-day end, min price ≤ max price in search, file content signature, country code in the list | `Result.Fail` with field errors → `ModelState` → same view |
| 4 | Business rules | BLL service + domain | `IBookingStateMachine`, repositories | Status changes, overlap, ownership, payment deadline, one review per booking | `Result.Fail` with an error code → message at the top of the page |

The database constraints in DAL (unique, check, max length) are a final guard. Hitting one means a validation layer above has a gap: fix that layer.

### Rules

- **Every form view** includes `<partial name="_ValidationScriptsPartial" />` and an `asp-validation-summary` plus `asp-validation-for` for each field.
- **Every POST action** checks `ModelState.IsValid` first, even when client-side validation passed.
- **Anti-forgery:** register `AutoValidateAntiforgeryTokenAttribute` globally. Forms use the form tag helper, which adds the token.
- **One source for limits:** ViewModel attributes and FluentValidation rules both read the constants in `BookingRules` and `FileStorageOptions` (for example `[Range(BookingRules.MinHours, BookingRules.MaxHours)]`). Never type the number again.
- **Custom client-side attributes** in `Web/Validation/` implement `ValidationAttribute` **and** `IClientModelValidator`, with a matching jQuery adapter in `wwwroot/js/validation.js`.
- **Error messages** are in English, short, and say how to fix it ("Choose 2 to 10 hours").
- **Validators** contain no database writes and no business decisions. One that needs data (for example the country list) gets it through an injected interface.
- **Payment card fields** are validated for format only (number length and digits, expiry in the future, CVV 3–4 digits), then thrown away (NFR-04).

## Catalog

Every class below is registered in DI by its interface.

### BLL services

| Interface | Responsibility | FRs |
| --- | --- | --- |
| `IAccountService` | Register a tourist or a guide (with documents), assign the role | FR-01, FR-02, FR-03 |
| `IGuideVerificationService` | Pending list, view documents, approve, reject with reason, re-apply | FR-04 – FR-07 |
| `IGuideProfileService` | Get and update own profile, get public profile | FR-08, FR-09 |
| `ISearchService` | Search approved and complete guides with filters | FR-10 |
| `IBookingService` | Create request, accept, reject, withdraw, list for tourist and guide | FR-11 – FR-13, FR-15, FR-16 |
| `IPaymentService` | Pay an accepted booking | FR-14 |
| `IReviewService` | Add a review, update the guide's rating | FR-19, FR-20 |
| `IBookingStatusJobService` | Auto-cancel and auto-complete bookings | FR-17, FR-18 |
| `IBookingStateMachine` | The allowed status changes | BR-05 – BR-10 |
| `IPriceCalculator` | Billable hours and total price | FR-12, BR-11 |
| `IFileStorage` | Save, read and replace files outside `wwwroot` | FR-02, FR-05 |
| `IPaymentGateway` | Simulated payment; returns a transaction reference | FR-14, BR-12 |
| `ICountryProvider` | ISO country list for dropdowns and validation | FR-01, FR-08 |

### Web services

| Interface | Responsibility |
| --- | --- |
| `ICurrentUserService` | Current user's id and role from the claims |

### DAL repositories and Unit of Work

| Interface | Extra queries beyond `IRepository<T>` |
| --- | --- |
| `IGuideProfileRepository` | `SearchAsync(filter)`, `GetPendingAsync()`, `GetWithDetailsAsync(id)`, `GetByUserIdAsync(userId)` |
| `IGuideDocumentRepository` | `GetByGuideAsync(guideId)`, `GetByTypeAsync(guideId, type)` |
| `IBookingRepository` | `HasOverlapAsync(guideId, startUtc, endUtc, excludeId)`, `GetForTouristAsync(...)`, `GetForGuideAsync(...)`, `GetDueForAutoCancelAsync(nowUtc)`, `GetDueForCompletionAsync(nowUtc)` |
| `IPaymentRepository` | `GetByBookingAsync(bookingId)` |
| `IReviewRepository` | `GetForGuideAsync(guideId, page)`, `ExistsForBookingAsync(bookingId)` |
| `IRepository<Language>`, `IRepository<Specialization>` | none |
| `IUnitOfWork` | Exposes all of the above, plus `SaveChangesAsync()` and `BeginTransactionAsync()` |

### Validators (BLL, FluentValidation)

| Validator | Request DTO | FRs / rules |
| --- | --- | --- |
| `RegisterTouristValidator` | `RegisterTouristRequest` | FR-01 |
| `RegisterGuideValidator` | `RegisterGuideRequest` | FR-02; uses `FileUploadValidator` for the 3 documents |
| `RejectGuideValidator` | `RejectGuideRequest` | FR-06; reason 10–500 characters |
| `ReapplyValidator` | `ReapplyRequest` | FR-07; uses `FileUploadValidator` for the 3 documents |
| `UpdateGuideProfileValidator` | `UpdateGuideProfileRequest` | FR-08; price, experience, bio, languages, specializations, time zone, photo |
| `GuideSearchFilterValidator` | `GuideSearchFilter` | FR-10; min price ≤ max price |
| `CreateBookingValidator` | `CreateBookingRequest` | FR-11, BR-04; uses `TimeProvider` |
| `PaymentValidator` | `PaymentRequest` | FR-14; card format only |
| `CreateReviewValidator` | `CreateReviewRequest` | FR-19; rating 1–5, comment ≤ 1,000 |
| `FileUploadValidator` | `FileUpload` (shared) | PDF/JPG/PNG by content signature, ≤ 5 MB |

### AutoMapper profiles

| Profile | Project | Maps |
| --- | --- | --- |
| `AccountMappingProfile` | BLL | register requests → `ApplicationUser`, `GuideProfile` |
| `GuideMappingProfile` | BLL | `GuideProfile` ↔ guide DTOs, `GuideDocument` → document DTO |
| `BookingMappingProfile` | BLL | `Booking` → booking DTOs; `CreateBookingRequest` → `Booking` (no price or status) |
| `ReviewMappingProfile` | BLL | `Review` ↔ review DTOs |
| `LookupMappingProfile` | BLL | `Language`, `Specialization` → lookup DTO |
| `AdminViewModelProfile`, `GuideViewModelProfile`, `TouristViewModelProfile`, `PublicViewModelProfile` | Web | DTOs ↔ ViewModels for each area |

Name AutoMapper profiles with a `MappingProfile` or `ViewModelProfile` suffix, so they never clash with the `GuideProfile` entity.

## Commands

No automated tests in this project. EF Core tools come from the local tool manifest (`.config/dotnet-tools.json`).

From the repo root:

```bash
dotnet tool restore
dotnet build TourGuide/TourGuide.sln
dotnet run --project TourGuide/TourGuide.PL
dotnet ef migrations add <Name> --project TourGuide/TourGuide.DAL --startup-project TourGuide/TourGuide.PL --output-dir Migrations
dotnet ef database update --project TourGuide/TourGuide.DAL --startup-project TourGuide/TourGuide.PL
```

Admin seed: the admin account is created at startup from `Seed:AdminEmail` + `Seed:AdminPassword`. Keep the password out of Git:

```bash
dotnet user-secrets set "Seed:AdminEmail" "<email>" --project TourGuide/TourGuide.PL
dotnet user-secrets set "Seed:AdminPassword" "<password>" --project TourGuide/TourGuide.PL
```

After editing `docs/class-diagram.md`, regenerate the HTML and PDF:

```bash
python .build/make_html.py
python -m http.server 8765 --bind 127.0.0.1   # in a second terminal
"C:/Program Files/Google/Chrome/Application/chrome.exe" --headless=new --no-pdf-header-footer --virtual-time-budget=30000 --print-to-pdf=docs/class-diagram.pdf http://127.0.0.1:8765/docs/class-diagram.html
```

## Data conventions (Class Specs)

- Primary keys: `int Id`, except `ApplicationUser`, which uses the Identity `string` key.
- Dates: `DateTime` in UTC; names end in `Utc`. Convert to the guide's `TimeZoneId` only for display.
- Money: `decimal(10,2)`, USD only.
- Every `string` has a max length, as given in the Class Specs.
- Enums are stored as `int`.
- Roles are Identity roles named `Tourist`, `Guide`, `Admin`, kept as string constants in a `Roles` class. They are not an enum.
- Nothing is deleted. Records change status instead. Reviews cannot be edited.
- Languages and specializations are seeded lists. Admin accounts are seeded too; public sign-up creates only Tourist or Guide.

## Classes

`ApplicationUser`, `GuideProfile`, `GuideDocument`, `Language`, `Specialization`, `GuideLanguage`, `GuideSpecialization`, `Booking`, `Payment`, `Review`.
Enums: `VerificationStatus`, `DocumentType`, `BookingType`, `BookingStatus`, `CancellationReason`.

## Business rules

### Guides

- A guide signs up with 3 documents: proof of residence, government ID, criminal record certificate. Only PDF, JPG or PNG, max 5 MB each; check the file content, not just the extension.
- A guide can only use the site as a guide after an Admin approves them (BR-01). Approval needs all 3 documents.
- Rejection needs a written reason (10 to 500 characters). A rejected guide can upload new documents, which sets the status back to `Pending`.
- Search shows a guide only if they are `Approved` **and** their profile is complete (BR-02).

### Bookings

- `Hourly`: 2 to 10 hours, ending on the same local day it starts. `MultiDay`: 2 to 14 whole days.
- The trip must start at least 24 hours after the request is sent.
- Price: `BillableHours × PricePerHourAtRequest`. A full day counts as 8 hours. The price is copied when the request is sent and does not change after that.
- These are the only allowed status changes. Refuse anything else:

| From | To | Who | Condition |
| --- | --- | --- | --- |
| Pending | Accepted | Guide (owner) | No overlap with the guide's Accepted or Confirmed bookings |
| Pending | Rejected | Guide (owner) | |
| Pending | Cancelled | Tourist (owner) | `WithdrawnByTourist` |
| Pending | Cancelled | System | 24 h after creation, `NoGuideResponse` |
| Accepted | Confirmed | Tourist (owner) | Paid before `PaymentDeadlineUtc`; creates a `Payment` |
| Accepted | Cancelled | Tourist (owner) | `WithdrawnByTourist` |
| Accepted | Cancelled | System | Deadline passed, `NotPaid` |
| Confirmed | Completed | System | `EndUtc` passed |

- `PaymentDeadlineUtc` = the earlier of (accept time + 24 h) and `StartUtc`.
- Overlap: `A.StartUtc < B.EndUtc && B.StartUtc < A.EndUtc`.
- `Booking.RowVersion` is a concurrency token.

### Payment

- Payment is simulated (BR-12). Validate the card fields' format, then throw them away. Never store or log card data (NFR-04).
- Amount must equal `TotalPrice`. One payment per booking.
- Save the `Payment` and set the booking to `Confirmed` in one transaction.

### Reviews

- Only the booking's tourist can review it, only once it is `Completed`, and only once per booking (BR-13 to BR-15).
- Rating is 1 to 5 whole stars; the comment is optional, max 1,000 characters.
- Update `AverageRating` and `ReviewCount` in the same transaction as the review.

## Security

- Each user reaches only their own role's pages and data (NFR-01).
- Guide documents are stored **outside `wwwroot`** and served only to logged-in Admins through an authorized action (NFR-02, BR-03).
- Passwords are hashed by Identity, never stored in plain text (NFR-03).

## UI

- Use the team's provided UI design. The SRS section 8 maps each screen to its FRs.
- "Explore Extraordinary Tours" and "Design Your Group" are about tour packages, which are out of scope. Reuse their layout only, as SRS section 8 describes.

## Out of scope

Do not build: chat, a real payment gateway or refunds, cancelling a Confirmed booking, an availability calendar, notifications, languages other than English, review replies or moderation, an admin statistics dashboard, tour packages or custom group tours, suspending an approved guide, or a mobile app.

## Git

One branch per module (NFR-08).
