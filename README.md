# Advanced Programming Project | Training & Certification Platform

## Group Information
Section 5 - Group 3
| Student ID | Name |
|-----------|------|
| 202301297 | Hajar Aljafen |
| 202301706 | Noora Humaid |
| 202303581 | Ahmed Alhalal |
| 202302860 | Abdulla Alaseeri |
| 202300450 | Abdulla Ahmed |

---

## Seeded Accounts

<details>
<summary>Show Seeded Account Credentials</summary>

### Default Password
`Pass@1234`

<details>
<summary>Training Coordinators</summary>

| Email |
|---------|
| noura.aldosari@bticademy.bh |
| yousif.janahi@bticademy.bh |

</details>

<details>
<summary>Instructors</summary>

| Email |
|---------|
| mohammed.alsayed@bticademy.bh |
| fatima.almansoor@bticademy.bh |
| ahmed.alkhalifa@bticademy.bh |
| sara.alhaddad@bticademy.bh |

</details>

<details>
<summary>Trainees</summary>

| Email |
|---------|
| ali.almutawa@bticademy.bh |
| mariam.alkooheji@bticademy.bh |
| hassan.alnajjar@bticademy.bh |
| noor.alkhaldi@bticademy.bh |
| abdullah.alsuwaidi@bticademy.bh |
| zainab.almahmood@bticademy.bh |
| khalid.alrumaihi@bticademy.bh |
| huda.alansari@bticademy.bh |

</details>

</details>

---

## First Time Setup
### !! Note If you already have the old DB file created, remove it first !!
- Using `database drop --force`
1. Open NuGet Package Manager Console (Toolbar at the top -> Tools -> NuGet Package Manager -> NuGet Package Manager Console)
2. Run `Update-Database` to create the database
3. Test by running the project (verify no errors)

---

## Reporting API Endpoints

The Reporting Application authenticates with JWT and reads all report data through the Web API.

| Route | HTTP Method | Purpose | Authentication |
|------|------|------|------|
| `/api/auth/login` | `POST` | Authenticates user credentials and returns JWT token payload (`token`, `email`, `roles`, `expiresIn`). | No (`AllowAnonymous`) |
| `/api/reports/overview` | `GET` | Returns dashboard KPIs for trainees, instructors, courses, sessions, enrollments, certifications, revenue, and pass/completion rates. | Yes (`Training Coordinator`) |
| `/api/reports/enrollments` | `GET` | Returns enrollment summary and course-level enrollment outcomes (completed, dropped, active, completion rate). | Yes (`Training Coordinator`) |
| `/api/reports/instructors` | `GET` | Returns instructor workload and performance metrics (sessions, trainees, assessments, pass rates). | Yes (`Training Coordinator`) |
| `/api/reports/certifications` | `GET` | Returns certification track performance and status distribution (issued, eligible, in progress, expired). | Yes (`Training Coordinator`) |
| `/api/reports/revenue` | `GET` | Returns revenue summary plus course-level and month-level invoiced/collected/outstanding figures. | Yes (`Training Coordinator`) |
