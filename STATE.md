# STATE.md

The project's current state. Read it at the start of every work cycle and update it at the end (see `AGENTS.md` → "Work cycle").

**Last updated:** 2026-11-06
**Phase:** Week 1: project setup and data layer

## Done

- [x] SRS written, with open questions settled as assumptions
- [x] Class Specs approved (10 classes, 5 enums)
- [x] `AGENTS.md` written
- [x] Full class diagrams in `docs/class-diagram.md`, `.html` and `.pdf` (27 pages): layers, use cases, domain model in 3 parts, 2 state diagrams, 8 use-case class diagrams, layer reference, DTO/ViewModel/configuration catalog
- [x] `src/TourGuide.DAL/TourGuide.DAL.csproj` created (.NET 8, EF Core SQL Server, Identity)
- [x] Enums: `VerificationStatus`, `DocumentType`, `BookingType`, `BookingStatus`, `CancellationReason`
- [x] `Constants/Roles.cs`
- [x] `.config/dotnet-tools.json` (dotnet-ef 8.0.11)
- [x] Project moved to `D:\TourGuide`
- [x] Skills `manage-project` and `build` installed in `.claude/skills/` (packages kept in `skills/`)
- [x] `docs/SRS.md` and `docs/CLASSES_SPECS.md` exported from the shared docs
- [x] `docs/WORK_ORDER.md` (+ `.html`, `.pdf`): 4 phases, 15 features, 29 cycles (made with `manage-project`)

## In progress

- [ ] Cycle 1.0.a — Data model (`docs/WORK_ORDER.md`)

## Next

Follow `docs/WORK_ORDER.md` in order. With the `build` skill: `plan <cycle>` → `build <cycle>` → `review <cycle>` → `done`.

1. Cycle 1.0.a — Data model (entities, configurations, `AppDbContext`, repositories, Unit of Work, `InitialCreate`)
2. Cycle 1.0.b — App skeleton (solution, BLL and Web projects, `Program.cs`, layout)
3. Cycle 1.0.c — Seed data (blocked on the lists below)
4. Cycle 1.1.a — Log in / log out (FR-03)

## Decisions log

Newest first. Add the team's decisions here; the full documents hold the details.

| Date | Decision |
| --- | --- |
| 2026-11-06 | Class diagrams define method signatures, controllers per area, DTOs and ViewModels; `PagedList<T>` and `GuideSearchQuery` live in DAL; `Actor` enum (BLL, not stored) for the state machine. |
| 2026-11-06 | Validation in 4 layers: client (jQuery Unobtrusive) → ViewModel DataAnnotations → FluentValidation in BLL → domain rules. AutoMapper 14.0.0 (not 15+, which is paid) replaces manual mapping. FluentValidation 11.11.0, called explicitly (no `FluentValidation.AspNetCore`). Full catalog of services, repositories, validators and profiles in `AGENTS.md`. |
| 2026-11-06 | Patterns: generic + specific Repository, Unit of Work, constructor DI with one `Add...` extension per layer, service interfaces, Result pattern, Options pattern, `BookingStateMachine`, `TimeProvider`, swappable `IPaymentGateway` and `IFileStorage`, manual mapping (later replaced by AutoMapper). Replaces "no repositories". |
| 2026-11-06 | N-Tier: Web → BLL → DAL. Services return DTOs. |
| 2026-11-06 | .NET 8, SQL Server, no automated tests |
| 2026-11-06 | Class Specs approved, including the 10 decisions in section 9 |
| 2026-11-06 | SRS assumptions accepted (pricing, limits, 24 h deadlines, USD, ASP.NET Core MVC) |

## Known issues

- The seed lists of languages and specializations are not decided yet. The team needs to give them before the seed step.
- The number and emails of admin accounts to seed are not decided yet.
- .NET 8 support ends on 2026-11-10.
- `TourGuide.DAL` has not been built yet.
- The `build` skill reads `docs/STATE.md`, but this file is at the project root (`AGENTS.md` points here). One of them needs to change.
- The skills mention tests per cycle; the team chose no automated tests, so `WORK_ORDER.md` uses manual checks instead.

## Cycle log

Newest first. One row per work cycle.

| Date | What changed | Build |
| --- | --- | --- |
| 2026-11-06 | Tried a 6-member version of `docs/WORK_ORDER.md`, then restored the cycles version on request; added `WORK_ORDER.html` and `.pdf`; `.build/make_html.py` now takes a doc name | PDF 11 pages |
| 2026-11-06 | Installed `manage-project` and `build` skills; exported `docs/SRS.md`, `docs/CLASSES_SPECS.md`; wrote `docs/WORK_ORDER.md` | Not run (no code changed) |
| 2026-11-06 | Expanded `docs/class-diagram.md` by use case and state; added `docs/class-diagram.html`, `docs/class-diagram.pdf`, `.build/make_html.py`; added `ReapplyValidator`, `PagedList<T>`, `GuideSearchQuery` to `AGENTS.md` | All 20 diagrams render; PDF 27 pages |
| 2026-11-06 | Added `docs/class-diagram.md` (UML: domain model + Booking feature across layers), also in Class Specs section 10 | Diagrams render (Mermaid 11) |
| 2026-11-06 | Added the 4-layer validation, AutoMapper, FluentValidation, packages table and full catalog to `AGENTS.md`; updated "Next" | Not run |
| 2026-11-06 | Added Repository, Unit of Work, DI and other patterns to `AGENTS.md` → "Architecture"; updated "Next" | Not run |
| 2026-11-06 | Created DAL project, enums, `Roles`, EF tool manifest, `AGENTS.md`, `STATE.md` | Not run |
