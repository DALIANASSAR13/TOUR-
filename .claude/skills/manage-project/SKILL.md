---
name: "manage-project"
description: "Give it the SRS; it suggests the work order as Phases > Features > Cycles > Tasks."
---

Input: docs/SRS.md (+ docs/CLASSES_SPECS.md if it exists). Nothing else.
This is a suggestion for the work order only.
Don't assign people, dates, or sprints. Don't write code.

Step 1: Phases (the order of work)
- Order FRs by dependency first, then by value, into phases:
  Phase 1 Foundations (what other FRs need) → Phase 2 Core flow
  → Phase 3 Secondary → Phase 4 Later (only if time is left)
- For each FR list: the BRs it enforces, the NFRs that touch it, needs: FR-xx
- NFRs that apply everywhere (auth, validation, logging) are "Cross-cutting".
  Place each one in the phase where it is first needed.

Step 2: Feature → Cycles
- Represent each feature as cycles: one action on one entity.
  Add · List · Details · Edit · Delete/Cancel (only what the FR needs)
- One cycle owns everything about that action, end to end:
  ViewModel + validation → Controller GET/POST → View
  → Service + its BRs → Repository/DbContext → tests for its BRs
- Choose the simplest way that satisfies the FR.
  No extra layers, patterns, or packages.

Step 3: Cycle → Tasks
- Small tasks, each one finishable and testable on its own
- Layer order: DAL → BLL → PL → Test

Output → docs/WORK_ORDER.md, as a tree:
Phase → Features (FR + BR + NFR + needs) → Cycles → Tasks

Next: each cycle goes to Build → "plan FR-02 Add Booking"