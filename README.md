# Event Platform - a RaceDay Event Management System

## Project Description
RaceDay is a web-based event management system for the South African road running, walking, swimming, triatholon and cycling community. The platform allows Event Organisers to create and manage events while Participants can browse, enrol, and track their personal performance history.

## User Roles

### Organiser
- Create, edit, and delete events
- Manage event categories (age/distance groups)
- View all event enrolments
- Capture participant results (finish times and positions)

### Participant
- Browse upcoming events
- Enrol in events by selecting a category
- View personal enrolment history
- Track personal race results
- Manage their profile

## Part 1 Deliverables

### Entity Relationship Diagram (ERD)
- File: `docs/RaceDay_ERD.pdf`
- Contains 7 entities: Role, Users, EventType, Event, Category, Enrolment, Result
- Shows primary keys, foreign keys, and cardinality

### API Endpoint Plan
- File: `docs/RaceDay_API_Endpoint_Plan.pdf`
- Covers: Authentication, User Profile, Events, Categories, Enrolments, Results
- Includes all 6 required columns per endpoint

### SQL Database Script
- File: `docs/RaceDay_Database.sql`
- Creates RaceDayDB database with 7 tables
- Includes seed data: 2 Organisers, 2 Participants, 3 Events, categories, enrolments, results

## Database Setup

1. Open SQL Server Management Studio (SSMS)
2. Connect to your SQL Server instance
3. Open `docs/RaceDay_Database.sql`
4. Execute the script (F5)
5. Verify RaceDayDB database with 7 tables is created
6. Check seed data with the verification queries at the bottom

## CI/CD

The GitHub Actions workflow validation **Screenshot of Successful CI Run:**
<img width="1242" height="485" alt="CI/CD Build" src="https://github.com/user-attachments/assets/1e3f6cd3-4644-41d7-9667-5c827456c3b5" />

## Video Presentation

[Watch the demonstration video](https://youtu.be/xvQ2Ah236eE)

[![Watch Demonstration Video](https://img.shields.io/badge/Watch-Video-red)](https://youtu.be/xvQ2Ah236eE)

## Repository Structure
```
RaceDay/
│
├── README.md
├── docs/
│   ├── RaceDay_ERD.pdf
│   ├── RaceDay_API_Endpoint_Plan.pdf
│   └── RaceDay_Database.sql
│
└── .github/
    └── workflows/
        └── part1-ci.yml
```
        
