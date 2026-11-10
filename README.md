# Event Platform – A RaceDay Event Management System

## Project Description

RaceDay is a web-based event management system for the South African road running, walking, swimming, triathlon and cycling community. The platform allows Event Organisers to create and manage events while Participants can browse, enrol, and track their personal performance history.

The system is developed across three parts, progressing from database and system design (Part 1), to RESTful API development (Part 2), and finally to an MVC web application with Azure Blob Storage and Docker integration (Part 3).

## User Roles

### Organiser
- Register and log in.
- Create, edit, and delete events they own.
- Manage event categories (age/distance groups).
- View all enrolments for their own events.
- Capture participant results (finish times and positions).

### Participant
- Register and log in.
- Browse upcoming events.
- Enrol in events by selecting a category.
- View personal enrolment history.
- Track personal race results.
- Manage their profile and upload a profile picture (where implemented).

---

## Part 1 Deliverables

### Entity Relationship Diagram (ERD)
- File: `docs/RaceDay_ERD.pdf`
- Contains 7 entities: Role, Users, EventType, Event, Category, Enrolment, Result.
- Shows primary keys, foreign keys, and cardinality.

### API Endpoint Plan
- File: `docs/RaceDay_API_Endpoint_Plan.pdf`
- Covers Authentication, User Profile, Events, Categories, Enrolments and Results.

### SQL Database Script
- File: `docs/RaceDay_Database.sql`
- Creates the RaceDayDB database with 7 tables and seed data.

---

## Part 2 – RESTful API

### Technologies Used
- ASP.NET Core Web API (.NET 8).
- Entity Framework Core (Code-First).
- MySQL Database (Pomelo.EntityFrameworkCore.MySql).
- Session-based authentication.
- Password hashing via ASP.NET Core Identity's `PasswordHasher<T>`.
- Swagger / OpenAPI.
- xUnit for unit testing.
- GitHub Actions for CI/CD.

### Project Structure

```text
event-platform/
├── docs/                               # Part 1 planning documents
│   ├── RaceDay_ERD.pdf
│   ├── RaceDay_API_Endpoint_Plan.pdf
│   └── RaceDay_Database.sql
│
├── RaceDay.Api/                        # ASP.NET Core Web API
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
│   ├── Services/
│   │   └── BlobStorageService.cs       # Part 3 Azure integration
│   ├── Migrations/
│   ├── appsettings.json
│   ├── appsettings.Development.json
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
├── RaceDay.Api.Tests/                  # API unit tests
│   ├── AuthTests.cs
│   ├── EventTests.cs
│   └── TestSession.cs
│
├── RaceDay.Mvc/                        # Part 3 MVC application
│   ├── Controllers/
│   ├── Services/
│   │   └── RaceDayApiClient.cs
│   ├── ViewModels/
│   ├── Views/
│   ├── wwwroot/
│   ├── appsettings.json
│   ├── Program.cs
│   └── Dockerfile
│
├── .dockerignore
│
└── .github/
    └── workflows/                      # GitHub Actions CI/CD
        ├── part1-ci.yml
        └── part2-ci.yml
```

### Database Setup (MySQL)

**Prerequisites:**
- MySQL Server 8.0 or later.
- MySQL Workbench (for viewing the database).
- Visual Studio 2022.
- .NET 8 SDK.

### Authentication

- Users register through two distinct endpoints: one for Organisers and one for Participants.
- Passwords are hashed using ASP.NET Core's `PasswordHasher<User>` before storage.
- On successful login, the server stores `UserId` and `Role` in a server-side session.
- Protected endpoints check the session and enforce role-based access rules.
- Wrong-role access returns **403 Forbidden**.
- Requests without an authenticated session return **401 Unauthorized**.

### API Endpoints

The following table documents the API routes and their intended access permissions.

#### Authentication

