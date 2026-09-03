-- ============================================
-- RaceDay Database Script
-- PROG6212 Part 1
-- ============================================

-- Create database
CREATE DATABASE RaceDayDB;

USE RaceDayDB;

-- ============================================
-- 1. ROLE TABLE
-- ============================================
CREATE TABLE Role (
    RoleID INT NOT NULL AUTO_INCREMENT,
    PRIMARY KEY (RoleID),
    RoleName VARCHAR(50) NOT NULL UNIQUE
);

-- ============================================
-- 2. EVENT TYPE TABLE
-- ============================================
CREATE TABLE EventType (
    EventTypeID INT NOT NULL AUTO_INCREMENT,
    PRIMARY KEY (EventTypeID),
    TypeName VARCHAR(50) NOT NULL UNIQUE
);

-- ============================================
-- 3. USER TABLE
-- ============================================
CREATE TABLE Users (
    UserID INT NOT NULL AUTO_INCREMENT,
    PRIMARY KEY (UserID),
    RoleID INT NOT NULL,
    CONSTRAINT FK_Users_Role FOREIGN KEY (RoleID)
        REFERENCES Role(RoleID),
    FullName VARCHAR(100) NOT NULL,
    Email VARCHAR(255) NOT NULL UNIQUE,
    PasswordHash VARCHAR(500) NOT NULL,
    PhoneNumber VARCHAR(20),
    ProfilePicture VARCHAR(500),
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- ============================================
-- 4. EVENT TABLE
-- ============================================
CREATE TABLE Event (
    EventID INT NOT NULL AUTO_INCREMENT,
    PRIMARY KEY (EventID),
    OrganiserID INT NOT NULL,
    CONSTRAINT FK_Event_Organiser FOREIGN KEY (OrganiserID)
        REFERENCES Users(UserID),
    EventTypeID INT NOT NULL,
    CONSTRAINT FK_Event_EventType FOREIGN KEY (EventTypeID)
        REFERENCES EventType(EventTypeID),
    EventName VARCHAR(200) NOT NULL,
    Description TEXT NOT NULL,
    EventDate DATETIME NOT NULL,
    Location VARCHAR(200) NOT NULL,
    Distance DECIMAL(10,2) NOT NULL,
    EventBanner VARCHAR(500),
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- ============================================
-- 5. CATEGORY TABLE
-- ============================================
CREATE TABLE Category (
    CategoryID INT NOT NULL AUTO_INCREMENT,
    PRIMARY KEY (CategoryID),
    EventID INT NOT NULL,
    CONSTRAINT FK_Category_Event FOREIGN KEY (EventID)
        REFERENCES Event(EventID),
    CategoryName VARCHAR(100) NOT NULL,
    Description VARCHAR(500),
    EntryFee DECIMAL(10,2) DEFAULT 0
);

-- ============================================
-- 6. ENROLMENT TABLE
-- ============================================
CREATE TABLE Enrolment (
    EnrolmentID INT NOT NULL AUTO_INCREMENT,
    PRIMARY KEY (EnrolmentID),
    ParticipantID INT NOT NULL,
    CONSTRAINT FK_Enrolment_Participant FOREIGN KEY (ParticipantID)
        REFERENCES Users(UserID),
    EventID INT NOT NULL,
    CONSTRAINT FK_Enrolment_Event FOREIGN KEY (EventID)
        REFERENCES Event(EventID),
    CategoryID INT NOT NULL,
    CONSTRAINT FK_Enrolment_Category FOREIGN KEY (CategoryID)
        REFERENCES Category(CategoryID),
    EnrolmentDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    Status VARCHAR(20) DEFAULT 'Pending',
    CONSTRAINT UQ_Enrolment UNIQUE (ParticipantID, EventID)
);

-- ============================================
-- 7. RESULT TABLE
-- ============================================
CREATE TABLE Result (
    ResultID INT NOT NULL AUTO_INCREMENT,
    PRIMARY KEY (ResultID),
    EnrolmentID INT NOT NULL,
    CONSTRAINT FK_Result_Enrolment FOREIGN KEY (EnrolmentID)
        REFERENCES Enrolment(EnrolmentID),
    FinishTime TIME NOT NULL,
    FinishPosition INT NOT NULL,
    PublishedAt DATETIME,
    CONSTRAINT UQ_Result_Enrolment UNIQUE (EnrolmentID)
);

-- ============================================
-- SEED DATA
-- ============================================

-- ============================================
-- ROLES
-- ============================================

INSERT INTO Role (RoleName) VALUES
('Organiser'),
('Participant');

-- ============================================
-- EVENT TYPES
-- ============================================

INSERT INTO EventType (TypeName) VALUES
('Run'),
('Walk'),
('Cycle'),
('Swim'),
('Triathlon');

-- ============================================
-- USERS
-- ============================================

-- Organisers
INSERT INTO Users
(RoleID, FullName, Email, PasswordHash, PhoneNumber)
VALUES
(1, 'Thabo Nkosi', 'thabo.nkosi@theeventplatform.co.za', 'hash_thabo_123', '0821112222'),
(1, 'Sarah Peters', 'sarah.peters@theeventplatform.co.za', 'hash_sarah_123', '0833334444'),
(1, 'Michael Daniels', 'michael.daniels@theeventplatform.co.za', 'hash_michael_123', '0842223333'),
(1, 'Nomsa Dlamini', 'nomsa.dlamini@theeventplatform.co.za', 'hash_nomsa_123', '0764445555');

-- Participants (UserIDs 5-14)
INSERT INTO Users
(RoleID, FullName, Email, PasswordHash, PhoneNumber)
VALUES
(2, 'Lerato Molefe', 'lerato.molefe@gmail.com', 'hash_lerato_123', '0825556666'),
(2, 'James van Wyk', 'james.vanwyk@gmail.com', 'hash_james_123', '0737778888'),
(2, 'Anele Jacobs', 'anele.jacobs@gmail.com', 'hash_anele_123', '0823456789'),
(2, 'Sipho Mthembu', 'sipho.mthembu@gmail.com', 'hash_sipho_123', '0834567890'),
(2, 'Megan Williams', 'megan.williams@gmail.com', 'hash_megan_123', '0725678901'),
(2, 'Daniel Naidoo', 'daniel.naidoo@gmail.com', 'hash_daniel_123', '0816789012'),
(2, 'Zanele Khumalo', 'zanele.khumalo@gmail.com', 'hash_zanele_123', '0797890123'),
(2, 'Ryan Botha', 'ryan.botha@gmail.com', 'hash_ryan_123', '0718901234'),
(2, 'Kayla Petersen', 'kayla.petersen@gmail.com', 'hash_kayla_123', '0829012345'),
(2, 'Andile Mokoena', 'andile.mokoena@gmail.com', 'hash_andile_123', '0730123456');


-- ============================================
-- EVENTS
-- ============================================

-- Running Events (EventIDs 1-6)
INSERT INTO Event
(OrganiserID, EventTypeID, EventName, Description, EventDate, Location, Distance)
VALUES
(1, 1, 'Comrades Marathon', 'The iconic ultra-marathon between Durban and Pietermaritzburg, attracting thousands of runners from South Africa and around the world.', '2026-06-14 05:30:00', 'Durban to Pietermaritzburg, KwaZulu-Natal', 89.00 ),
(1, 1, 'Totalsports Two Oceans Marathon', 'A major Cape Town road running event featuring the famous 56km Ultra Marathon and Half Marathon.', '2026-04-19 06:00:00', 'Cape Town, Western Cape', 56.00 ),
(2, 1, 'Two Oceans Half Marathon', 'A popular 21.1km road race through the southern suburbs of Cape Town.', '2026-04-19 07:00:00', 'Cape Town, Western Cape', 21.10 ),
(2, 1, 'Gun Run', 'A popular Cape Town road running event offering half marathon and shorter distance options.', '2026-10-04 06:30:00', 'Cape Town, Western Cape', 21.10 ),
(3, 1, 'Soweto Marathon', 'A major Johannesburg road running event taking participants through the historic streets and landmarks of Soweto.', '2026-11-01 06:00:00', 'Soweto, Johannesburg', 42.20 ),
(3, 1, 'Absa Run Your City Cape Town', 'A mass participation road running event through the streets of Cape Town.', '2026-09-20 08:00:00', 'Cape Town, Western Cape', 10.00 );


-- Walking Events (EventIDs 7-9)
INSERT INTO Event
(OrganiserID, EventTypeID, EventName, Description, EventDate, Location, Distance)
VALUES
(4, 2, 'Cape Town Big Walk', 'A community-focused walking event encouraging participants of all fitness levels to enjoy an active day outdoors.', '2026-09-27 07:00:00', 'Cape Town, Western Cape', 10.00 ),
(4, 2, 'Durban Beach Walk', 'A scenic community walk along the Durban beachfront with shorter options for families and casual participants.', '2026-10-11 07:30:00', 'Durban, KwaZulu-Natal', 10.00 ),
(1, 2, 'Johannesburg Community Walk', 'A family-friendly community walking event supporting active and healthy lifestyles in Johannesburg.', '2026-10-24 08:00:00', 'Johannesburg, Gauteng', 5.00 );


-- Cycling Events (EventIDs 10-13)
INSERT INTO Event
(OrganiserID, EventTypeID, EventName, Description, EventDate, Location, Distance)
VALUES
(2, 3, 'Cape Town Cycle Tour', 'South Africa''s iconic mass-participation cycling event around the Cape Peninsula.', '2026-03-08 06:00:00', 'Cape Town, Western Cape', 109.00 ),
(3, 3, 'aQuellé Tour Durban', 'A major KwaZulu-Natal road cycling event offering several distances for experienced and recreational cyclists.', '2026-04-19 06:30:00', 'Durban, KwaZulu-Natal', 100.00 ),
(1, 3, 'Ride Joburg', 'A major Johannesburg mass-participation road cycling event featuring a challenging urban route.', '2026-11-22 06:00:00', 'Johannesburg, Gauteng', 97.00 ),
(4, 3, 'Cape Town Cycle Tour 42km', 'A shorter Cape Town cycling route designed for riders looking for a less demanding mass-participation ride.', '2026-03-08 07:00:00', 'Cape Town, Western Cape', 42.00 );


-- Swimming Events (EventIDs 14-15)
INSERT INTO Event
(OrganiserID, EventTypeID, EventName, Description, EventDate, Location, Distance)
VALUES
(4, 4, 'aQuellé Midmar Mile', 'One of South Africa''s best-known open-water swimming events, attracting swimmers of different ages and abilities.', '2026-02-14 08:00:00', 'Midmar Dam, KwaZulu-Natal', 1.60 ),
(3, 4, 'Cape Town Open Water Swim', 'An open-water swimming event providing competitive and recreational distances along the Cape Town coastline.', '2026-12-05 07:00:00', 'Cape Town, Western Cape', 5.00 );


-- Triathlon Events (EventIDs 16-17)
INSERT INTO Event
(OrganiserID, EventTypeID, EventName, Description, EventDate, Location, Distance)
VALUES
(3, 5, 'IRONMAN South Africa', 'A full-distance triathlon in Nelson Mandela Bay combining a 3.8km swim, 180km cycle and 42.2km run.', '2026-04-19 06:30:00', 'Gqeberha, Eastern Cape', 226.00 ),
(4, 5, 'IRONMAN 70.3 South Africa', 'A half-distance triathlon combining swimming, cycling and running for athletes seeking a shorter endurance challenge.', '2026-11-01 06:30:00', 'Gqeberha, Eastern Cape', 113.00 );

