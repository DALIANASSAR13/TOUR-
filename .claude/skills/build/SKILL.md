---
name: "build"
description: "Build . Use for any development task on this project: plan, build, review, or end a session."
---

Read first, nothing else: AGENTS.md · docs/STATE.md · the current task file
When planning, also read only this cycle's part of docs/WORK_ORDER.md
and only this entity's part of docs/CLASSES_SPECS.md.
Never scan the whole repo. Never paste docs back. Refer to FR-xx / BR-xx by ID.

"plan FR-xx [Cycle]" →
   1) "What to respect" first, each line with its source file:
      every BR + NFR of this cycle (WORK_ORDER.md),
      the entity's validation (CLASSES_SPECS.md),
      the AGENTS.md rules this cycle touches.
   2) Then write tasks/FR-xx-<cycle>.md: <= 5 steps, files, acceptance criteria
      (one per item in step 1). No code. Wait.
"build FR-xx [Cycle]"  → implement that task file only. Fill its Execution Report.
"review FR-xx [Cycle]" → read task file + git diff only. Numbered findings, no praise.
"done"         → update docs/STATE.md (<= 20 lines): Done · Next · Open questions.

Token rules
- Show changes as diffs, never reprint a whole file
- No recap of my request, no step-by-step summary
- Short answers unless I say "explain"

# Gemini reads the same files:
# GEMINI.md → @./AGENTS.md
#             @./.claude/skills/elwakeel-build/SKILL.md