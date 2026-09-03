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