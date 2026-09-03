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