# STATE.md

The project's current state. Read it at the start of every work cycle and update it at the end (see `AGENTS.md` → "Work cycle").

**Last updated:** 2026-11-08
**Phase:** Phase 1 — Foundations (cycles 1.0.a – 1.0.c done) · Phase 2 cycle 2.8.a done
**Branch:** `List-Bookings` (Cycle 2.8.a done, not merged to `main` yet)

## Done

- [x] SRS, Class Specs, class diagrams, `docs/WORK_ORDER.md`, `AGENTS.md`, skills (`manage-project`, `build`)
- [x] **Cycle 1.0.a — Data model:** 10 entities, 5 enums, `Roles`, one configuration per entity (lengths, unique indexes, 9 check constraints, `RowVersion`, all deletes `Restrict`), `AppDbContext`, `IRepository<T>`/`Repository<T>`, 5 empty specific repositories (`partial`), `IUnitOfWork`/`UnitOfWork`, `PagedList<T>`, `AddDataAccess()`, migration `InitialCreate`
- [x] **Cycle 1.0.b — App skeleton:** BLL `Result`, `Result<T>`, `ValidationError`, `ErrorCodes`, `BookingRules`, `FileStorageOptions`, `BookingJobOptions`, `AddBusinessLogic()`; PL `Program.cs` (DAL + BLL + AutoMapper + Identity + global anti-forgery + error handler + mapper check in Development), `ICurrentUserService`, `ModelStateExtensions`; ViewModels moved from BLL to `TourGuide.PL/ViewModels`
- [x] **Cycle 1.0.c — Seed data:** 3 roles (migration), 9 languages and 5 specializations (migration), admin account at startup from `Seed:AdminEmail` + `Seed:AdminPassword` (user secrets)
- [x] Checked on a throw-away database: all 17 tables, constraints and seed rows created; app starts; `/`, `/Account/Login`, `/Guides`, `/Guides/Dashboard`, `/Home/Privacy` return 200; admin created with a hashed password and the Admin role

- [x] **Demo seed (all tables):** `DAL/Data/Seed/DemoDataSeeder.cs`, called from `Program.cs` in Development when `Seed:DemoData` is true. 14 users (admin, 3 tourists, 10 guides), 10 guide profiles (7 approved + complete, 1 approved incomplete, 1 pending, 1 rejected), 30 documents (placeholder PDFs in `App_Data/uploads`), language/specialization links, 15 bookings in all 6 statuses, 9 payments, 4 reviews. Logins: `<first>.<last>@tourguide.local` / `Seed:DemoPassword`. Emma Schmidt's bookings match UI screen 6.
- [x] **Cycle 2.8.a — List my bookings (FR-15):** `IBookingRepository.GetForTouristAsync`, `BookingDto` + `BookingMappingProfile`, `IBookingService`/`BookingService.GetForTouristAsync` (sets `CanPay`, `CanWithdraw`, `CanReview`, `AutoCancelAtUtc`), `Areas/Tourist/BookingsController.Index`, `MyBookingsViewModel`, `BookingItemViewModel`, `TouristViewModelProfile`, `TimeZoneHelper`, view + `css/Tourist/Bookings.css` from UI screen 6. Reviewed; fixes applied (see `tasks/FR-15-2.8.a.md`). Pay / Withdraw / Review buttons shown by status, disabled until 2.9 / 2.8.b / 3.2.
- [x] **Clean-up:** old tour-package UI deleted (`GuidesController`, `Views/Guides`, old ViewModels, `css/Guides`); home page without tours.

## In progress

- [ ] —

## Next

Follow `docs/WORK_ORDER.md` in order. With the `build` skill: `plan <cycle>` → `build <cycle>` → `review <cycle>` → `done`.

0. Cycle 2.8.b — Withdraw a booking (FR-15, BR-07): first build `IBookingStateMachine`/`BookingStateMachine`/`Actor` (from 2.6.b), then `BookingService.WithdrawAsync` + `Withdraw` POST
1. Cycle 1.1.a — Log in / log out (FR-03): replace the placeholder `AccountController.Login` POST with `SignInManager`
2. Cycle 1.1.b — Role access (areas, `[Authorize(Roles)]`, access-denied page)
3. Cycle 1.2.a — Tourist sign-up (FR-01)
4. Cycle 1.3.a — Guide sign-up with documents (FR-02)

## Decisions log

Newest first. Add the team's decisions here; the full documents hold the details.

