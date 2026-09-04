## RaceDay Management System
Project Background
South Africa has an incredibly vibrant sports culture, hosting legendary events like the Comrades Marathon, Cape Town Cycle Tour, and Two Oceans Marathon. However, many local community races and running/cycling clubs still rely on paper sheets, spreadsheets, and fragmented channels to coordinate entries and record results. This leaves event organisers overwhelmed and participants underserved.

RaceDay is a full-stack, cloud-aware, API-driven platform built progressively using ASP.NET Core Web API, MVC, Entity Framework Core, and SQL Server. It is designed specifically to bring community events, categories, registrations, live race day weather/routes, and results tracking together under a unified, role-based platform.

Supported Roles
The RaceDay system strictly enforces two distinct user roles throughout the API and interface:

Event Organiser:

Create, update, and delete events (capturing name, description, date, location, distance, type, banner image).
Configure distance and age categories per event (e.g., Senior, Junior, 10km, 21km).
View all registered enrolments for their events.
Capture, update, and publish finish times and positioning results after the race completes.
Participant:

Create a secure participant profile.
Browse upcoming sporting events with convenient category filtering.
Enrol in events by selecting an appropriate category (e.g., Run, Walk, Cycle).
View their custom registrations and personal race results history.
Directory Structure (PoE Standard)
RaceDay/
├── .github/
│   └── workflows/
│       └── validate-docs.yml     <-- Automated CI/CD Workflow validating repository structure
├── docs/
│   ├── raceday-erd.png           <-- Section A: Entity Relationship Diagram
│   ├── raceday-api-plan.md       <-- Section B: API Endpoint Plan (Markdown)
│   └── raceday-db-script.sql     <-- Section C: Complete SQL Schema & Seeding Script
└── README.md                     <-- General Documentation and Walkthrough Details
Setup & Running the Database Script
Open SQL Server Management Studio (SSMS) and connect to your local SQL Server instance.
Open a new query window.
Open or copy the script docs/raceday-db-script.sql.
Press F5 or click Execute to run the script. This will:
Create the database RaceDayDB.
Setup the tables with strict keys, relational constraints, unique conditions, and checks.
Seed the database with 2 Organisers, 2 Participants, 3 events, and initial confirmed enrolments and results.
Review the verification count printouts in the results pane to confirm successful creation.
Video Presentation & Demo Walkthrough
An unlisted YouTube video showing the live execution and design decisions can be found here:

YouTube Walkthrough Link: [INSERT YOUR YOUTUBE LINK HERE]
The presentation covers:

System overview and South African community road racing background.
Structural decisions made in Section A (ERD cardinality and normalization).
Detailed breakdown of Section B's Endpoint Plan (routing, access security, and expected responses).
Live execution of raceday-db-script.sql on a clean SQL instance in SSMS, showing successful data counts.
