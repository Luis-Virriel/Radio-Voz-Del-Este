CREATE DATABASE Radio
USE Radio;
GO
CREATE TABLE Roles (
    ID INT PRIMARY KEY IDENTITY(1,1),
    RoleName VARCHAR(50) NOT NULL  
);

CREATE TABLE Users (
    ID INT PRIMARY KEY IDENTITY(1,1),
    UserName VARCHAR(30) UNIQUE,  
    Email VARCHAR(255) UNIQUE,  
    UserPassword VARCHAR(255),  
    RoleID INT,
    FOREIGN KEY (RoleID) REFERENCES Roles(ID)
);

CREATE TABLE Clients (
    IDNumber VARCHAR(8) PRIMARY KEY,
    FirstName VARCHAR(30),
    LastName VARCHAR(30),
    Email VARCHAR(255) UNIQUE,  
    BirthDate DATE,
    UserID INT,
    FOREIGN KEY (UserID) REFERENCES Users(ID)
);

CREATE TABLE RadioPrograms (
    ID INT PRIMARY KEY IDENTITY(1,1),
    ProgramName VARCHAR(50),
    ImageURL VARCHAR(255),
    RadioDescription VARCHAR(255),
    Schedule DATETIME
);

CREATE TABLE Hosts (
    ID INT PRIMARY KEY IDENTITY(1,1),
    HostName VARCHAR(50),
    Bio VARCHAR(MAX), 
    ProgramID INT,
    FOREIGN KEY (ProgramID) REFERENCES RadioPrograms(ID)
);

CREATE TABLE HostProgram (
    IDHost INT,
    IDProgram INT,
    FOREIGN KEY (IDProgram) REFERENCES RadioPrograms(ID),
    FOREIGN KEY (IDHost) REFERENCES Hosts(ID),
    PRIMARY KEY (IDHost, IDProgram)
);

CREATE TABLE Sponsors (
    ID INT PRIMARY KEY IDENTITY(1,1),
    SponsorsName VARCHAR(50), 
    SponsorDescription VARCHAR(255),
    CantPlan INT 
);

CREATE TABLE News (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Title VARCHAR(100), 
    Content TEXT, 
    PublishDate DATE,
    ImageURL VARCHAR(255)
);

CREATE TABLE Comments (
    ID INT PRIMARY KEY IDENTITY(1,1),
    ClientID VARCHAR(8),
    ProgramID INT,
    Comment VARCHAR(500), 
    CommentDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ClientID) REFERENCES Clients(IDNumber),
    FOREIGN KEY (ProgramID) REFERENCES RadioPrograms(ID)
);

CREATE TABLE ExchangeRates (
    ID INT PRIMARY KEY IDENTITY(1,1),
    ExchangeDate DATE,
    CurrencyType VARCHAR(10) CHECK (CurrencyType IN ('USD', 'EUR', 'BRL')),
    ExchangeValue DECIMAL(10, 4)
);

CREATE TABLE Weather (
    ID INT PRIMARY KEY IDENTITY(1,1),
    WeatherDate DATE,  
    Temperature DECIMAL(5, 2),
    WeatherDescription TEXT,  
    Icon VARCHAR(255)
);

insert into Roles
(RoleName) values
('Admin'),('Editors'),('Clients')

-- Asegúrate de habilitar esto para que HASHBYTES funcione correctamente
SET CONCAT_NULL_YIELDS_NULL ON;


INSERT INTO Users (UserName, Email, UserPassword, RoleID)
VALUES (
    'LuisVirriel',
    'luisvirriel@gmail.com',
    CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', 'admin'), 2),
    1
);


INSERT INTO Users (UserName, Email, UserPassword, RoleID)
VALUES (
    'FranciscoPerez',
    'franperez19092003@gmail.com',
    CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', 'admin'), 2),
    1
);
