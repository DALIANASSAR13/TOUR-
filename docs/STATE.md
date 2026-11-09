# STATE — 2026-11-10

**Done** (code written; ACs not checked yet)
- 2.9.a Checkout details (FR-14) — `tasks/FR-14-2.9.a.md`; Tourist `/Tourist/Payments/Checkout/{id}`, read-only, Pay button disabled until 2.9.b; "Pay now" on My Bookings links to it (Accepted, not expired)
- 2.5.a Guide details (FR-09) — `tasks/FR-09-2.5.a.md`; 2.4.a Search (FR-10) — `tasks/FR-10-2.4.a.md`
- 2.8.a–b My bookings, Withdraw (FR-15); 2.7.a–c Requests, Accept, Reject (FR-16, FR-13)
- 2.6.a Price preview (FR-12); 2.6.b send request (FR-11), unverified

**Next**
1. `dotnet build`; run once in Development so `AssertConfigurationIsValid()` checks the new `CheckoutDto → CheckoutViewModel` map (`MemberList.Source`)
2. Branches: 2.4 `Search-Guides`, 2.5 `Guide-Profile`, 2.6 `Booking-Request`, 2.7 `Guide-Requests`, 2.8 `List-Bookings`, 2.9 name not chosen (all on `main`)
3. Check ACs end to end: Search → profile → Book → request → guide accepts → My Bookings → Pay now → checkout (2.4.a – 2.9.a)
4. Fix 2.6.a review items 1 (route) and 4 (no hint when Days is empty)
5. `plan FR-14 2.9.b` (Add payment: card form, `PayAsync`, receipt)

**Open questions**
- 2.9.a: branch name (`Payment`?); proposed answers to Q1–4, Q6 were built without confirmation — confirm or change
- 2.9.a: `IBookingRepository.GetWithGuideAsync` and `Checkout(int id)` differ from the class diagram — update the diagram?
- 2.9.a: "This booking was not found." is a literal in two services — shared message constants?
- 2.5.a/2.4.a defaults taken: `GuidesController` (AGENTS.md still lists `GuideProfile` — update?), page sizes 9/5, reviewer = first name · country
- BR-02 lives in DAL `Searchable` and BLL `GuideRules` — keep in sync
- 2.8.b: expired Pending note "within 0 h" — reword? 2.6.b: 24 h check in validator or service? duplicate Pending requests allowed?
- Known issue: two overlapping requests accepted at the same instant can both pass (MVP accepted risk)
