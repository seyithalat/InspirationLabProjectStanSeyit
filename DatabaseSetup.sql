-- Check if database exists, if not create it
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'InspirationLab')
BEGIN
    CREATE DATABASE InspirationLab;
END
GO

USE InspirationLab;
GO

-- Create Users table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(50) NOT NULL UNIQUE,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        Password NVARCHAR(255) NOT NULL,
        Name NVARCHAR(100) NOT NULL,
        CreatedAt DATETIME DEFAULT GETDATE(),
        LastLogin DATETIME,
        IsActive BIT DEFAULT 1
    );
END
GO

-- Create Friends table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Friends')
BEGIN
    CREATE TABLE Friends (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        FriendId INT NOT NULL,
        Status NVARCHAR(20) DEFAULT 'Pending', -- Pending, Accepted, Blocked
        CreatedAt DATETIME DEFAULT GETDATE(),
        UpdatedAt DATETIME,
        FOREIGN KEY (UserId) REFERENCES Users(Id),
        FOREIGN KEY (FriendId) REFERENCES Users(Id),
        CONSTRAINT UQ_Friends_UserId_FriendId UNIQUE (UserId, FriendId)
    );
END
GO

-- Create Messages table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Messages')
BEGIN
    CREATE TABLE Messages (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        SenderId INT NOT NULL,
        ReceiverId INT NOT NULL,
        Content NVARCHAR(MAX) NOT NULL,
        SentAt DATETIME DEFAULT GETDATE(),
        IsRead BIT DEFAULT 0,
        FOREIGN KEY (SenderId) REFERENCES Users(Id),
        FOREIGN KEY (ReceiverId) REFERENCES Users(Id)
    );
END
GO

-- Create StudyGroups table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'StudyGroups')
BEGIN
    CREATE TABLE StudyGroups (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(MAX),
        CreatedBy INT NOT NULL,
        CreatedAt DATETIME DEFAULT GETDATE(),
        IsActive BIT DEFAULT 1,
        FOREIGN KEY (CreatedBy) REFERENCES Users(Id)
    );
END
GO

-- Create StudyGroupMembers table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'StudyGroupMembers')
BEGIN
    CREATE TABLE StudyGroupMembers (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        GroupId INT NOT NULL,
        UserId INT NOT NULL,
        Role NVARCHAR(20) DEFAULT 'Member', -- Member, Admin
        JoinedAt DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (GroupId) REFERENCES StudyGroups(Id),
        FOREIGN KEY (UserId) REFERENCES Users(Id),
        CONSTRAINT UQ_StudyGroupMembers_GroupId_UserId UNIQUE (GroupId, UserId)
    );
END
GO

-- Create StudyLocations table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'StudyLocations')
BEGIN
    CREATE TABLE StudyLocations (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(MAX),
        Address NVARCHAR(255),
        Latitude DECIMAL(9,6),
        Longitude DECIMAL(9,6),
        CreatedAt DATETIME DEFAULT GETDATE(),
        IsActive BIT DEFAULT 1
    );
END
GO

-- Insert sample study locations if they don't exist
IF NOT EXISTS (SELECT * FROM StudyLocations WHERE Name = 'Main Campus Library')
BEGIN
    INSERT INTO StudyLocations (Name, Description, Address, Latitude, Longitude)
    VALUES 
        ('Main Campus Library', 'Main library with quiet study areas', 'Campus Main Building', 51.0283, 4.4808),
        ('Science Wing', 'Science building study rooms', 'Science Building', 51.0285, 4.4810),
        ('Study Cafe', 'Cafe with study spaces', 'Student Center', 51.0280, 4.4805);
END
GO

-- Create indexes if they don't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Username' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE INDEX IX_Users_Username ON Users(Username);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE INDEX IX_Users_Email ON Users(Email);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Friends_UserId' AND object_id = OBJECT_ID('Friends'))
BEGIN
    CREATE INDEX IX_Friends_UserId ON Friends(UserId);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Friends_FriendId' AND object_id = OBJECT_ID('Friends'))
BEGIN
    CREATE INDEX IX_Friends_FriendId ON Friends(FriendId);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Messages_SenderId' AND object_id = OBJECT_ID('Messages'))
BEGIN
    CREATE INDEX IX_Messages_SenderId ON Messages(SenderId);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Messages_ReceiverId' AND object_id = OBJECT_ID('Messages'))
BEGIN
    CREATE INDEX IX_Messages_ReceiverId ON Messages(ReceiverId);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_StudyGroupMembers_GroupId' AND object_id = OBJECT_ID('StudyGroupMembers'))
BEGIN
    CREATE INDEX IX_StudyGroupMembers_GroupId ON StudyGroupMembers(GroupId);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_StudyGroupMembers_UserId' AND object_id = OBJECT_ID('StudyGroupMembers'))
BEGIN
    CREATE INDEX IX_StudyGroupMembers_UserId ON StudyGroupMembers(UserId);
END
GO 