| Method | Route | Access |
|---|---|---|
| POST | `/api/auth/register/organiser` | Public |
| POST | `/api/auth/register` | Public |
| POST | `/api/auth/login` | Public |
| POST | `/api/auth/logout` | Any logged-in user |

#### Profile

| Method | Route | Access |
|---|---|---|
| GET | `/api/profile` | Any logged-in user |
| PUT | `/api/profile` | Any logged-in user |

#### Events

| Method | Route | Access |
|---|---|---|
| GET | `/api/events` | Any logged-in user |
| GET | `/api/events/{id}` | Any logged-in user |
| GET | `/api/events/mine` | Organiser only |
| POST | `/api/events` | Organiser only |
| POST | `/api/events/{id}/banner` | Organiser (owner only), if implemented |
| PUT | `/api/events/{id}` | Organiser (owner only) |
| DELETE | `/api/events/{id}` | Organiser (owner only) |

#### Categories

| Method | Route | Access |
|---|---|---|
| GET | `/api/events/{eventId}/categories` | Any logged-in user |
| POST | `/api/events/{eventId}/categories` | Organiser (owner only) |
| PUT | `/api/categories/{id}` | Organiser (owner only) |
| DELETE | `/api/categories/{id}` | Organiser (owner only) |

#### Enrolments

| Method | Route | Access |
|---|---|---|
| POST | `/api/enrolments` | Participant only |
| GET | `/api/enrolments/mine` | Participant only |
| GET | `/api/enrolments/event/{eventId}` | Organiser (owner only) |
| PUT | `/api/enrolments/{id}/status` | Organiser (owner only) |

#### Results

| Method | Route | Access |
|---|---|---|
| POST | `/api/results` | Organiser only |
| GET | `/api/results/mine` | Participant only |
| GET | `/api/results/event/{eventId}` | Organiser (owner only) |

### Swagger / OpenAPI

Swagger provides an interactive interface for viewing and testing the API endpoints when the API runs in Development mode.

It allows developers to:
- View available API endpoints.
- Inspect request bodies and response structures.
- Submit requests to test API functionality.
- Review expected HTTP response status codes.

Swagger is accessible through:

`https://localhost:<api-port>/swagger`

### Unit Testing

Tests are located in `RaceDay.Api.Tests` and use xUnit.

The implemented test coverage includes:
- Registering an Organiser successfully.
- Registering a Participant successfully.
- Duplicate email registration (409 Conflict).
- Login with valid credentials (200 OK).
- Login with an incorrect password (401 Unauthorized).
- Participant attempting to create an event (403 Forbidden).
- Organiser creating an event (201 Created).
- Unauthenticated user accessing events (401 Unauthorized).

### CI/CD

GitHub Actions is used to automate the build and testing process.

The workflow performs the following steps:
1. Checks out the repository.
2. Installs the .NET 8 SDK.
3. Restores NuGet packages.
4. Builds the solution.
5. Runs the available unit tests.

**Screenshot of Successful Part 2 CI/CD Build:**

<img width="900" height="416" alt="Part 2 CI/CD Build" src="https://github.com/user-attachments/assets/c0107604-839d-44fe-896f-6d912b0699bf" />

---

## Part 3 – MVC Web Application, Azure Blob Storage and Docker

Part 3 extends the RaceDay system by introducing an ASP.NET Core MVC web application that provides a user-friendly interface for interacting with the RESTful API developed in Part 2.

The MVC application communicates with the API for data operations rather than accessing the database directly.

Azure Blob Storage is used for image storage through the API, while Docker provides a containerised environment for running the MVC application.

### Technologies Used

- ASP.NET Core MVC (.NET 8).
- ASP.NET Core Web API (.NET 8).
- Bootstrap 5 for responsive UI design.
- Shared DTOs through `RaceDay.Contracts`.
- Session-based authentication with API session cookie pass-through.
- Azure Blob Storage for image storage.
- Entity Framework Core and MySQL.
- Docker Desktop using Linux containers.
- GitHub Actions for CI/CD.

### MVC Architecture

