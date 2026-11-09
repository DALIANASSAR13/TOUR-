# STATE — 2026-11-09

**Done** (code written, not built or checked yet)
- 2.8.a List my bookings (FR-15) — `tasks/FR-15-2.8.a.md`
- 2.6.a Price preview (FR-12) — `tasks/FR-12-2.6.a.md`; + blocked negative/decimal hours and days

**Next**
1. Switch branch: `git switch -c Booking-Request` (2.6.a changes are uncommitted on `main`)
2. `dotnet build`, then check 2.6.a AC1–AC10 at `/Tourist/Bookings/Create?guideProfileId=<id>`
3. Fix review items 1 (route `/Create/{id}`) and 4 (no hint when Days is empty)
4. Answer 2.6.b questions, then `build FR-11 2.6.b` — plan in `tasks/FR-11-2.6.b.md`

**Open questions**
- 2.6.a route: `?guideProfileId=` or `/Create/{id}`? 2.5's Book button must match
- 2.6.b: 24 h check — validator reads the guide time zone via `IUnitOfWork`, or move it to the service?
- 2.6.b: `"Create"` rule set; client `FutureDate` only a rough filter; clock-change times; duplicate Pending requests allowed?
- 2.3: guide price must be > 0 (`GuideRules.IsBookable` assumes it)
- 2.8.a: AC1–AC12 still need a build + login (1.1.a)
