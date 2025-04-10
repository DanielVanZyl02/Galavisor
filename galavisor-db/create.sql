-- Create UserRole table (independent table)
CREATE TABLE UserRole (
    UserRoleID SERIAL PRIMARY KEY,
    RoleName VARCHAR(50) NOT NULL UNIQUE
);
 
-- Create Planet table
CREATE TABLE Planet (
    PlanetID SERIAL PRIMARY KEY,
    Name VARCHAR(100) NOT NULL UNIQUE,
    Atmosphere VARCHAR(100),
    Temperature VARCHAR(50),
    Colour VARCHAR(50)
);
 
-- Create Activity table
CREATE TABLE Activity (
    ActivityID SERIAL PRIMARY KEY,
    Name VARCHAR(100) NOT NULL UNIQUE
);
 
-- Create Transport table
CREATE TABLE Transport (
    TransportID SERIAL PRIMARY KEY,
    Name VARCHAR(100) NOT NULL UNIQUE
);
 
-- Create User table with referential integrity
CREATE TABLE "User" (
    UserID SERIAL PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    PlanetID INT,
    UserRoleID INT,
	IsActive BOOLEAN DEFAULT True,
    FOREIGN KEY (PlanetID) REFERENCES Planet(PlanetID)
        ON DELETE SET NULL
        ON UPDATE CASCADE,
    FOREIGN KEY (UserRoleID) REFERENCES UserRole(UserRoleID)
        ON DELETE SET NULL
        ON UPDATE CASCADE
);
 
-- Create junction table for Planet-Activity many-to-many relationship
CREATE TABLE PlanetActivity (
    PlanetID INT,
    ActivityID INT,
    PRIMARY KEY (PlanetID, ActivityID),
    FOREIGN KEY (PlanetID) REFERENCES Planet(PlanetID)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
    FOREIGN KEY (ActivityID) REFERENCES Activity(ActivityID)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);
 
-- Create junction table for Planet-Transport many-to-many relationship
CREATE TABLE PlanetTransport (
    PlanetID INT,
    TransportID INT,
    PRIMARY KEY (PlanetID, TransportID),
    FOREIGN KEY (PlanetID) REFERENCES Planet(PlanetID)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
    FOREIGN KEY (TransportID) REFERENCES Transport(TransportID)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

CREATE TABLE Review (
    ReviewID SERIAL PRIMARY KEY,
	PlanetID INT,
    UserID INT,
    Rating INT NOT NULL,
    Comment VARCHAR(255),
    FOREIGN KEY (PlanetID) REFERENCES Planet(PlanetID)
        ON DELETE SET NULL
        ON UPDATE CASCADE,
    FOREIGN KEY (UserID) REFERENCES "User"(UserID)
        ON DELETE SET NULL
        ON UPDATE CASCADE
);