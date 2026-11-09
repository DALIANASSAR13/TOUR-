# STATE — 2026-11-09

**Done** (code written; ACs not checked yet)
- 2.5.a Guide details (FR-09) — `tasks/FR-09-2.5.a.md`; public `/Guides/Details/{id}`, 5 reviews/page, Book → `/Tourist/Bookings/Create/{id}`; Search + My Bookings links repointed
- 2.4.a Search (FR-10) — `tasks/FR-10-2.4.a.md`; public `/Search`, 9/page; `ICountryProvider` (+`GetName`); seed +14 guides
- 2.8.b Withdraw (FR-15) — `tasks/FR-15-2.8.b.md`; 2.8.a My bookings; 2.7.a–c Requests, Accept, Reject (FR-16, FR-13)
- 2.6.a Price preview (FR-12); 2.6.b send request (FR-11), now reachable via Book, unverified

**Next**
1. `dotnet build`; branches: 2.4 `Search-Guides`, 2.5 `Guide-Profile`, 2.6 `Booking-Request`, 2.7 `Guide-Requests`, 2.8 `List-Bookings` (all on `main`)
2. Check ACs end to end: Search → profile → Book → request → guide accepts → My Bookings (2.4.a, 2.5.a, 2.6.a/b, 2.7.a–c, 2.8.a–b)
3. Fix 2.6.a review items 1 (route) and 4 (no hint when Days is empty)
4. `plan FR-14 2.9` (Pay)

**Open questions**
- 2.5.a/2.4.a defaults taken: `GuidesController` (AGENTS.md still lists `GuideProfile` — update?), page sizes 9/5, reviewer = first name · country
- BR-02 lives in DAL `Searchable` and BLL `GuideRules` — keep in sync
- 2.8.b: 2.9 must refuse payment on a withdrawn booking; expired Pending note "within 0 h" — reword?
- 2.6.b: 24 h check in validator or service? duplicate Pending requests allowed?
- Known issue: two overlapping requests accepted at the same instant can both pass (MVP accepted risk)
