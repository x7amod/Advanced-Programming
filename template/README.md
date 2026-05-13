# Taalam Design System

> Professional training & certification management platform — Bahrain.

Taalam is a web app for managing the full lifecycle of professional training: courses, instructors, classrooms, course sessions, trainee enrollments, assessments, payments, and certifications. It serves three roles — **Trainee**, **Instructor**, and **Training Coordinator** — each with a tailored dashboard and navigation.

The brand voice is **clean, minimal, professional** — closer to a modern SaaS dashboard (Linear, Stripe, Notion-for-business) than a traditional LMS. Teal is the single accent on a near-white canvas; type does the heavy lifting; cards and chrome stay quiet so data can speak.

---

## Sources

- **Codebase:** `x7amod/Advanced-Programming` (master) — ASP.NET Core MVC + Web API + SQL.
  - `MVC Frontend/` — scaffold only (default Bootstrap starter; no real UI implemented yet). The design system fills this gap.
  - `Web API/Models/` — full domain model (Course, CourseSession, Enrollment, Trainee, Instructor, Coordinator, CertificationTrack, Assessment, PaymentRecord, PaymentTransaction, Notification, Waitlist, Classroom, Category, SubjectArea, …). This is the **source of truth for entity shapes, statuses, and field names** used in screens.
  - `SQL/1_schema.sql`, `SQL/2_seed.sql` — schema + sample data.
  - `erd.pdf` — entity-relationship diagram.
- **Spec:** brand colors, typography, component guidance, page list, and role matrix were provided directly by the project owner (see CONTENT FUNDAMENTALS / VISUAL FOUNDATIONS below).

> ⚠️ The MVC frontend has no real screens yet — only the default ASP.NET scaffold and Bootstrap. All UI in this design system is a **first-pass design**, not a recreation, grounded in the domain models and the provided spec.

---

## Roles & access

| Role | Lands on | Sees |
|---|---|---|
| **Trainee** | Trainee Dashboard | Browse courses, enrollments, certifications, payments, results |
| **Instructor** | Instructor Dashboard | My sessions, session detail, record assessment |
| **Training Coordinator** | Coordinator Dashboard | Full management: users, courses, sessions, enrollments, payments, certification tracks, reports |

Coordinator is the senior role — full read/write across the platform.

---

## CONTENT FUNDAMENTALS

The product is for working professionals, training providers, and HR/L&D coordinators in Bahrain. Copy is **professional, neutral, and instructive** — never breezy, never marketing-heavy.

**Voice & tone**
- Direct and informative. Tells the user what the screen is for, then gets out of the way.
- Second person ("Your enrollments", "Record an assessment") for trainee/instructor surfaces; third person / object-first ("Course sessions", "Payment records") for coordinator/admin surfaces.
- No exclamation marks. No "Awesome!", "Let's go!", "Woohoo!". Confirmation toasts are flat: "Enrollment confirmed.", "Payment received.", "Assessment recorded."
- Bahraini English: British-leaning spelling where it matters (enrolment vs enrollment — the codebase uses **Enrollment** so we follow the code), 24-hour time, BHD currency.

**Casing**
- **Sentence case** for everything: page titles, section headings, buttons, form labels, table column headers, menu items.
  - "Browse courses" not "Browse Courses". "Record assessment" not "Record Assessment".
- ALL-CAPS only for very small eyebrow labels (status badge text uses normal case, not caps).
- Proper nouns capitalised: Taalam, Bahrain, names of certification tracks ("Occupational Safety & Health, Level 2").

**Pronouns**
- "My …" for the trainee/instructor's own things in the sidebar: My enrollments, My certifications, My payments, My results, My sessions, My profile.
- Bare nouns for coordinator management surfaces: Users, Courses, Sessions, Enrollments, Payments.

**Numbers, dates, currency**
- Currency: **BHD 120.000** (Bahraini Dinar, three decimal places — Bahrain convention). Never `$`.
- Dates: **6 May 2026** in body copy; **06/05/2026** in dense tables. ISO `2026-05-06` only in admin/export contexts.
- Times: **14:00 – 17:00** (24h, en-dash with hair spaces).
- Counts: "12 enrolled / 20 capacity", "3 of 5 required courses completed".

