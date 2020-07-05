DROP TABLE IF EXISTS RoleInPiece ;
DROP TABLE IF EXISTS Credit ;

CREATE  TABLE IF NOT EXISTS Credit (
  PerformanceDate DATE NOT NULL ,
  LocationId VARCHAR(3) NOT NULL ,
  SetNo INT NOT NULL ,
  PieceNo INT NOT NULL ,
  ArtistName VARCHAR(255) NOT NULL ,
  RoleId VARCHAR(3) NOT NULL ,
  CONSTRAINT PK_Credit PRIMARY KEY (PerformanceDate, LocationId, SetNo, PieceNo, ArtistName, RoleId) ,
  CONSTRAINT FK_Credit_Artist
    FOREIGN KEY (ArtistName )
    REFERENCES Artist (ArtistName )
    ON DELETE NO ACTION
    ON UPDATE CASCADE,
  CONSTRAINT FK_Credit_Role
    FOREIGN KEY (RoleId )
    REFERENCES Role (RoleId )
    ON DELETE NO ACTION
    ON UPDATE CASCADE,
  CONSTRAINT FK_Credit_Piece
    FOREIGN KEY (PerformanceDate , LocationId , SetNo , PieceNo )
    REFERENCES Piece (PerformanceDate , LocationId , SetNo , PieceNo )
    ON DELETE NO ACTION
    ON UPDATE CASCADE)
;

grant select, insert, update, delete
	on Credit
	to Fred;
