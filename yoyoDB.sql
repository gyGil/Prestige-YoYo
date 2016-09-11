/* ====================================================================================
* PROGRAMMERS:	Marcus Rankin & Geun Young Gil
* REFERENCES:	Author/Programmer: Norber Mika - SampleQueueReader
* FILENAME:		yoyoDB.sql
* PROJECT:		PROG3070 - Milestone 01 - Final Project - Reject Analysis for Prestige YoYo
* DATE:			March 14th, 2016
* DESCRIPTION:	This script file creates the database and all the tables required
*				for the Reject Analysis system.
=======================================================================================*/

/******************************************************************************************************
*								YOYO DATABASE SETUP
*******************************************************************************************************/
CREATE DATABASE yoyo;
GO

USE yoyo;
GO
/******************************************************************************************************
*								YOYO DATABASE TABLES CREATION
*******************************************************************************************************/



/************************** MAIN MANUFACTURING DATA TABLE *********************************************/
CREATE TABLE manufacturingData 
(yoyoID INT IDENTITY(1,1) PRIMARY KEY, workArea VARCHAR(50), sinNumber VARCHAR(50),
 line VARCHAR(10), station VARCHAR(50), resultState VARCHAR(30), actionTimeStamp DATETIME, reject INT DEFAULT 0, rework INT DEFAULT 0);
 GO

/* resultState in manufacturingData table needs foreign key to resultState in stateStatus table */

/************************** CREATE REJECT/REWORK TABLE *********************************************/
CREATE TABLE rejection
(rejectID INT IDENTITY(1,1) PRIMARY KEY, inspectionLocation INT, rejectType VARCHAR(10), reason VARCHAR(30));
GO

/************************** PRODUCT SKU/DESCRIPTION/COLOUR TABLE *********************************************/
CREATE TABLE products
(sku VARCHAR(6) NOT NULL PRIMARY KEY, productDescription VARCHAR(20), colour VARCHAR(10));
GO

/************************** STATE/DESCRIPTION TABLE *********************************************/
CREATE TABLE stateStatus
(stateID INT IDENTITY(1,1) PRIMARY KEY, resultState VARCHAR(20), stateDescription VARCHAR(50));
GO

/************************** USERS TABLE *********************************************/
CREATE TABLE users
(userID VARCHAR(20) NOT NULL PRIMARY KEY, userPassword VARCHAR(20), isADMIN INT DEFAULT 0);
GO

/************************** SCHEDULE TABLE *********************************************/
CREATE TABLE schedules
(scheduleID INT IDENTITY(1,1) PRIMARY KEY, sku VARCHAR(6) FOREIGN KEY REFERENCES products(sku), startDate DATETIME, endDate DATETIME);
GO

/************************** BUILD USERS TABLE *********************************************/
INSERT INTO users (userID, userPassword) VALUES ('user', 'user');
INSERT INTO users (userID, userPassword, isADMIN) VALUES ('admin', 'admin', 1);

/************************** BUILD REJECT/REWORK TABLE *********************************************/
INSERT INTO rejection (inspectionLocation, rejectType, reason)
 VALUES(1, 'Reject', 'Inconsistent thickness');
INSERT INTO rejection (inspectionLocation, rejectType, reason)
 VALUES(1, 'Reject', 'Pitting');
INSERT INTO rejection (inspectionLocation, rejectType, reason)
 VALUES(1, 'Reject', 'Warping');
INSERT INTO rejection (inspectionLocation, rejectType, reason)
 VALUES(2, 'Reject', 'Primer Defect');
INSERT INTO rejection (inspectionLocation, rejectType, reason)
 VALUES(2, 'Rework', 'Drip mark');
INSERT INTO rejection (inspectionLocation, rejectType, reason)
 VALUES(2, 'Rework', 'Final Coat flaw');
INSERT INTO rejection (inspectionLocation, rejectType, reason)
 VALUES(3, 'Reject', 'Broken shell');
