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

-- Seed Users (2 Organisers, 2 Participants) [21]
-- Password hashes represent the secure hashing implementation planned for Part 2 [12]
INSERT INTO Users (Email, PasswordHash, FirstName, LastName, RoleId, ProfilePictureUrl) VALUES
('organiser_sipho@raceday.co.za', 'SECURE_HASHED_PASS_SIPHO', 'Sipho', 'Khumalo', 1, NULL),
('organiser_chantal@raceday.co.za', 'SECURE_HASHED_PASS_CHANTAL', 'Chantal', 'Du Toit', 1, NULL),
('participant_jabu@raceday.co.za', 'SECURE_HASHED_PASS_JABU', 'Jabu', 'Adele', 2, 'https://racedaystorage.blob.core.windows.net/profiles/jabu.jpg'),
('participant_sarah@raceday.co.za', 'SECURE_HASHED_PASS_SARAH', 'Sarah', 'Smith', 2, 'https://racedaystorage.blob.core.windows.net/profiles/sarah.jpg');

-- Seed Events (3 Events: Run, Cycle, Walk) [21, 22]
INSERT INTO Events (Name, Description, Date, Location, Distance, EventType, BannerImageUrl, OrganiserId) VALUES
('Soweto Half Marathon', 'An exciting running event through the historic streets of Soweto.', '2026-11-01 06:00:00', 'Soweto, Johannesburg', 21.10, 'Run', 'https://racedaystorage.blob.core.windows.net/banners/soweto.jpg', 1),
('Cape Town Cycle Classic', 'A breathtaking cycle tour along the Cape peninsula.', '2026-10-12 07:00:00', 'Cape Town', 109.00, 'Cycle', 'https://racedaystorage.blob.core.windows.net/banners/ct-cycle.jpg', 2),
('Gauteng Community Walk', 'A fun walk for the whole family promoting healthy living.', '2026-09-20 08:00:00', 'Pretoria', 5.00, 'Walk', 'https://racedaystorage.blob.core.windows.net/banners/gauteng-walk.jpg', 1);