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

-- ============================================
-- CATEGORIES
-- ============================================

-- Comrades Marathon (EventID 1)
INSERT INTO Category
(EventID, CategoryName, Description, EntryFee)
VALUES
(1, 'Ultra Marathon', 'Approximately 89km ultra-marathon route between Durban and Pietermaritzburg.', 750.00);

-- Two Oceans Marathon (EventID 2)
INSERT INTO Category
(EventID, CategoryName, Description, EntryFee)
VALUES
(2, '56km Ultra Marathon', 'The iconic Two Oceans Ultra Marathon route.', 700.00),
(2, '21.1km Half Marathon', 'Half marathon route through the Cape Town southern suburbs.', 400.00);

-- Two Oceans Half Marathon (EventID 3)
INSERT INTO Category
(EventID, CategoryName, Description, EntryFee)
VALUES
(3, '21.1km Half Marathon', 'Standard half marathon category.', 400.00),
(3, '21.1km Junior', 'Junior half marathon category subject to event eligibility requirements.', 300.00);

-- Gun Run (EventID 4)
INSERT INTO Category
(EventID, CategoryName, Description, EntryFee)
VALUES
(4, '21.1km Half Marathon', 'Half marathon road race through Cape Town.', 300.00),
(4, '10km Road Race', '10km road running event.', 220.00),
(4, '5km Fun Run', 'Short community-focused fun run.', 120.00);

-- Soweto Marathon (EventID 5)
INSERT INTO Category
(EventID, CategoryName, Description, EntryFee)
VALUES
(5, '42.2km Marathon', 'Full marathon distance through Soweto.', 450.00),
(5, '21.1km Half Marathon', 'Half marathon distance.', 350.00),
(5, '10km Road Race', '10km road running event.', 200.00);

-- Absa Run Your City (EventID 6)
INSERT INTO Category
(EventID, CategoryName, Description, EntryFee)
VALUES
(6, '10km Road Race', '10km mass participation road race.', 180.00),
(6, '10km Corporate Entry', '10km corporate participation category.', 200.00);

-- Cape Town Big Walk (EventID 7)
INSERT INTO Category
(EventID, CategoryName, Description, EntryFee)
VALUES
(7, '10km Walk', '10km community walking route.', 150.00),
(7, '5km Walk', 'Shorter 5km walking route.', 100.00);

-- Durban Beach Walk (EventID 8)
INSERT INTO Category
(EventID, CategoryName, Description, EntryFee)
VALUES
(8, '10km Beach Walk', '10km beachfront walking route.', 150.00),
(8, '5km Family Walk', 'Family-friendly 5km route.', 100.00);

-- Johannesburg Community Walk (EventID 9)
INSERT INTO Category
(EventID, CategoryName, Description, EntryFee)
VALUES
(9, '5km Community Walk', '5km community walking route.', 80.00),
(9, '5km Family Entry', 'Family-oriented 5km walking category.', 60.00);

-- Cape Town Cycle Tour (EventID 10)
INSERT INTO Category
(EventID, CategoryName, Description, EntryFee)
VALUES
(10, '109km Main Route', 'Full Cape Peninsula cycling route.', 940.00),
(10, 'Tandem Entry', 'Tandem cycling category for two riders.', 1200.00);

-- aQuellé Tour Durban (EventID 11)
INSERT INTO Category
(EventID, CategoryName, Description, EntryFee)
VALUES
(11, '100km Road Race', 'Long-distance road cycling route.', 1450.00),
(11, '65km Road Ride', 'Intermediate road cycling route.', 900.00),
(11, '28km Fun Ride', 'Short recreational cycling route.', 300.00);

-- Ride Joburg (EventID 12)
INSERT INTO Category
(EventID, CategoryName, Description, EntryFee)
VALUES
(12, '97km Road Race', 'Main mass-participation road cycling route.', 950.00),
(12, '35km Short Ride', 'Shorter cycling route for recreational riders.', 450.00);

-- Cape Town Cycle Tour 42km (EventID 13)
INSERT INTO Category
(EventID, CategoryName, Description, EntryFee)
VALUES
(13, '42km Route', 'Shorter Cape Town Cycle Tour route.', 500.00);

-- Midmar Mile (EventID 14)
INSERT INTO Category
(EventID, CategoryName, Description, EntryFee)
VALUES
(14, '1.6km Open Water Swim', 'Standard Midmar Mile open-water swimming distance.', 250.00),
(14, '1.6km Junior Swim', 'Junior open-water swimming category.', 200.00);