The MVC application follows a client-server architecture.
The API remains responsible for database operations, authentication, business logic and communication with Azure Blob Storage.
The MVC application handles the presentation layer and sends requests to the API through `RaceDayApiClient`.

**Application communication flow:**
```text
             USER
               |
               v
       RaceDay.Mvc
       (Presentation)
               |
               v
       RaceDayApiClient
       (API Requests)
               |
               v
         RaceDay.Api
        (Business Logic)
               |
          +----+----+
          |         |
          v         v
       MySQL    Azure Blob
      Database   Storage
```

This separation keeps the MVC application independent of direct database access and allows the API to manage the application's data and business operations.

### Project Structure

The Part 3 implementation introduces the following MVC and Azure-related components:
```text
RaceDay/
│
├── RaceDay.Api/
│   ├── Controllers/
│   ├── Data/
│   ├── Helpers/
│   ├── Models/
│   ├── Services/
│   │   └── BlobStorageService.cs
│   ├── Migrations/
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── Program.cs
│
├── RaceDay.Contracts/
│   └── Dtos/
│
├── RaceDay.Api.Tests/
│
├── RaceDay.Mvc/
│   ├── Controllers/
│   ├── Services/
│   │   └── RaceDayApiClient.cs
│   ├── ViewModels/
│   ├── Views/
│   ├── wwwroot/
│   ├── appsettings.json
│   ├── Program.cs
│   └── Dockerfile
│
├── .dockerignore
│
└── .github/
    └── workflows/
```

### MVC Application Features

The MVC application provides separate navigation and functionality for Organisers and Participants.

**Organiser functionality:**
- Register and log in.
- Access the Organiser dashboard.
- Create and manage events.
- Manage event categories.
- View enrolments for their events.
- Capture participant results.

**Participant functionality:**
- Register and log in.
- Browse available events.
- View event details.
- Enrol in events.
- View personal enrolment history.
- View personal results.
- Manage their profile.

**User Interface:**
- Consistent RaceDay branding.
- Bootstrap 5 responsive layouts.
- Role-based navigation.
- Form validation and user-friendly feedback.
- Event banner image display.
- Enrolment status indicators:
  - Green – Confirmed.
  - Amber – Pending.
  - Grey – Cancelled.

### How to Run the MVC Application Locally

**Prerequisites:**
- Visual Studio 2022.
- .NET 8 SDK.
- MySQL Server 8.0 or later.
- A configured RaceDay database.
- A working RaceDay API.

### Azure Blob Storage Setup

Azure Blob Storage is used to store uploaded event banner images.

The MVC application does not communicate directly with Azure. Instead, it sends image upload requests to the API, which handles the storage operation.

**Image upload flow:**

```text
       MVC Application
              |
              v
       Image Upload Request
              |
              v
          RaceDay.Api
              |
              v
      BlobStorageService
              |
              v
      Azure Blob Storage
              |
              v
       Image URL Returned
              |
              v
        MySQL Database
       (Image URL Stored)
              |
              v
       MVC Displays Image
```

**Prerequisites:**
- An Azure account.
- An Azure Storage Account.
- A configured Blob Storage container.

**Image Storage:**

Event banner images are stored in the Azure container using the following folder structure:

`event-banners/<guid>.<ext>`

The image URL is associated with the relevant event and stored in the database through the API.

### Docker Setup

Docker is used to containerise the RaceDay MVC application.

Containerisation allows the application to run in a consistent environment without requiring the MVC application to be launched directly through Visual Studio.

**Prerequisites:**
- Docker Desktop installed.
- Docker Desktop running with Linux containers enabled.
- .NET 8 SDK.
- A running RaceDay API.
- A configured MySQL database.

#### Building the Docker Image

Open a terminal in the solution root directory (the directory containing `RaceDay.sln`).
Run:
```bash
docker build -f RaceDay.Mvc/Dockerfile -t raceday-mvc:part3 .
```

This builds the MVC application into a Docker image named:

