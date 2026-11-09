# STATE — 2026-11-09

**Done** (code written, not built or checked yet)
- 2.7.b Accept (FR-13) — `tasks/FR-13-2.7.b.md`; overlap warning on card + refusal; RowVersion → `ConcurrencyException`; seed: Sophie→Omar overlapping Pending; Mona in DevLogin
- 2.7.a Guide requests list (FR-16) — `tasks/FR-16-2.7.a.md`; `/Guide/BookingRequests`, nav "Requests"
- 2.8.a List my bookings (FR-15) — `tasks/FR-15-2.8.a.md`
- 2.6.a Price preview (FR-12) — `tasks/FR-12-2.6.a.md`; 2.6.b send request (FR-11) code exists at `/Tourist/Bookings/Create/{id}`, unverified

**Next**
1. Branches: 2.6 → `Booking-Request`, 2.7 → `Guide-Requests` (all changes uncommitted on `main`)
2. `dotnet ef database drop` + `update` (re-seed), `dotnet build`, then check 2.6.a, 2.7.a AC1–AC14, 2.7.b AC1–AC14
3. Fix 2.6.a review items 1 (route) and 4 (no hint when Days is empty); verify 2.6.b
4. `plan FR-13 2.7.c` (Reject), then 2.8.b (Withdraw), 2.5 (Book button)

**Open questions**
- 2.7.a defaults to confirm: unapproved guide sees notice; tourist name + country only; newest first; no tiles
- 2.6.a route: `?guideProfileId=` or `/Create/{id}`? 2.5's Book button must match
- 2.6.b: 24 h check in validator via `IUnitOfWork` or service? duplicate Pending requests allowed?
- Known issue: two different overlapping requests accepted at the same instant can both pass (MVP accepted risk)