| Date | Decision |
| --- | --- |
| 2026-11-08 | 2.8.b builds the full `BookingStateMachine` first (option a), so 2.6.b / 2.7 / 2.9 reuse it; no status change outside it (NFR-05). |
| 2026-11-07 | Booking reference shown as `TG-{Id:D4}`; pending auto-cancel countdown = `CreatedAtUtc + BookingRules.PendingExpiryHours`; tourist actions (`CanPay`, `CanWithdraw`, `CanReview`) are decided in `BookingService`, not in PL. |
| 2026-11-07 | Role pages live in `Areas/<Role>` with `[Area]` + `[Authorize(Roles)]`; layout links use `asp-area=""`. |
| 2026-11-07 | Demo data is seeded at startup by code (needs `UserManager` for hashed passwords), not by migration; Development only, runs once when no guide profile exists. Limits come from `BookingRules` via `DemoSeedSettings`. |
| 2026-11-07 | Branch per feature, e.g. `List-Bookings` for Cycle 2.8.a (NFR-08). |
| 2026-11-06 | Code follows the SRS, not the earlier tour-package model: `Tour`, `Tourist`, `Guide` (TPH), `CustomTripRequest`, `Notification` and their migrations were removed. |
| 2026-11-06 | Keep the team's project names and folder: `TourGuide/TourGuide.sln` with `TourGuide.PL` (= "Web" in the docs), `TourGuide.BLL`, `TourGuide.DAL`. EF Core and tools 8.0.32. |
| 2026-11-06 | Identity uses the default `string` key (Class Specs), not `int`. |
| 2026-11-06 | AutoMapper stays on 14.0.0 despite advisory GHSA-rvv3-g6hj-g44x (accepted risk; `NU1903` warning expected). |
| 2026-11-06 | Roles, languages and specializations are seeded by the migration (`HasData`); the admin account at startup from configuration. |
| 2026-11-06 | Class diagrams define method signatures, controllers per area, DTOs and ViewModels; `PagedList<T>` and `GuideSearchQuery` live in DAL; `Actor` enum (BLL, not stored) for the state machine. |
| 2026-11-06 | Validation in 4 layers: client (jQuery Unobtrusive) → ViewModel DataAnnotations → FluentValidation in BLL → domain rules. FluentValidation 11.11.0, called explicitly. |
| 2026-11-06 | Patterns: generic + specific Repository, Unit of Work, constructor DI with one `Add...` extension per layer, service interfaces, Result pattern, Options pattern, `BookingStateMachine`, `TimeProvider`, swappable `IPaymentGateway` and `IFileStorage`, AutoMapper. |
| 2026-11-06 | N-Tier: Web → BLL → DAL. Services return DTOs. .NET 8, SQL Server, no automated tests. |
| 2026-11-06 | Class Specs approved, including the 10 decisions in section 9. SRS assumptions accepted. |

## Known issues

- **Demo password in Git:** `Seed:DemoPassword` is in `appsettings.json` for local use; never enable `Seed:DemoData` outside Development. To reseed: `dotnet ef database drop --force` then `database update`.
- **TEMPORARY dev login (2026-11-07):** `Controllers/DevLoginController.cs`, `Views/DevLogin/Index.cshtml`, `ViewModels/DevLoginUserViewModel.cs`, the red "Dev login" link in `_Layout.cshtml`, and `LoginPath = /DevLogin` in Development (`Program.cs`). Lets testers sign in as a demo user; 404 outside Development. **Remove all of it in Cycle 1.1.a.**
- **Package Manager Console:** `dotnet ef database drop` hangs on the y/N prompt; add `--force`.
- **2.8.a not fully checked:** AC1–AC12 to be run by the team (Dev login → Emma / John / Omar). A guide opening `/Tourist/Bookings` gets a 404 for `/Account/AccessDenied` until 1.1.b.
- **Links to pages not built yet (404):** "Find a guide" → `Search/Index` (2.4), "Guide profile" → `GuideProfile/Details` (2.5). Guide photos show initials until 2.3.
- **Existing local databases:** `TourGuideDb` on each machine still has the old tour schema. Drop it, then run `dotnet ef database update` (see `AGENTS.md` → Commands).
- **Seed lists to confirm:** languages (English, Arabic, French, German, Spanish, Italian, Russian, Chinese, Japanese) and specializations (Historical, Camping, Cruise, Walking, Photography, from the old `TourType`). Change `Data/Seed/LookupSeed.cs` + add a migration if the team wants others.
- **Admin email not decided:** the admin is created only when `Seed:AdminEmail` and `Seed:AdminPassword` are set (user secrets).
- **Placeholder:** Login POST shows "not connected yet" until 1.1.a. `wwwroot/css/Home/Index.css` still has unused tour styles.
- .NET 8 support ends on 2026-11-10.
- The `build` skill reads `docs/STATE.md`, but this file is at the project root (`AGENTS.md` points here). One of them needs to change.
- The skills mention tests per cycle; the team chose no automated tests, so `WORK_ORDER.md` uses manual checks instead.

## Cycle log

Newest first. One row per work cycle.

| Date | What changed | Build |
| --- | --- | --- |
| 2026-11-07 | Built + reviewed Cycle 2.8.a; temporary Dev login; old tour UI deleted (5 files deleted by mistake restored from git) | Build passes on the team's machine; page opens after restoring `[Area]` |
| 2026-11-07 | Planned Cycle 2.8.a (`tasks/FR-15-2.8.a.md`); added `DemoDataSeeder` for all tables + `Seed:DemoData`/`DemoPassword` settings | Build passed on the team's machine; DB dropped, updated and seeded |
| 2026-11-06 | Cycles 1.0.a – 1.0.c on `cycle-1/foundation`: new data model + `InitialCreate`, BLL/PL skeleton, seed; old tour model removed; ViewModels moved to PL; `AGENTS.md` paths fixed to `TourGuide/TourGuide.*` | Passes (only `NU1903` AutoMapper warning); migration applied to a test DB; 5 pages return 200 |
| 2026-11-06 | Tried a 6-member version of `docs/WORK_ORDER.md`, then restored the cycles version on request; added `WORK_ORDER.html` and `.pdf`; `.build/make_html.py` now takes a doc name | PDF 11 pages |
| 2026-11-06 | Installed `manage-project` and `build` skills; exported `docs/SRS.md`, `docs/CLASSES_SPECS.md`; wrote `docs/WORK_ORDER.md` | Not run (no code changed) |
| 2026-11-06 | Expanded `docs/class-diagram.md` by use case and state; added `docs/class-diagram.html`, `docs/class-diagram.pdf`, `.build/make_html.py` | All 20 diagrams render; PDF 27 pages |
| 2026-11-06 | Added validation, AutoMapper, FluentValidation, Repository, Unit of Work, DI and the catalog to `AGENTS.md` | Not run |
| 2026-11-06 | Created DAL project, enums, `Roles`, EF tool manifest, `AGENTS.md`, `STATE.md` | Not run |