`raceday-mvc:part3`

#### Starting the API

The API must be accessible to the MVC container.

For the example below, the API is started on HTTP port 5000.
Open a terminal:
```bash
cd RaceDay.Api
dotnet run --urls http://0.0.0.0:5000
```

Keep the terminal running while using the Docker container.

#### Running the MVC Container

Open another terminal in the solution root directory.
Run:
```bash
docker run -d --name raceday-mvc -p 8080:8080 -e ApiSettings__BaseUrl=http://host.docker.internal:5000/ raceday-mvc:part3
```

This command:
- Starts the MVC container in detached mode.
- Maps host port 8080 to container port 8080.
- Configures the MVC application to communicate with the API running on the host.
- Uses the `raceday-mvc:part3` image.

These port and configuration values must match the actual Dockerfile and application settings.

#### Verifying the Docker Container

Check whether the container is running:
```bash
docker ps
```

View the application logs:
```bash
docker logs raceday-mvc
```

Open the browser and navigate to:
`http://localhost:8080`

The MVC application should load through the Docker container and communicate with the API.

**Stopping the container:**
```bash
docker stop raceday-mvc
```

**Removing the container:**
```bash
docker rm raceday-mvc
```

### CI/CD

GitHub Actions is used to automate the building and testing of the RaceDay solution.

The CI/CD workflow should validate the relevant projects and ensure that changes do not introduce build or test failures.

The workflow process includes:

1. Checking out the repository.
2. Installing the .NET 8 SDK.
3. Restoring NuGet dependencies.
4. Building the solution in Release configuration.
5. Running the available unit tests.

**Screenshot of Successful Part 3 CI/CD Build:**
<img width="832" height="267" alt="Part 3 CI/CD Build" src="https://github.com/user-attachments/assets/1e618f0b-7ec5-46bc-9df9-ae4f773254aa" />
<img width="627" height="345" alt="Part 3 CI/CD Build" src="https://github.com/user-attachments/assets/d6252bff-eaf8-4682-b208-a726a1f100a2" />


---

## Video Presentations

### Part 3 – MVC, Azure Blob Storage and Docker

**Part 3 Demonstration Video:**

[Watch the Part 3 Demonstration Video](https://youtu.be/IdaFF8D-zo0)

[![Watch Part 3 Demonstration Video](https://img.shields.io/badge/Watch-Video-red)](https://youtu.be/IdaFF8D-zo0)



### Part 2 – RESTful API

**Part 2 Demonstration Video:**

[Watch the Part 2 Demonstration Video](https://youtu.be/L67DBCfMdkM)

[![Watch Part 2 Demonstration Video](https://img.shields.io/badge/Watch-Video-red)](https://youtu.be/L67DBCfMdkM)



### Part 1 – Database and System Design

**Screenshot of Successful Part 1 CI/CD Build:**

<img width="1242" height="485" alt="Part 1 CI/CD Build" src="https://github.com/user-attachments/assets/1e3f6cd3-4644-41d7-9667-5c827456c3b5" />

**Part 1 Demonstration Video:**

[Watch the Part 1 Demonstration Video](https://youtu.be/4u8yH-S_8nI)

[![Watch Part 1 Demonstration Video](https://img.shields.io/badge/Watch-Video-red)](https://youtu.be/4u8yH-S_8nI)

---

## Conclusion

RaceDay demonstrates the development of a web-based event management system through three progressive stages.

**Part 1 – Database and System Design:** Established the database structure, entity relationships, SQL implementation and API endpoint planning.

**Part 2 – RESTful API:** Implemented the backend API, database communication, authentication, role-based access control, endpoint functionality and unit testing.

**Part 3 – MVC, Azure Blob Storage and Docker:** Extended the system with a user-facing MVC application, cloud-based image storage and containerisation.

Together, these components form the RaceDay Event Management System, providing a platform for event organisers to manage sporting events and participants to discover events, enrol and track their performance history.
