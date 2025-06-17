-- Created by Vertabelo (http://vertabelo.com)
-- Last modification date: 2024-06-24 20:13:53.576

-- tables
-- Table: Actor
CREATE TABLE Actor (
    IdActor int  NOT NULL IDENTITY,
    Name nvarchar(30)  NOT NULL,
    Surname nvarchar(30)  NOT NULL,
    CONSTRAINT Actor_pk PRIMARY KEY  (IdActor)
);

-- Table: Actor_Movie
CREATE TABLE Actor_Movie (
    IdActorMovie int  NOT NULL IDENTITY,
    IdMovie int  NOT NULL,
    IdActor int  NOT NULL,
    CharacterName nvarchar(50)  NOT NULL,
    CONSTRAINT Actor_Movie_pk PRIMARY KEY  (IdActorMovie)
);

-- Table: AgeRating
CREATE TABLE AgeRating (
    IdRating int  NOT NULL IDENTITY,
    Name nvarchar(30)  NOT NULL,
    CONSTRAINT AgeRating_pk PRIMARY KEY  (IdRating)
);

-- Table: Movie
CREATE TABLE Movie (
    IdMovie int  NOT NULL IDENTITY,
    IdAgeRating int  NOT NULL,
    Name nvarchar(30)  NOT NULL,
    ReleaseDate datetime  NOT NULL,
    CONSTRAINT Movie_pk PRIMARY KEY  (IdMovie)
);

-- foreign keys
-- Reference: Actor_Movie_Actor (table: Actor_Movie)
ALTER TABLE Actor_Movie ADD CONSTRAINT Actor_Movie_Actor
    FOREIGN KEY (IdActor)
    REFERENCES Actor (IdActor);

-- Reference: Actor_Movie_Movie (table: Actor_Movie)
ALTER TABLE Actor_Movie ADD CONSTRAINT Actor_Movie_Movie
    FOREIGN KEY (IdMovie)
    REFERENCES Movie (IdMovie);

-- Reference: Movie_Rating (table: Movie)
ALTER TABLE Movie ADD CONSTRAINT Movie_Rating
    FOREIGN KEY (IdAgeRating)
    REFERENCES AgeRating (IdRating);

-- End of file.

-- Insert test data into Actor table
INSERT INTO Actor (Name, Surname) VALUES 
('Robert', 'Downey'),
('Chris', 'Hemsworth'),
('Scarlett', 'Johansson'),
('Chris', 'Evans'),
('Mark', 'Ruffalo');

-- Insert test data into AgeRating table
INSERT INTO AgeRating (Name) VALUES 
('G'),
('PG'),
('PG-13'),
('R'),
('NC-17');

-- Insert test data into Movie table
INSERT INTO Movie (IdAgeRating, Name, ReleaseDate) VALUES 
(3, 'The Avengers', '2012-05-04'),
(3, 'Avengers: Age of Ultron', '2015-05-01'),
(3, 'Avengers: Infinity War', '2018-04-27'),
(3, 'Avengers: Endgame', '2019-04-26'),
(4, 'Deadpool', '2016-02-12');

-- Insert test data into Actor_Movie table
INSERT INTO Actor_Movie (IdMovie, IdActor, CharacterName) VALUES 
(1, 1, 'Tony Stark / Iron Man'),
(1, 2, 'Thor'),
(1, 3, 'Natasha Romanoff / Black Widow'),
(1, 4, 'Steve Rogers / Captain America'),
(1, 5, 'Bruce Banner / Hulk'),
(2, 1, 'Tony Stark / Iron Man'),
(2, 2, 'Thor'),
(2, 3, 'Natasha Romanoff / Black Widow'),
(2, 4, 'Steve Rogers / Captain America'),
(2, 5, 'Bruce Banner / Hulk'),
(3, 1, 'Tony Stark / Iron Man'),
(3, 2, 'Thor'),
(3, 3, 'Natasha Romanoff / Black Widow'),
(3, 4, 'Steve Rogers / Captain America'),
(3, 5, 'Bruce Banner / Hulk'),
(4, 1, 'Tony Stark / Iron Man'),
(4, 2, 'Thor'),
(4, 3, 'Natasha Romanoff / Black Widow'),
(4, 4, 'Steve Rogers / Captain America'),
(4, 5, 'Bruce Banner / Hulk'),
(5, 1, 'Wade Wilson / Deadpool');
