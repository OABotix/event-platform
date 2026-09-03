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
    RoleID INT AUTO_INCREMENT PRIMARY KEY,
    RoleName VARCHAR(50) NOT NULL UNIQUE
);

-- ============================================
-- 2. EVENT TYPE TABLE
-- ============================================
CREATE TABLE EventType (
    EventTypeID INT AUTO_INCREMENT PRIMARY KEY,
    TypeName VARCHAR(50) NOT NULL UNIQUE
);
