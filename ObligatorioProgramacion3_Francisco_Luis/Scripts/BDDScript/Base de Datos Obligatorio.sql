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
-- Domingo 14 junio 2025 08:00 AM
INSERT INTO RadioPrograms (ProgramName, ImageURL, RadioDescription, Schedule)
VALUES ('Mañanas Radiales', '/images/manha.jpg', 'Programa matutino para arrancar el día', '2025-06-15 08:00:00');

-- Lunes 16 junio 2025 14:00 PM
INSERT INTO RadioPrograms (ProgramName, ImageURL, RadioDescription, Schedule)
VALUES ('Tardes Musicales', '/images/tarde.jpg', 'Música y entretenimiento para la tarde', '2025-06-16 14:00:00');

-- Martes 17 junio 2025 20:00 PM
INSERT INTO RadioPrograms (ProgramName, ImageURL, RadioDescription, Schedule)
VALUES ('Noche de Charla', '/images/noche.jpg', 'Charlas y entrevistas para la noche', '2025-06-17 20:00:00');

-- Miércoles 18 junio 2025 10:00 AM
INSERT INTO RadioPrograms (ProgramName, ImageURL, RadioDescription, Schedule)
VALUES ('Noticias y Actualidad', '/images/noticias.jpg', 'Resumen de noticias diarias', '2025-06-18 10:00:00');

-- Jueves 19 junio 2025 18:00 PM
INSERT INTO RadioPrograms (ProgramName, ImageURL, RadioDescription, Schedule)
VALUES ('Hora Feliz', '/images/horafeliz.jpg', 'Programa de humor y diversión', '2025-06-19 18:00:00');

-- Viernes 20 junio 2025 12:00 PM
INSERT INTO RadioPrograms (ProgramName, ImageURL, RadioDescription, Schedule)
VALUES ('Almuerzos en Vivo', '/images/almuerzo.jpg', 'Entrevistas y música en el mediodía', '2025-06-20 12:00:00');

-- Sábado 21 junio 2025 22:00 PM
INSERT INTO RadioPrograms (ProgramName, ImageURL, RadioDescription, Schedule)
VALUES ('Música Nocturna', '/images/musica.jpg', 'Lo mejor de la música para la noche', '2025-06-21 22:00:00');

-- CLIENTES (vinculados a usuarios)
INSERT INTO Clients (IDNumber, FirstName, LastName, Email, BirthDate, UserID)
VALUES 
('12345678', 'María', 'González', 'maria.gonzalez@example.com', '1995-03-12', 1),
('87654321', 'Carlos', 'Ramírez', 'carlos.ramirez@example.com', '1990-07-25', 2);

-- USUARIOS adicionales con contraseña '1234'
INSERT INTO Users (UserName, Email, UserPassword, RoleID)
VALUES 
('AnaSuarez', 'ana.suarez@example.com', CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', '1234'), 2), 3),
('JoseLopez', 'jose.lopez@example.com', CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', '1234'), 2), 3);

-- CLIENTES para estos usuarios
INSERT INTO Clients (IDNumber, FirstName, LastName, Email, BirthDate, UserID)
VALUES 
('11223344', 'Ana', 'Suárez', 'ana.suarez@example.com', '1998-06-01', 3),
('55667788', 'José', 'López', 'jose.lopez@example.com', '1992-09-15', 4);

-- HOSTS
INSERT INTO Hosts (HostName, Bio, ProgramID)
VALUES 
('Lucía Méndez', 'Periodista y conductora de larga trayectoria.', 1),
('Ramiro Varela', 'Especialista en música urbana y entrevistas.', 2),
('Sofía Ortiz', 'Locutora profesional con enfoque social.', 3);

-- COMENTARIOS
INSERT INTO Comments (ClientID, ProgramID, Comment)
VALUES 
('12345678', 1, 'Excelente programa para comenzar el día.'),
('87654321', 2, 'Buena música y contenido variado.'),
('11223344', 3, 'Muy interesante la entrevista de hoy.'),
('55667788', 1, 'Me encantó la selección musical.');

-- SPONSORS
INSERT INTO Sponsors (SponsorsName, SponsorDescription, CantPlan)
VALUES 
('Café el Amanecer', 'Apoya el programa Mañanas Radiales', 2),
('Supermercado TuAhorro', 'Patrocinador oficial de Tardes Musicales', 3);

-- CLIMA (datos para hoy y mañana)
INSERT INTO Weather (WeatherDate, Temperature, WeatherDescription, Icon)
VALUES 
(GETDATE(), 18.5, 'Parcialmente nublado', '/icons/nublado.png'),
(DATEADD(DAY, 1, GETDATE()), 22.3, 'Soleado', '/icons/soleado.png');

-- COTIZACIONES (USD, EUR, BRL)
INSERT INTO ExchangeRates (ExchangeDate, CurrencyType, ExchangeValue)
VALUES 
(GETDATE(), 'USD', 38.75),
(GETDATE(), 'EUR', 41.20),
(GETDATE(), 'BRL', 7.85);

-- NOTICIAS
INSERT INTO News (Title, Content, PublishDate, ImageURL)
VALUES 
('Nuevo estudio sobre el impacto de la música', 'Una investigación reciente sugiere que la música puede mejorar el estado de ánimo y la productividad.', GETDATE(), '/images/noticia1.jpg'),
('Anunciada nueva programación de invierno', 'La radio local lanza una nueva grilla de programas para la temporada invernal.', GETDATE(), '/images/noticia2.jpg');