-- Cape Town Open Water Swim (EventID 15)
INSERT INTO Category
(EventID, CategoryName, Description, EntryFee)
VALUES
(15, '5km Open Water Swim', '5km open-water swimming event.', 350.00),
(15, '3km Open Water Swim', '3km open-water swimming event.', 280.00),
(15, '1.5km Open Water Swim', 'Short recreational open-water swim.', 200.00);

-- IRONMAN South Africa (EventID 16)
INSERT INTO Category
(EventID, CategoryName, Description, EntryFee)
VALUES
(16, 'Full IRONMAN', '3.8km swim, 180km bike and 42.2km run.', 9410.00);

-- IRONMAN 70.3 South Africa (EventID 17)
INSERT INTO Category
(EventID, CategoryName, Description, EntryFee)
VALUES
(17, 'IRONMAN 70.3', '1.9km swim, 90km bike and 21.1km run.', 6500.00);

-- ============================================
-- ENROLMENTS
-- ============================================

-- Lerato Molefe (UserID 5)
INSERT INTO Enrolment
(ParticipantID, EventID, CategoryID, EnrolmentDate, Status)
VALUES
(5, 1, 1, '2025-12-10 09:15:00', 'Confirmed'),
(5, 2, 2, '2026-01-15 10:30:00', 'Confirmed'),
(5, 10, 13, '2025-11-20 14:00:00', 'Confirmed'),
(5, 14, 24, '2025-12-12 11:20:00', 'Confirmed'),
(5, 6, 11, '2026-07-15 09:00:00', 'Confirmed');

-- James van Wyk (UserID 6)
INSERT INTO Enrolment
(ParticipantID, EventID, CategoryID, EnrolmentDate, Status)
VALUES
(6, 1, 1, '2025-12-05 08:45:00', 'Confirmed'),
(6, 3, 4, '2026-01-20 16:10:00', 'Confirmed'),
(6, 12, 21, '2026-01-15 09:30:00', 'Confirmed'),
(6, 16, 25, '2025-10-10 12:00:00', 'Confirmed'),
(6, 17, 26, '2026-05-15 10:00:00', 'Confirmed');

-- Anele Jacobs (UserID 7)
INSERT INTO Enrolment
(ParticipantID, EventID, CategoryID, EnrolmentDate, Status)
VALUES
(7, 3, 3, '2026-01-18 13:25:00', 'Confirmed'),
(7, 4, 6, '2026-07-01 15:40:00', 'Confirmed'),
(7, 7, 14, '2026-06-15 10:00:00', 'Confirmed'),
(7, 15, 26, '2026-08-20 09:15:00', 'Confirmed'),
(7, 11, 17, '2026-02-10 11:00:00', 'Confirmed');

-- Sipho Mthembu (UserID 8)
INSERT INTO Enrolment
(ParticipantID, EventID, CategoryID, EnrolmentDate, Status)
VALUES
(8, 5, 8, '2026-06-12 11:00:00', 'Confirmed'),
(8, 11, 17, '2026-02-25 14:20:00', 'Confirmed'),
(8, 12, 20, '2026-07-10 12:45:00', 'Confirmed'),
(8, 9, 18, '2026-09-01 10:00:00', 'Pending');

-- Megan Williams (UserID 9)
INSERT INTO Enrolment
(ParticipantID, EventID, CategoryID, EnrolmentDate, Status)
VALUES
(9, 2, 3, '2026-01-22 09:00:00', 'Confirmed'),
(9, 6, 11, '2026-07-15 10:15:00', 'Confirmed'),
(9, 14, 24, '2025-12-18 08:30:00', 'Confirmed'),
(9, 15, 28, '2026-09-15 11:00:00', 'Confirmed');

-- Daniel Naidoo (UserID 10)
INSERT INTO Enrolment
(ParticipantID, EventID, CategoryID, EnrolmentDate, Status)
VALUES
(10, 10, 12, '2025-11-25 15:00:00', 'Confirmed'),
(10, 11, 16, '2026-03-01 11:30:00', 'Confirmed'),
(10, 16, 25, '2025-10-08 09:45:00', 'Confirmed'),
(10, 13, 23, '2026-01-15 10:00:00', 'Confirmed');

-- Zanele Khumalo (UserID 11)
INSERT INTO Enrolment
(ParticipantID, EventID, CategoryID, EnrolmentDate, Status)
VALUES
(11, 5, 9, '2026-05-20 13:00:00', 'Confirmed'),
(11, 8, 16, '2026-08-10 14:15:00', 'Confirmed'),
(11, 9, 18, '2026-09-01 10:00:00', 'Pending'),
(11, 14, 25, '2026-01-10 09:00:00', 'Confirmed');

