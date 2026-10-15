# Event Platform - a RaceDay Event Management System

## Project Description
RaceDay is a web-based event management system for the South African road running, walking, swimming, triathlon and cycling community. The platform allows Event Organisers to create and manage events while Participants can browse, enrol, and track their personal performance history.

## User Roles

### Organiser
- Register and log in
- Create, edit, and delete events they own
- Manage event categories (age/distance groups)
- View all enrolments for their own events
- Capture participant results (finish times and positions)

### Participant
- Register and log in
- Browse upcoming events
- Enrol in events by selecting a category
- View personal enrolment history
- Track personal race results
- Manage their profile

---

## Part 1 Deliverables

### Entity Relationship Diagram (ERD)
- File: `docs/RaceDay_ERD.pdf`
- Contains 7 entities: Role, Users, EventType, Event, Category, Enrolment, Result
- Shows primary keys, foreign keys, and cardinality

### API Endpoint Plan
- File: `docs/RaceDay_API_Endpoint_Plan.pdf`
- Covers: Authentication, User Profile, Events, Categories, Enrolments, Results

### SQL Database Script
- File: `docs/RaceDay_Database.sql`
- Creates RaceDayDB with 7 tables and seed data

---

## Part 2 - RESTful API

### Technologies Used
- ASP.NET Core Web API (.NET 8)
- Entity Framework Core (Code-First)
- MySQL Database (Pomelo.EntityFrameworkCore.MySql)
- Session-based authentication
- Password hashing via ASP.NET Core Identity's `PasswordHasher<T>`
- Swagger / OpenAPI
- xUnit for unit testing
- GitHub Actions CI/CD

### Project Structure
```
event-platform/
├── docs/                               # Part 1 planning documents
│   ├── RaceDay_ERD.pdf
│   ├── RaceDay_API_Endpoint_Plan.pdf
│   └── RaceDay_Database.sql
│
├── RaceDay.Api/                        # Web API project
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── ProfileController.cs
│   │   ├── EventsController.cs
│   │   ├── CategoriesController.cs
│   │   ├── EnrolmentsController.cs
│   │   └── ResultsController.cs
│   ├── Data/
│   │   ├── RaceDayDbContext.cs
│   │   └── DbSeeder.cs
│   ├── Helpers/
│   │   └── SessionHelper.cs
│   ├── Models/
│   │   ├── Role.cs
│   │   ├── User.cs
│   │   ├── EventType.cs
│   │   ├── Event.cs
│   │   ├── Category.cs
│   │   ├── Enrolment.cs
│   │   └── Result.cs
│   ├── Migrations/
│   ├── appsettings.json                # Placeholder password (committed)
│   ├── appsettings.Development.json    # Real password (gitignored)
│   └── Program.cs
│
├── RaceDay.Contracts/                  # Shared DTOs
│   └── Dtos/
│       ├── Auth/
│       ├── Profile/
│       ├── Events/
│       ├── Categories/
│       ├── Enrolments/
│       └── Results/
│
├── RaceDay.Api.Tests/                  # Unit tests
│   ├── AuthTests.cs
│   ├── EventTests.cs
│   └── TestSession.cs
│
└── .github/workflows/
    ├── part1-ci.yml
    └── part2-ci.yml
```

---

### Database Setup (MySQL)

**Prerequisites:**
- MySQL Server 8.0 or later
- MySQL Workbench (for viewing data)
- Visual Studio 2022 with .NET 8 SDK

**Steps:**

1. Install and start MySQL Server.
2. Open `RaceDay.Api/appsettings.Development.json` and set your real MySQL password:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Port=3306;Database=RaceDayDB;User=root;Password=YOUR_REAL_PASSWORD;"
     }
   }
   ```
   This file is gitignored and will not be committed.

3. Open the solution in Visual Studio. Set `RaceDay.Api` as the startup project.

4. Open **Tools → NuGet Package Manager → Package Manager Console**.
   Set **Default project** to `RaceDay.Api`.

5. Run the migration:
   ```powershell
   Update-Database
   ```
   Or from terminal:
   ```bash
   cd RaceDay.Api
   dotnet ef database update
   ```

6. Verify in MySQL Workbench that `RaceDayDB` exists with these 7 tables:
   `Roles, EventTypes, Users, Events, Categories, Enrolments, Results`

---

### How to Run the API

1. Open `RaceDay.sln` in Visual Studio 2022.
2. Set `RaceDay.Api` as the startup project.
3. Press **F5** (or Ctrl+F5).
4. Swagger UI opens in the browser at `https://localhost:<port>/swagger`.

