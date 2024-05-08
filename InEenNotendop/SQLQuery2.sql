USE PianoHeroDB
DROP TABLE IF EXISTS Scores;
DROP TABLE IF EXISTS Nummers;

CREATE TABLE Nummers (
    Title VARCHAR(255),
    Artiest VARCHAR(255),
    Lengte INT,
    Bpm INT,
    Moeilijkheid INT NOT NULL,
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Filepath varchar(255),
    CONSTRAINT CHK_Moeilijkheidsgraad CHECK (Moeilijkheid >= 1 AND Moeilijkheid <= 3)
    );

CREATE TABLE Scores (
    Score INT,
    NummerID INT,
    FOREIGN KEY (NummerID) REFERENCES Nummers(ID)
    );


DECLARE @CurrentTitle VARCHAR(255) = 'Title1';
DECLARE @CurrentTitleNumber INT = 1;

DECLARE @CurrentArtist VARCHAR(255) = 'Artiest1';;
DECLARE @CurrentArtistNumber INT = 1;



WHILE @CurrentTitleNumber <= 25
BEGIN
    SET @CurrentTitle = CONCAT('Title', @CurrentTitleNumber);
    SET @CurrentArtist = CONCAT('Artist', @CurrentArtistNumber)

    DECLARE @RandomLengte INT = ABS(CHECKSUM(NEWID())) % 151 + 150;
    DECLARE @RandomBpm INT = ABS(CHECKSUM(NEWID())) % 101 + 80;
    DECLARE @RandomMoeilijkheid INT = ABS(CHECKSUM(NEWID())) % 3 + 1;

    INSERT INTO Nummers (Title, Artiest, Lengte, Bpm, Moeilijkheid)
    VALUES (@CurrentTitle, @CurrentArtist, @RandomLengte, @RandomBpm, @RandomMoeilijkheid);

    DECLARE @LastInsertedId INT;
    SET @LastInsertedId = SCOPE_IDENTITY();

    INSERT INTO Scores (Score, NummerID)
    VALUES (0, @LastInsertedId);

    SET @CurrentTitleNumber = @CurrentTitleNumber + 1;
    SET @CurrentArtistNumber = @CurrentArtistNumber + 1;
END

SELECT n.Title, n.Artiest, n.Lengte, n.Bpm, n.Moeilijkheid, n.ID, n.Filepath, s.Score FROM dbo.Nummers AS n INNER JOIN dbo.Scores AS s ON n.ID = s.NummerID