-- Ryan Botha (UserID 12)
INSERT INTO Enrolment
(ParticipantID, EventID, CategoryID, EnrolmentDate, Status)
VALUES
(12, 2, 2, '2026-01-10 12:30:00', 'Confirmed'),
(12, 10, 12, '2025-11-30 09:10:00', 'Confirmed'),
(12, 17, 26, '2026-05-20 16:00:00', 'Confirmed'),
(12, 4, 5, '2026-07-01 08:00:00', 'Confirmed');

-- Kayla Petersen (UserID 13)
INSERT INTO Enrolment
(ParticipantID, EventID, CategoryID, EnrolmentDate, Status)
VALUES
(13, 4, 7, '2026-07-20 08:45:00', 'Confirmed'),
(13, 7, 14, '2026-06-05 11:15:00', 'Confirmed'),
(13, 15, 28, '2026-08-25 10:30:00', 'Confirmed'),
(13, 8, 16, '2026-08-01 09:00:00', 'Confirmed');

-- Andile Mokoena (UserID 14)
INSERT INTO Enrolment
(ParticipantID, EventID, CategoryID, EnrolmentDate, Status)
VALUES
(14, 5, 7, '2026-04-10 15:30:00', 'Confirmed'),
(14, 12, 20, '2026-07-05 09:00:00', 'Confirmed'),
(14, 17, 26, '2026-05-12 14:45:00', 'Confirmed'),
(14, 3, 4, '2026-01-25 11:00:00', 'Confirmed');

-- ============================================
-- RESULTS
-- ============================================

-- Results referencing Enrolment IDs (1-34)
INSERT INTO Result
(EnrolmentID, FinishTime, FinishPosition, PublishedAt)
VALUES
-- Comrades Marathon
(1, '08:42:17', 1842, '2026-06-14 18:00:00'),
(6, '07:58:31', 967,  '2026-06-14 18:00:00'),

-- Two Oceans Ultra Marathon (EventID 2)
(2, '05:36:22', 1284, '2026-04-19 14:30:00'),
(19, '04:48:47', 932,  '2026-04-19 14:30:00'),
(25, '05:52:10', 1542, '2026-04-19 14:30:00'),

-- Two Oceans Half Marathon (EventID 3)
(7, '01:54:38', 742, '2026-04-19 11:30:00'),
(11, '01:48:25', 521, '2026-04-19 11:30:00'),
(34, '01:38:12', 312, '2026-04-19 11:30:00'),

-- Cape Town Cycle Tour 109km (EventID 10)
(3, '04:21:45', 5842, '2026-03-08 16:30:00'),
(23, '03:57:18', 4210, '2026-03-08 16:30:00'),
(29, '04:12:06', 5137, '2026-03-08 16:30:00'),

-- Midmar Mile (EventID 14)
(4, '00:31:42', 384, '2026-02-14 11:00:00'),
(22, '00:34:18', 612, '2026-02-14 11:00:00'),

-- Tour Durban 100km (EventID 11)
(12, '03:42:15', 821, '2026-04-19 13:00:00'),
(26, '02:18:44', 412, '2026-04-19 12:30:00'),

-- IRONMAN South Africa (EventID 16)
(9, '11:48:32', 614, '2026-04-20 00:30:00'),
(27, '10:56:21', 388, '2026-04-20 00:30:00'),

-- Soweto Marathon (EventID 5)
(13, '04:07:26', 512, '2026-11-01 14:00:00'),

-- Gun Run Half Marathon (EventID 4)
(16, '01:52:08', 318, '2026-10-04 11:30:00'),

-- Cape Town Big Walk (EventID 7)
(17, '01:32:45', 102, '2026-09-27 10:00:00'),
(30, '01:28:33', 87, '2026-09-27 10:00:00'),

-- Run Your City 10km (EventID 6)
(21, '00:52:17', 342, '2026-09-20 10:30:00'),
(28, '00:48:53', 215, '2026-09-20 10:30:00');

-- ============================================
-- VERIFY DATA
-- ============================================

SELECT * FROM Role;

SELECT * FROM EventType;

SELECT * FROM Users;

SELECT * FROM Event;

SELECT * FROM Category;

SELECT * FROM Enrolment;

SELECT * FROM Result;
