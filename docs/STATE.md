# STATE — 2026-11-09

**Done** (code written, not built or checked yet)
- 2.8.b Withdraw (FR-15) — `tasks/FR-15-2.8.b.md`; inline confirm; expired bookings refused + button hidden (left to job); new `BookingExpiry`, shared `SaveStatusChangeAsync` (Accept/Reject touched)
- 2.7.c Reject (FR-13) — `tasks/FR-13-2.7.c.md`; 2.7.b Accept — `tasks/FR-13-2.7.b.md`
- 2.7.a Guide requests list (FR-16) — `tasks/FR-16-2.7.a.md`; 2.8.a List my bookings (FR-15) — `tasks/FR-15-2.8.a.md`
- 2.6.a Price preview (FR-12) — `tasks/FR-12-2.6.a.md`; 2.6.b send request (FR-11) at `/Tourist/Bookings/Create/{id}`, unverified

**Next**
1. Branches: 2.6 → `Booking-Request`, 2.7 → `Guide-Requests`, 2.8 → `List-Bookings` (all uncommitted on `main`)
2. `dotnet ef database drop` + `update`, `dotnet build`, then check 2.6.a, 2.7.a–c, 2.8.a–b ACs (re-check 2.7.b/c after shared-save refactor)
3. Fix 2.6.a review items 1 (route) and 4 (no hint when Days is empty); verify 2.6.b
4. 2.5 (Book button), then 2.9 (Pay)

**Open questions**
- 2.8.b: 2.9 must refuse payment on a withdrawn booking — confirm; expired Pending note says "within 0 h" — reword?
- 2.7.a defaults to confirm: unapproved guide notice; tourist name + country only; newest first
- 2.6.a route: `?guideProfileId=` or `/Create/{id}`? 2.5's Book button must match
- 2.6.b: 24 h check in validator or service? duplicate Pending requests allowed?
- Known issue: two overlapping requests accepted at the same instant can both pass (MVP accepted risk)
