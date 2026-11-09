# STATE — 2026-11-09

**Done** (code written; DB dropped + re-seeded and app run by team; ACs not checked yet)
- 2.4.a Search (FR-10) — `tasks/FR-10-2.4.a.md`; public `/Search`, 9/page, rating ↓; `ICountryProvider`; seed +14 guides (21 searchable = 3 pages, Egypt = 2)
- 2.8.b Withdraw (FR-15) — `tasks/FR-15-2.8.b.md`; expired refused + hidden; `BookingExpiry`, shared `SaveStatusChangeAsync`
- 2.7.a–c Guide requests, Accept, Reject (FR-16, FR-13); 2.8.a My bookings (FR-15)
- 2.6.a Price preview (FR-12); 2.6.b send request (FR-11) at `/Tourist/Bookings/Create/{id}`, unverified

**Next**
1. Branches: 2.4 → `Search-Guides`, 2.6 → `Booking-Request`, 2.7 → `Guide-Requests`, 2.8 → `List-Bookings` (all on `main`)
2. Check ACs: 2.4.a AC1–AC11 (`/Search`, `?CountryCode=EG&page=2`), 2.6.a, 2.7.a–c, 2.8.a–b
3. `plan FR-09 2.5.a` (guide details + Book button); then fix 2.6.a review items 1, 4; then 2.9 (Pay)

**Open questions**
- 2.5.a: controller name `GuideProfile` (AGENTS, links in 2.4/2.8) or `Guides` (WORK_ORDER)? Book route must match 2.6 (`/Create/{id}`)
- 2.4.a: BR-02 lives in DAL `Searchable` and BLL `GuideRules` — keep in sync; extra repo method `GetSearchableCountryCodesAsync`
- 2.8.b: 2.9 must refuse payment on a withdrawn booking; expired Pending note "within 0 h" — reword?
- 2.6.b: 24 h check in validator or service? duplicate Pending requests allowed?
- 2.7.a defaults to confirm: unapproved guide notice; tourist name + country only; newest first
- Known issue: two overlapping requests accepted at the same instant can both pass (MVP accepted risk)
