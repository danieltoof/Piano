USE PianoHeroDB
CREATE TABLE Nummers (
                            Title VARCHAR(255),
                            Artiest VARCHAR(255),
                            Lengte INT,
                            Bpm INT,
                            Moeilijkheid INT NOT NULL,
                            ID INT IDENTITY(1,1) PRIMARY KEY,
                            CONSTRAINT CHK_Moeilijkheidsgraad CHECK (Moeilijkheid >= 1 AND Moeilijkheid <= 3)
                        );

                        CREATE TABLE Scores (
                            Score INT,
                            NummerID INT,
                            FOREIGN KEY (NummerID) REFERENCES Nummers(ID)
                        );
                        
                        INSERT INTO Nummers (Title, Artiest, Lengte, Bpm, Moeilijkheid)
                        VALUES 
                            ('Title1', 'Artiest1', 180, 120, 1),
                            ('Title2', 'Artiest2', 190, 130, 2),
                            ('Title3', 'Artiest3', 200, 140, 3),
                            ('Title4', 'Artiest4', 210, 150, 1),
                            ('Title5', 'Artiest5', 220, 160, 2),
                            ('Title6', 'Artiest6', 230, 170, 3),
                            ('Title7', 'Artiest7', 240, 180, 1),
                            ('Title8', 'Artiest8', 250, 190, 2),
                            ('Title9', 'Artiest9', 260, 200, 3),
                            ('Title10', 'Artiest10', 270, 210, 1),
                            ('Title11', 'Artiest11', 280, 220, 2),
                            ('Title12', 'Artiest12', 290, 230, 3),
                            ('Title13', 'Artiest13', 300, 240, 1),
                            ('Title14', 'Artiest14', 310, 250, 2),
                            ('Title15', 'Artiest15', 320, 260, 3),
                            ('Title16', 'Artiest16', 330, 270, 1),
                            ('Title17', 'Artiest17', 340, 280, 2),
                            ('Title18', 'Artiest18', 350, 290, 3),
                            ('Title19', 'Artiest19', 360, 300, 1),
                            ('Title20', 'Artiest20', 370, 310, 2),
                            ('Title21', 'Artiest21', 380, 320, 3),
                            ('Title22', 'Artiest22', 390, 330, 1),
                            ('Title23', 'Artiest23', 400, 340, 2),
                            ('Title24', 'Artiest24', 410, 350, 3),
                            ('Title25', 'Artiest25', 420, 360, 1);
                        