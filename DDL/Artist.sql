DROP TABLE IF EXISTS ArtistInImage ;
DROP TABLE IF EXISTS Credit ;
DROP TABLE IF EXISTS Artist ;

CREATE TABLE IF NOT EXISTS Artist (
  ArtistName citext NOT NULL ,
  Forename citext NOT NULL ,
  Surname citext NOT NULL ,
  Comments citext NOT NULL DEFAULT '' ,
  CONSTRAINT PK_Artist PRIMARY KEY (
    ArtistName),
  CONSTRAINT UK_Artist UNIQUE (
    Surname, 
    Forename))
;

grant select, insert, update, delete
	on Artist
	to Fred;

CREATE TABLE IF NOT EXISTS Credit (
  PerformanceDate DATE NOT NULL ,
  LocationId citext NOT NULL ,
  SetNo INT NOT NULL ,
  PieceNo INT NOT NULL ,
  ArtistName citext NOT NULL ,
  RoleId citext NOT NULL ,
  CONSTRAINT PK_Credit PRIMARY KEY (
    PerformanceDate, 
    LocationId, 
    SetNo, 
    PieceNo, 
    ArtistName, 
    RoleId) ,
  CONSTRAINT FK_Credit_Artist
    FOREIGN KEY (ArtistName)
    REFERENCES Artist (ArtistName)
    ON DELETE NO ACTION
    ON UPDATE CASCADE,
  CONSTRAINT FK_Credit_Role
    FOREIGN KEY (RoleId )
    REFERENCES Role (RoleId )
    ON DELETE NO ACTION
    ON UPDATE CASCADE,
  CONSTRAINT FK_Credit_Piece
    FOREIGN KEY (
      PerformanceDate , 
      LocationId , 
      SetNo , 
      PieceNo )
    REFERENCES Piece (
      PerformanceDate , 
      LocationId , 
      SetNo , 
      PieceNo )
    ON DELETE NO ACTION
    ON UPDATE CASCADE)
;

grant select, insert, update, delete
	on Credit
	to Fred;


CREATE TABLE IF NOT EXISTS ArtistInImage (
  ImageId INT NOT NULL ,
  ArtistName citext NOT NULL ,
  CONSTRAINT PK_ArtistInImage PRIMARY KEY (
    ImageId, 
    ArtistName) ,
  CONSTRAINT FK_ArtistInImage_Artist
    FOREIGN KEY (ArtistName)
    REFERENCES Artist (ArtistName)
    ON DELETE NO ACTION
    ON UPDATE CASCADE,
  CONSTRAINT FK_ArtistInImage_Image
    FOREIGN KEY (ImageId )
    REFERENCES Image (ImageId )
    ON DELETE NO ACTION
    ON UPDATE CASCADE)
;

grant select, insert, update, delete
	on ArtistInImage
	to Fred;
