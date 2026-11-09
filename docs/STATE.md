# STATE — 2026-11-09

**Done** (code written, not built or checked yet)
- 2.7.a Guide requests list (FR-16) — `tasks/FR-16-2.7.a.md`; `/Guide/BookingRequests`, nav "Requests"
- 2.8.a List my bookings (FR-15) — `tasks/FR-15-2.8.a.md`
- 2.6.a Price preview (FR-12) — `tasks/FR-12-2.6.a.md`; + blocked negative/decimal hours and days

**Next**
1. Branches: 2.6.a → `Booking-Request`, 2.7.a → `Guide-Requests` (all changes uncommitted on `main`)
2. `dotnet build`, then check 2.6.a AC1–AC10, 2.7.a AC1–AC14 (DevLogin as Omar / Ahmed / Laila / Emma)
3. Fix 2.6.a review items 1 (route `/Create/{id}`) and 4 (no hint when Days is empty)
4. Answer 2.6.b questions, then `build FR-11 2.6.b`; then `plan 2.7.b` (Accept)

**Open questions**
- 2.7.a: confirm defaults — unapproved guide sees notice; tourist name + country only; newest first; no tiles
- 2.7.a: DevLogin lacks Mona (Confirmed/Cancelled); guides land on Home, not Requests
- 2.6.a route: `?guideProfileId=` or `/Create/{id}`? 2.5's Book button must match
- 2.6.b: 24 h check in validator via `IUnitOfWork`, or in the service? `"Create"` rule set; duplicate Pending allowed?
- 2.3: guide price must be > 0 (`GuideRules.IsBookable` assumes it)
