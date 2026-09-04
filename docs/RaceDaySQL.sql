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

-- 5. Create Categories Table (With ON DELETE CASCADE)
CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    EventId INT NOT NULL,
    Name NVARCHAR(100) NOT NULL, -- e.g., 'Senior', 'Under 20', '10km'
    CONSTRAINT FK_Categories_Events FOREIGN KEY (EventId) REFERENCES Events(EventId) ON DELETE CASCADE
);

-- 6. Create Enrolments Table (With UNIQUE composite registration check)
CREATE TABLE Enrolments (
    EnrolmentId INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantId INT NOT NULL,
    EventId INT NOT NULL,
    CategoryId INT NOT NULL,
    EnrolmentDate DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    CONSTRAINT FK_Enrolments_Participant FOREIGN KEY (ParticipantId) REFERENCES Users(UserId),
    CONSTRAINT FK_Enrolments_Events FOREIGN KEY (EventId) REFERENCES Events(EventId),
    CONSTRAINT FK_Enrolments_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId),
    CONSTRAINT UC_Participant_Event UNIQUE (ParticipantId, EventId), -- Enforces single registration [5]
    CONSTRAINT CHK_EnrolmentStatus CHECK (Status IN ('Pending', 'Confirmed'))
);

-- 7. Create Results Table (Enforces 1-to-1 via UNIQUE EnrolmentId)
CREATE TABLE Results (
    ResultId INT IDENTITY(1,1) PRIMARY KEY,
    EnrolmentId INT NOT NULL UNIQUE, -- Strict 1-to-1 cardinality [1, 7]
    FinishTime NVARCHAR(50) NOT NULL, -- Formatted HH:MM:SS [18]
    FinishPosition INT NOT NULL,
    CONSTRAINT FK_Results_Enrolments FOREIGN KEY (EnrolmentId) REFERENCES Enrolments(EnrolmentId) ON DELETE CASCADE,
    CONSTRAINT CHK_FinishPosition CHECK (FinishPosition > 0) -- Logical domain check
);

-- Seed Core Roles
INSERT INTO Roles (RoleName) VALUES ('Organiser'), ('Participant');