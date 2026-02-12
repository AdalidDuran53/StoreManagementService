-- Feature: SMS-3 
-- Author: Adalid
-- Purpose: Create a new database
-- UPDATES: 
-- * SMS-9 updated schema
CREATE DATABASE StoreManagement
GO

USE StoreManagement;
--****************************** TABLE FOR LOGIN ******************************
CREATE TABLE StoreManagement.dbo.Clients (
    ClientID UNIQUEIDENTIFIER PRIMARY KEY, -- UserID, must be UNIQUEIDENTIFIER
    emailAddress NVARCHAR(50) UNIQUE,         -- User name, must be UNIQUE
    ClientName NVARCHAR(50)  NOT NULL,         -- User name, must be UNIQUE
	ClientLastName NVARCHAR(100) NOT NULL,		-- required
    ClientAddress NVARCHAR(MAX) NOT NULL,		-- required
    PasswordHash NVARCHAR(MAX) NOT NULL,          -- PasswordHash, required
    PasswordSalst NVARCHAR(MAX) NOT NULL,          -- PasswordHash, required
    isDeleted BIT DEFAULT 0 -- isDeleted, DEFAULT 0 => isDeleted = false
);

--****************************** TABLES FOR GENERAL DATA ******************************

CREATE TABLE StoreManagement.dbo.Stores (
    StoreID UNIQUEIDENTIFIER PRIMARY KEY, -- UserID, must be UNIQUEIDENTIFIER
    StoreBranch NVARCHAR(50)  NOT NULL,			-- required
	StoreAddress NVARCHAR(100)  NOT NULL,		-- required
    isDeleted BIT DEFAULT 0 -- isDeleted, DEFAULT 0 => isDeleted = false
);

CREATE TABLE StoreManagement.dbo.Items (
    ItemID UNIQUEIDENTIFIER PRIMARY KEY, -- ItemID, must be UNIQUEIDENTIFIER
    ItemCode NVARCHAR(50)  NOT NULL,			-- required
    ItemDescription NVARCHAR(50)  NOT NULL,			-- required
	ItemPrice DECIMAL(10, 2) NOT NULL,		-- required
	ItemImg IMAGE  NOT NULL,		-- required
	ItemStock INT NOT NULL,
    isDeleted BIT DEFAULT 0 -- isDeleted, DEFAULT 0 => isDeleted = false
);

-- ****************************** ABLES FOR RELATIONSHIPS ******************************
CREATE TABLE StoreManagement.dbo.ItemsStoresRelationship (
	ID UNIQUEIDENTIFIER PRIMARY KEY, -- must be UNIQUEIDENTIFIER
	ItemID UNIQUEIDENTIFIER,
	StoreID UNIQUEIDENTIFIER,
    FOREIGN KEY (ItemID) REFERENCES Items(ItemID),
    FOREIGN KEY (StoreID) REFERENCES Stores(StoreID),
    OperationDate DATETIME NOT NULL,
    isDeleted BIT DEFAULT 0, -- isDeleted, DEFAULT 0 => isDeleted = false
);

CREATE TABLE StoreManagement.dbo.ItemsClientsRelationship (
	ID UNIQUEIDENTIFIER PRIMARY KEY, -- must be UNIQUEIDENTIFIER
	ClientID UNIQUEIDENTIFIER,
	ItemID UNIQUEIDENTIFIER,
	ItemAmount INT NOT NULL,
    FOREIGN KEY (ClientID) REFERENCES Clients(ClientID),
    FOREIGN KEY (ItemID) REFERENCES Items(ItemID),
    OperationDate DATETIME NOT NULL,
    isDeleted BIT DEFAULT 0, -- isDeleted, DEFAULT 0 => isDeleted = false
    WasSold BIT DEFAULT 0 -- isDeleted, DEFAULT 0 => isDeleted = false);
);

--****************************** CONTROL TABLES ******************************
CREATE TABLE StoreManagement.dbo.SessionLog (
    SessionID UNIQUEIDENTIFIER PRIMARY KEY, 
    ClientID UNIQUEIDENTIFIER,         -- Foreign key
    InitSession DATETIME NOT NULL,          
    EndSession DATETIME
    FOREIGN KEY (ClientID) REFERENCES Clients(ClientID)
);

CREATE TABLE StoreManagement.dbo.OperationLog (
    OperationID INT IDENTITY(1,1) PRIMARY KEY, 
    SessionID UNIQUEIDENTIFIER,      -- Foreign key  
    OperationDate DATETIME,         
    Request NVARCHAR(MAX),          
    Response NVARCHAR(MAX)
    FOREIGN KEY (SessionID) REFERENCES SessionLog(SessionID)
);

-- Create a table named StatusCodes within the EmailVerifyServiceDB database to store status descriptions
CREATE TABLE StoreManagement.dbo.StatusCodes (  
    -- Unique identifier for each status, auto-incremented starting from 1 with an increment of 1
    ID INT IDENTITY(1,1) PRIMARY KEY,  
    -- Description of the status in NVARCHAR format
    StatusDescription NVARCHAR(100) NOT NULL 
);  
  
-- Insert predefined statuses into the StatusCodes table. These are static values that represent different states or outcomes.
INSERT INTO StoreManagement.dbo.StatusCodes (StatusDescription)  
VALUES ('Pending'), ('Verified'), ('Expired');  

CREATE TABLE StoreManagement.dbo.VerifyCodes (
    ID UNIQUEIDENTIFIER PRIMARY KEY, 
    ClientID UNIQUEIDENTIFIER,
    Token UNIQUEIDENTIFIER,         -- Foreign key
    VerifyStatus INT,
    Code NVARCHAR (15),
    CreationDate DATETIME NOT NULL,          
    VerifyDate DATETIME
    FOREIGN KEY (ClientID) REFERENCES Clients(ClientID)
);
