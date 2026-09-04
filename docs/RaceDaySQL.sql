-- 2. Role Table
CREATE TABLE Roles (
    RoleID INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL UNIQUE 
    );

-- 3. Create Users Table
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(256) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL, -- Secure salt+hash storage [12]
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    RoleId INT NOT NULL,
    ProfilePictureUrl NVARCHAR(500) NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)
);

-- 4. Create Events Table
CREATE TABLE Events (
    EventId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    Date DATETIME NOT NULL,
    Location NVARCHAR(256) NOT NULL,
    Distance DECIMAL(5,2) NOT NULL,
    EventType NVARCHAR(50) NOT NULL,
    BannerImageUrl NVARCHAR(500) NULL,
    OrganiserId INT NOT NULL,
    CONSTRAINT FK_Events_Organiser FOREIGN KEY (OrganiserId) REFERENCES Users(UserId),
    CONSTRAINT CHK_EventType CHECK (EventType IN ('Run', 'Walk', 'Cycle')), -- Domain validity [5]
    CONSTRAINT CHK_Distance CHECK (Distance > 0.0) -- Prevents negative/zero values
);