**Status language** (matches the schema's status enums)
- Enrollment: Enrolled · Waitlisted · Completed · Dropped · Cancelled
- Payment: Pending · Partially paid · Paid · Overdue · Refunded
- Session: Scheduled · In progress · Completed · Cancelled
- Certification: In progress · Awarded · Expired · Revoked
- Assessment result: Pass · Fail · Pending

**Emoji & icons in copy**
- **No emoji.** Anywhere. Not in toasts, not in empty states, not in marketing.
- Icons are line icons from Lucide (see ICONOGRAPHY).

**Empty states**
- One short sentence describing what the user would see here, plus one primary action if applicable.
  - "No enrollments yet. Browse the catalogue to get started." → [Browse courses]
  - "No assessments to record. Your sessions will appear here once they're in progress."

**Examples (good)**
- "Your enrollment for *Project Management Fundamentals* has been confirmed. Payment is due by 20 May 2026."
- "12 trainees waitlisted. Add a new session to clear the queue."
- "Awarded on 14 March 2026. Valid for 24 months."

**Examples (avoid)**
- "🎉 You're all set! Welcome aboard!"
- "Oops! Looks like something went wrong 😬"
- "AWESOME WORK — your certificate is on its way!"

---

## VISUAL FOUNDATIONS

A clean, minimal SaaS dashboard aesthetic. Teal is the **only** brand colour; everything else is greyscale + semantic status colours.

**Colour**
- Primary teal `#0D9488`, dark teal `#0F766E` (hover/press, focused borders).
- Background `#F8F9FA` (page), `#FFFFFF` (cards / surfaces).
- Text `#1A1A2E` (primary, near-black with a faint navy cast), `#6B7280` (secondary), `#9CA3AF` (tertiary/placeholder).
- Border `#E5E7EB` (1px hairlines on cards, tables, inputs).
- Semantic: success `#10B981`, warning `#F59E0B`, danger `#EF4444`, info `#3B82F6`.
- **No gradients** anywhere — flat fills only. No glassmorphism, no blurs.

**Type**
- **Inter** (variable). System sans-serif fallback. We use weights 400 / 500 / 600 / 700.
- Display 32/40/700 → page titles. H1 24/32/600, H2 20/28/600, H3 16/24/600.
- Body 14/22/400 (default), large body 16/24/400, small 13/20/400, micro 12/16/500 (badges, eyebrows, table headers).
- Numbers in tables and metrics use **tabular-nums** (`font-variant-numeric: tabular-nums`).
- Letter-spacing: tight on display (-0.01em), normal on body, +0.04em uppercase for tiny labels.

**Spacing**
- 4px base scale: 4 · 8 · 12 · 16 · 20 · 24 · 32 · 40 · 48 · 64.
- Card inner padding: 24px (desktop), 16px (mobile).
- Form row gap: 16px. Section gap: 32px.

**Backgrounds**
- Pages: flat `#F8F9FA`. No imagery, no patterns.
- Auth pages (login, register) use a **split layout**: form on the left over white, a teal panel on the right with a soft geometric Bahrain-inspired SVG motif (subtle, low-contrast, ~6% white-on-teal).
- Empty states use a small line illustration only when meaningful — otherwise a single icon + sentence.

**Animation**
- Subtle. 150ms for hover/focus, 200ms for menu/modal enter, 250ms for sidebar transitions. Easing `cubic-bezier(0.4, 0, 0.2, 1)` (Material standard). No bounces. No springs. No parallax.
- Skeleton loaders fade in at 300ms; never spinners on full pages.

**Hover states**
- Buttons: primary darkens to `#0F766E`; secondary fills its border colour at 8% (`rgba(13,148,136,0.08)`).
- Rows in tables: background `#F8F9FA`.
- Cards: no lift, no shadow change. Optional `border-color` deepen to `#D1D5DB`.

**Press / active states**
- Primary buttons darken further to `#115E59`. No shrink, no scale transform.
- Focus rings: 2px teal ring with 2px white offset (`box-shadow: 0 0 0 2px #fff, 0 0 0 4px #0D9488`).

**Borders**
- 1px solid `#E5E7EB` on cards, tables, inputs.
- Dividers: same colour, never thicker than 1px.

**Shadows / elevation**
- Cards: `0 1px 3px rgba(0,0,0,0.06), 0 1px 2px rgba(0,0,0,0.04)` (xs).
- Popovers / dropdowns: `0 8px 16px rgba(0,0,0,0.08), 0 2px 4px rgba(0,0,0,0.06)` (md).
- Modals: `0 20px 40px rgba(0,0,0,0.12), 0 4px 8px rgba(0,0,0,0.06)` (lg).
- **No coloured shadows.** No teal-tinted glows.

**Layout**
- Authenticated app: fixed 240px sidebar (left) + 64px top bar + scrolling main. Sidebar collapses to a 64px icon rail < 1024px.
- Public/auth pages: split 1fr/1fr on desktop, single column < 768px.
- Max content width 1280px inside main, with 32px gutters.

**Transparency & blur**
- Used sparingly: top bar gets `backdrop-filter: blur(8px)` over a 90% white when content scrolls under it. Otherwise opaque surfaces.

**Imagery**
- Almost none — this is a data product. When used (course thumbnails, certification preview), keep it cool, professional, neutral. No people-stock-photo. Solid colour blocks or muted illustration are preferred placeholders.

**Corner radii**
- 4px — inputs, badges, small chips
- 8px — buttons, dropdowns, table cells (outer)
- 12px — cards, modals, panels
- 9999px — avatars, status dots

**Cards**
- White, 1px `#E5E7EB` border, 12px radius, xs shadow, 24px inner padding.
- Card header: 16/24/600 title + optional secondary action right-aligned. 16px gap below header before content.

---

## ICONOGRAPHY

The codebase ships no custom icon set. We use **Lucide** (line icons, 1.5px stroke, rounded joins) — it pairs cleanly with Inter and matches the minimal SaaS feel.

- Loaded via CDN: `https://unpkg.com/lucide@latest`.
- Default size: 20px in nav/buttons, 16px inline, 24px on empty-state hero.
- Stroke: `currentColor`, 1.5px. Never filled.
- Common icons: `home`, `book-open`, `users`, `calendar-days`, `clipboard-check`, `award`, `credit-card`, `bar-chart-3`, `bell`, `search`, `chevron-right`, `chevron-down`, `x`, `check`, `plus`, `pencil`, `trash-2`, `download`, `log-out`, `user`, `settings-2`.
- **No emoji.** Never used in product copy or icons.
- **No unicode glyph icons** (no ★ ✓ ↑ in place of real icons). Real icons only.
- **No colored icons in nav.** Sidebar icons inherit text colour; the active item gets teal text + teal icon together.
- Status indicators are 8px circular dots in semantic colour next to text — not icons.

> **Substitution flag:** Lucide is a substitution. The codebase has no icon library committed. If Taalam later adopts a different set (e.g. Phosphor, Heroicons), update `colors_and_type.css` and the kit accordingly.

---

## Index

| File / folder | What's in it |
|---|---|
| `README.md` | This file — context, content rules, visuals, iconography. |
| `colors_and_type.css` | All design tokens as CSS custom properties. Import this. |
| `SKILL.md` | Skill manifest (compatible with Claude Code Agent Skills). |
| `assets/` | Logo, favicon, brand SVG motif. |
| `preview/` | Design-system cards rendered in the Design System tab. |
| `ui_kits/web_app/` | Hi-fi recreations of Taalam screens — auth, three role dashboards, key flows. Open `index.html`. |

---

## Font substitution flag

We use **Inter** via Google Fonts (`https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap`). The codebase doesn't ship its own font file. If Taalam licenses a custom typeface later, drop the `.woff2` files into `fonts/` and update the `@font-face` block in `colors_and_type.css`.