---

### Authentication

- Users register via **two distinct endpoints**: one for Organisers, one for Participants.
- Passwords are hashed using ASP.NET Core's `PasswordHasher<User>` before storage.
- On successful login, the server stores `UserId` and `Role` in a server-side session.
- All protected endpoints check the session and enforce role rules.
- Wrong-role access returns **403 Forbidden**; no session returns **401 Unauthorized**.

---

### API Endpoints

#### Authentication
| Method | Route | Access |
|--------|-------|--------|
| POST | /api/auth/register/organiser | Public |
| POST | /api/auth/register | Public |
| POST | /api/auth/login | Public |
| POST | /api/auth/logout | Any logged-in |

#### Profile
| Method | Route | Access |
|--------|-------|--------|
| GET | /api/profile | Any logged-in |
| PUT | /api/profile | Any logged-in |

#### Events
| Method | Route | Access |
|--------|-------|--------|
| GET | /api/events | Any logged-in |
| GET | /api/events/{id} | Any logged-in |
| GET | /api/events/mine | Organiser only |
| POST | /api/events | Organiser only |
| PUT | /api/events/{id} | Organiser only (owner) |
| DELETE | /api/events/{id} | Organiser only (owner) |

#### Categories
| Method | Route | Access |
|--------|-------|--------|
| GET | /api/events/{eventId}/categories | Any logged-in |
| POST | /api/events/{eventId}/categories | Organiser only (owner) |
| PUT | /api/categories/{id} | Organiser only (owner) |
| DELETE | /api/categories/{id} | Organiser only (owner) |

#### Enrolments
| Method | Route | Access |
|--------|-------|--------|
| POST | /api/enrolments | Participant only |
| GET | /api/enrolments/mine | Participant only |
| GET | /api/enrolments/event/{eventId} | Organiser only (owner) |
| PUT | /api/enrolments/{id}/status | Organiser only (owner) |

#### Results
| Method | Route | Access |
|--------|-------|--------|
| POST | /api/results | Organiser only |
| GET | /api/results/mine | Participant only |
| GET | /api/results/event/{eventId} | Organiser only (owner) |

---

### Swagger

All endpoints are visible and testable through Swagger UI when the API runs in Development mode. Each endpoint shows its request body, expected response, and status codes.

---

### Unit Testing

Tests are located in `RaceDay.Api.Tests`. They cover:

- Registering an Organiser (success)
- Registering a Participant (success)
- Duplicate email on registration (409 Conflict)
- Login with valid credentials (200 OK)
- Login with wrong password (401 Unauthorized)
- Participant trying to create an event (403 Forbidden)
- Organiser creating an event (201 Created)
- Unauthenticated user accessing events (401 Unauthorized)

Run locally:
- Visual Studio: **Test → Test Explorer → Run All**
- Terminal: `dotnet test`

---

### CI/CD

The GitHub Actions workflow (`.github/workflows/part2-ci.yml`) runs on every push:

1. Checks out the repository
2. Installs .NET 8
3. Restores NuGet packages
4. Builds the solution
5. Runs all unit tests

**Screenshot of Successful CI/CD Build:**

![Part 2 CI/CD Build](INSERT_YOUR_PART_2_SCREENSHOT_URL_HERE)

---

### Video Presentation

[Watch the Part 2 demonstration video](INSERT_YOUR_PART_2_YOUTUBE_LINK_HERE)

---

## Part 1 Video

## CI/CD

The GitHub Actions workflow validation **Screenshot of Successful CI Run:**
<img width="1242" height="485" alt="CI/CD Build" src="https://github.com/user-attachments/assets/1e3f6cd3-4644-41d7-9667-5c827456c3b5" />

## Video Presentation

[Watch PART 1 demonstration video](https://youtu.be/4u8yH-S_8nI)

[![Watch PART 1 Demonstration Video](https://img.shields.io/badge/Watch-Video-red)](https://youtu.be/4u8yH-S_8nI)
