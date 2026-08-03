----CREATE Database How to Create it ------
IF DB_ID(N'ScheduleManagementDB') IS NULL
BEGIN
    CREATE DATABASE ScheduleManagementDB;
END
GO

USE ScheduleManagementDB;
GO

------Roles Table Creation ----
CREATE TABLE dbo.Roles
(
    RoleId       INT IDENTITY(1,1) NOT NULL,
    RoleName     NVARCHAR(30) NOT NULL,

    IsActive     BIT NOT NULL
        CONSTRAINT DF_Roles_IsActive DEFAULT (1),  
    CreatedOn    DATETIME2(0) NOT NULL
        CONSTRAINT DF_Roles_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy    INT NULL,
    ModifiedOn   DATETIME2(0) NULL,
    ModifiedBy   INT NULL,

    CONSTRAINT PK_Roles PRIMARY KEY (RoleId),
    CONSTRAINT UQ_Roles_RoleName UNIQUE (RoleName)
);
GO
---Countries Table Created Here----
CREATE TABLE dbo.Countries
(
    CountryId    INT IDENTITY(1,1) NOT NULL,
    CountryName  NVARCHAR(100) NOT NULL,

    IsActive     BIT NOT NULL
        CONSTRAINT DF_Countries_IsActive DEFAULT (1),
    CreatedOn    DATETIME2(0) NOT NULL
        CONSTRAINT DF_Countries_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy    INT NULL,
    ModifiedOn   DATETIME2(0) NULL,
    ModifiedBy   INT NULL,

    CONSTRAINT PK_Countries PRIMARY KEY (CountryId),
    CONSTRAINT UQ_Countries_CountryName UNIQUE (CountryName)
);
GO
---Staties Table Created here----
CREATE TABLE dbo.States
(
    StateId      INT IDENTITY(1,1) NOT NULL,
    CountryId    INT NOT NULL,
    StateName    NVARCHAR(100) NOT NULL,

    IsActive     BIT NOT NULL
        CONSTRAINT DF_States_IsActive DEFAULT (1),
    CreatedOn    DATETIME2(0) NOT NULL
        CONSTRAINT DF_States_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy    INT NULL,
    ModifiedOn   DATETIME2(0) NULL,
    ModifiedBy   INT NULL,

    CONSTRAINT PK_States PRIMARY KEY (StateId),
    CONSTRAINT FK_States_Countries
        FOREIGN KEY (CountryId) REFERENCES dbo.Countries(CountryId),
    CONSTRAINT UQ_States_Country_State
        UNIQUE (CountryId, StateName)
);
GO
---Index Of States
CREATE INDEX IX_States_CountryId
ON dbo.States(CountryId);
GO
----Districts----
CREATE TABLE dbo.Districts
(
    DistrictId    INT IDENTITY(1,1) NOT NULL,
    StateId       INT NOT NULL,
    DistrictName  NVARCHAR(100) NOT NULL,

    IsActive      BIT NOT NULL
        CONSTRAINT DF_Districts_IsActive DEFAULT (1),
    CreatedOn     DATETIME2(0) NOT NULL
        CONSTRAINT DF_Districts_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy     INT NULL,
    ModifiedOn    DATETIME2(0) NULL,
    ModifiedBy    INT NULL,

    CONSTRAINT PK_Districts PRIMARY KEY (DistrictId),
    CONSTRAINT FK_Districts_States
        FOREIGN KEY (StateId) REFERENCES dbo.States(StateId),
    CONSTRAINT UQ_Districts_State_District
        UNIQUE (StateId, DistrictName)
);
GO
---Index of Districts 
CREATE INDEX IX_Districts_StateId
ON dbo.Districts(StateId);
GO
--
CREATE TABLE dbo.Users
(
    UserId          INT IDENTITY(1,1) NOT NULL,
    RoleId          INT NOT NULL,
    DistrictId      INT NULL,

    FullName        NVARCHAR(150) NOT NULL,
    Email           NVARCHAR(256) NOT NULL,
    PhoneNumber     NVARCHAR(20) NULL,
    PasswordHash    NVARCHAR(500) NOT NULL,
    FullAddress     NVARCHAR(500) NULL,

    IsActive        BIT NOT NULL
        CONSTRAINT DF_Users_IsActive DEFAULT (1),
    CreatedOn       DATETIME2(0) NOT NULL
        CONSTRAINT DF_Users_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy       INT NULL,
    ModifiedOn      DATETIME2(0) NULL,
    ModifiedBy      INT NULL,

    CONSTRAINT PK_Users PRIMARY KEY (UserId),
    CONSTRAINT FK_Users_Roles
        FOREIGN KEY (RoleId) REFERENCES dbo.Roles(RoleId),
    CONSTRAINT FK_Users_Districts
        FOREIGN KEY (DistrictId) REFERENCES dbo.Districts(DistrictId),
    CONSTRAINT UQ_Users_Email UNIQUE (Email)
);
GO
---UX_Users_PhoneNumber is Created As unique Index
CREATE UNIQUE INDEX UX_Users_PhoneNumber
ON dbo.Users(PhoneNumber)
WHERE PhoneNumber IS NOT NULL;
GO
------Role Id -----
CREATE INDEX IX_Users_RoleId
ON dbo.Users(RoleId);
GO
----Activity Types ----
CREATE TABLE dbo.ActivityTypes
(
    ActivityTypeId  INT IDENTITY(1,1) NOT NULL,
    ActivityName    NVARCHAR(100) NOT NULL,

    IsActive        BIT NOT NULL
        CONSTRAINT DF_ActivityTypes_IsActive DEFAULT (1),
    CreatedOn       DATETIME2(0) NOT NULL
        CONSTRAINT DF_ActivityTypes_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy       INT NULL,
    ModifiedOn      DATETIME2(0) NULL,
    ModifiedBy      INT NULL,

    CONSTRAINT PK_ActivityTypes PRIMARY KEY (ActivityTypeId),
    CONSTRAINT UQ_ActivityTypes_ActivityName UNIQUE (ActivityName)
);
GO
----Coach Availability----
CREATE TABLE dbo.CoachAvailabilities
(
    AvailabilityId  INT IDENTITY(1,1) NOT NULL,
    CoachId         INT NOT NULL,
    ActivityTypeId  INT NOT NULL,

    AvailableDate   DATE NOT NULL,
    StartTime       TIME(0) NOT NULL,
    EndTime         TIME(0) NOT NULL,
    IsBooked        BIT NOT NULL
        CONSTRAINT DF_CoachAvailabilities_IsBooked DEFAULT (0),

    IsActive        BIT NOT NULL
        CONSTRAINT DF_CoachAvailabilities_IsActive DEFAULT (1),
    CreatedOn       DATETIME2(0) NOT NULL
        CONSTRAINT DF_CoachAvailabilities_CreatedOn
        DEFAULT (SYSUTCDATETIME()),
    CreatedBy       INT NULL,
    ModifiedOn      DATETIME2(0) NULL,
    ModifiedBy      INT NULL,

    RowVersion      ROWVERSION NOT NULL,

    CONSTRAINT PK_CoachAvailabilities PRIMARY KEY (AvailabilityId),
    CONSTRAINT FK_CoachAvailabilities_Users
        FOREIGN KEY (CoachId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_CoachAvailabilities_ActivityTypes
        FOREIGN KEY (ActivityTypeId)
        REFERENCES dbo.ActivityTypes(ActivityTypeId),
    CONSTRAINT CK_CoachAvailabilities_Time
        CHECK (StartTime < EndTime),
    CONSTRAINT UQ_CoachAvailabilities_Schedule
        UNIQUE
        (
            CoachId,
            ActivityTypeId,
            AvailableDate,
            StartTime,
            EndTime
        )
);
GO
---CoachAvailabilities INDEX Creation
CREATE INDEX IX_CoachAvailabilities_Search
ON dbo.CoachAvailabilities
(
    ActivityTypeId,
    CoachId,
    AvailableDate,
    IsBooked,
    IsActive
);
GO
----Booking Table 
CREATE TABLE dbo.Bookings
(
    BookingId       INT IDENTITY(1,1) NOT NULL,
    UserId          INT NOT NULL,
    AvailabilityId  INT NOT NULL,

    BookingStatus   NVARCHAR(20) NOT NULL
        CONSTRAINT DF_Bookings_BookingStatus DEFAULT (N'Confirmed'),
    BookedOn        DATETIME2(0) NOT NULL
        CONSTRAINT DF_Bookings_BookedOn DEFAULT (SYSUTCDATETIME()),
    CancelledOn     DATETIME2(0) NULL,
    CancellationReason NVARCHAR(300) NULL,

    IsActive        BIT NOT NULL
        CONSTRAINT DF_Bookings_IsActive DEFAULT (1),
    CreatedOn       DATETIME2(0) NOT NULL
        CONSTRAINT DF_Bookings_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedBy       INT NULL,
    ModifiedOn      DATETIME2(0) NULL,
    ModifiedBy      INT NULL,

    CONSTRAINT PK_Bookings PRIMARY KEY (BookingId),
    CONSTRAINT FK_Bookings_Users
        FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_Bookings_CoachAvailabilities
        FOREIGN KEY (AvailabilityId)
        REFERENCES dbo.CoachAvailabilities(AvailabilityId),
    CONSTRAINT CK_Bookings_Status
        CHECK
        (
            BookingStatus IN
            (
                N'Confirmed',
                N'Completed',
                N'Cancelled'
            )
        )
);
GO
---Booking UserID ---
CREATE INDEX IX_Bookings_UserId
ON dbo.Bookings(UserId, BookingStatus);
GO
---Bookings_AvailabilityId
CREATE INDEX IX_Bookings_AvailabilityId
ON dbo.Bookings(AvailabilityId, BookingStatus);
GO

---Seed Data----
INSERT INTO dbo.Roles (RoleName)
VALUES
(N'Admin'),
(N'Coach'),
(N'User');
GO
---Seed Data----
INSERT INTO dbo.ActivityTypes (ActivityName)
VALUES
(N'Football'),
(N'Cricket'),
(N'Hockey'),
(N'Yoga');
GO
----Example Master data---
select * from [dbo].[Countries]
select * from [dbo].[States]
select * from [dbo].[Districts]