INSERT INTO rejection (inspectionLocation, rejectType, reason)
 VALUES(3, 'Rework', 'Broken Axle');
INSERT INTO rejection (inspectionLocation, rejectType, reason)
 VALUES(3, 'Rework', 'Tangled String');
INSERT INTO rejection (inspectionLocation, rejectType, reason)
 VALUES(3, 'Rework', 'Final Coat flaw');
GO



/************************** BUILD SKU/DESCRIPTION/COLOUR TABLE *********************************************/
INSERT INTO products (sku, productDescription, colour)
 VALUES('Y001-1', 'Prestige Classic', 'Red');
INSERT INTO products (sku, productDescription, colour)
 VALUES('Y001-2', 'Prestige Classic', 'Blue');
INSERT INTO products (sku, productDescription, colour)
 VALUES('Y001-3', 'Prestige Classic', 'Green');
INSERT INTO products (sku, productDescription, colour)
 VALUES('Y002-0', 'Clear Plastic', 'Clear');
INSERT INTO products (sku, productDescription, colour)
 VALUES('Y005-1', 'Whistler', 'Red');
INSERT INTO products (sku, productDescription, colour)
 VALUES('Y005-2', 'Whistler', 'Blue');
INSERT INTO products (sku, productDescription, colour)
 VALUES('Y005-3', 'Whistler', 'Green');
GO



/************************** BUILD STATE/DESCRIPTION TABLE *********************************************/
INSERT INTO stateStatus (resultState, stateDescription)
 VALUES('MOLD', 'In the Mold process');
INSERT INTO stateStatus (resultState, stateDescription)
 VALUES('QUEUE_INSPECTION_1', 'On the conveyor to inspection station 1');
INSERT INTO stateStatus (resultState, stateDescription)
 VALUES('INSPECTION_1', 'At inspection station 1');
INSERT INTO stateStatus (resultState, stateDescription)
 VALUES('INSPECTION_1_SCRAP', 'In scrap (end of process for that particular yoyo)');
INSERT INTO stateStatus (resultState, stateDescription)
 VALUES('QUEUE_PAINT', 'On the conveyor to the paint process');
INSERT INTO stateStatus (resultState, stateDescription)
 VALUES('PAINT', 'In the paint process');
INSERT INTO stateStatus (resultState, stateDescription)
 VALUES('QUEUE_INSPECTION_2', 'On the conveyor to inspection station 2');
INSERT INTO stateStatus (resultState, stateDescription)
 VALUES('INSPECTION_2', 'At inspection station 2');
INSERT INTO stateStatus (resultState, stateDescription)
 VALUES('INSPECTION_2_REWORK', 'Being reworked and sent back to paint');
INSERT INTO stateStatus (resultState, stateDescription)
 VALUES('INSPECTION_2_SCRAP', 'In scrap (end of process for that particular yoyo');
INSERT INTO stateStatus (resultState, stateDescription)
 VALUES('QUEUE_ASSEMBLY', 'On the conveyor to the assembly process');
INSERT INTO stateStatus (resultState, stateDescription)
 VALUES('ASSEMBLY', 'In the assembly process');
INSERT INTO stateStatus (resultState, stateDescription)
 VALUES('QUEUE_INSPECTION_3', 'On the conveyor to inspection station 3');
INSERT INTO stateStatus (resultState, stateDescription)
 VALUES('INSPECTION_3', 'At inspection station 3');
INSERT INTO stateStatus (resultState, stateDescription)
 VALUES('INSPECTION_3_REWORK', 'Being reworked and sent back to assembly');
INSERT INTO stateStatus (resultState, stateDescription)
 VALUES('INSPECTION_3_SCRAP', 'In scrap (end of process for that particular yoyo');
INSERT INTO stateStatus (resultState, stateDescription)
 VALUES('PACKAGE', 'In package (end of process for a good yoyo)');
GO

