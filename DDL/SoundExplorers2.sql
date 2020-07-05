DROP TABLE IF EXISTS ArtistInImage ;
DROP TABLE IF EXISTS Credit ;
DROP TABLE IF EXISTS Image ;
DROP TABLE IF EXISTS Piece ;
DROP TABLE IF EXISTS Set ;
DROP TABLE IF EXISTS Performance ;
DROP TABLE IF EXISTS Act ;
DROP TABLE IF EXISTS Artist ;
DROP TABLE IF EXISTS Location;
DROP TABLE IF EXISTS Newsletter ;
DROP TABLE IF EXISTS Series ;
DROP TABLE IF EXISTS Role ;
DROP TABLE IF EXISTS UserOption ;
DROP ROLE IF EXISTS Fred;

CREATE ROLE Fred LOGIN
  ENCRYPTED PASSWORD 'md580ba6374b44abcc488ca5c8240f37394'
  NOSUPERUSER INHERIT NOCREATEDB NOCREATEROLE REPLICATION;

CREATE  TABLE IF NOT EXISTS Act (
  ActName VARCHAR(255) NOT NULL ,
  Comments VARCHAR(8000) NOT NULL DEFAULT '' ,
  CONSTRAINT PK_Act PRIMARY KEY (ActName) )
;

grant select, insert, update, delete
	on Act
	to Fred;

CREATE  TABLE IF NOT EXISTS Artist (
  ArtistName VARCHAR(255) NOT NULL ,
  Comments VARCHAR(8000) NOT NULL DEFAULT '' ,
  CONSTRAINT PK_Artist PRIMARY KEY (ArtistName) )
;

grant select, insert, update, delete
	on Artist
	to Fred;

CREATE  TABLE IF NOT EXISTS Location (
  LocationId VARCHAR(3) NOT NULL ,
  Name VARCHAR(255) NULL ,
  Comments VARCHAR(8000) NOT NULL DEFAULT '' ,
  CONSTRAINT PK_Location PRIMARY KEY (LocationId) ,
  CONSTRAINT UK_Location_Name UNIQUE (Name) )
;

grant select, insert, update, delete
	on Location
	to Fred;

CREATE  TABLE IF NOT EXISTS Newsletter (
  NewsletterDate DATE NOT NULL ,
  Path VARCHAR(511) NULL ,
  CONSTRAINT PK_Newsletter PRIMARY KEY (NewsletterDate) ,
  CONSTRAINT UK_Newsletter_Path UNIQUE (Path) )
;

grant select, insert, update, delete
	on Newsletter
	to Fred;

CREATE  TABLE IF NOT EXISTS Series (
  SeriesId VARCHAR(3) NOT NULL ,
  Name VARCHAR(255) NULL ,
  Comments VARCHAR(8000) NOT NULL DEFAULT '' ,
  CONSTRAINT PK_Series PRIMARY KEY (SeriesId) ,
  CONSTRAINT UK_Series_Name UNIQUE (Name) )
;

grant select, insert, update, delete
	on Series
	to Fred;

CREATE  TABLE IF NOT EXISTS Role (
  RoleId VARCHAR(3) NOT NULL ,
  Name VARCHAR(255) NULL ,
  CONSTRAINT PK_Role PRIMARY KEY (RoleId) ,
  CONSTRAINT UK_Role_Name UNIQUE (Name) )
;

grant select, insert, update, delete
	on Role
	to Fred;

CREATE  TABLE IF NOT EXISTS UserOption (
  UserId VARCHAR(255) NOT NULL ,
  OptionName VARCHAR(225) NOT NULL ,
  OptionValue VARCHAR(8000) NOT NULL ,
  CONSTRAINT PK_UserOption PRIMARY KEY (UserId, OptionName) )
;
grant select, insert, update, delete
	on UserOption
	to Fred;

CREATE  TABLE IF NOT EXISTS Performance (
  PerformanceDate DATE NOT NULL ,
  LocationId VARCHAR(3) NOT NULL ,
  SeriesId VARCHAR(3) NOT NULL ,
  NewsletterDate DATE NOT NULL DEFAULT '1900-01-01' ,
  Comments VARCHAR(8000) NOT NULL DEFAULT '' ,
  CONSTRAINT PK_Performance PRIMARY KEY (PerformanceDate, LocationId) ,
  CONSTRAINT FK_Performance_Location
    FOREIGN KEY (LocationId )
    REFERENCES Location (LocationId )
    ON DELETE NO ACTION
    ON UPDATE CASCADE,
  CONSTRAINT FK_Performance_Series
    FOREIGN KEY (SeriesId )
    REFERENCES Series (SeriesId )
    ON DELETE NO ACTION
    ON UPDATE CASCADE,
  CONSTRAINT FK_Performance_Newsletter
    FOREIGN KEY (NewsletterDate )
    REFERENCES Newsletter (NewsletterDate )
    ON DELETE NO ACTION
    ON UPDATE CASCADE)
 ;

grant select, insert, update, delete
	on Performance
	to Fred;

CREATE  TABLE IF NOT EXISTS Set (
  PerformanceDate DATE NOT NULL ,
  LocationId VARCHAR(3) NOT NULL ,
  SetNo INT NOT NULL ,
  ActName VARCHAR(255) NOT NULL DEFAULT '' ,
  Comments VARCHAR(8000) NOT NULL DEFAULT '' ,
  CONSTRAINT PK_Set PRIMARY KEY (PerformanceDate, LocationId, SetNo) ,
  CONSTRAINT FK_Set_Performance
    FOREIGN KEY (PerformanceDate , LocationId )
    REFERENCES Performance (PerformanceDate , LocationId )
    ON DELETE NO ACTION
    ON UPDATE CASCADE,
  CONSTRAINT FK_Set_Act
    FOREIGN KEY (ActName )
    REFERENCES Act (ActName )
    ON DELETE NO ACTION
    ON UPDATE CASCADE)
;

grant select, insert, update, delete
	on Set
	to Fred;

CREATE  TABLE IF NOT EXISTS Piece (
  PerformanceDate DATE NOT NULL ,
  LocationId VARCHAR(3) NOT NULL ,
  SetNo INT NOT NULL ,
  PieceNo INT NOT NULL ,
  Title VARCHAR(255) NOT NULL ,
  AudioPath VARCHAR(511) NOT NULL DEFAULT '' ,
  VideoPath VARCHAR(511) NOT NULL DEFAULT '' ,
  Comments VARCHAR(8000) NOT NULL DEFAULT '' ,
  CONSTRAINT PK_Piece PRIMARY KEY (PerformanceDate, LocationId, SetNo, PieceNo) ,
  CONSTRAINT FK_Piece_Set
    FOREIGN KEY (PerformanceDate , LocationId , SetNo )
    REFERENCES Set (PerformanceDate , LocationId , SetNo )
    ON DELETE NO ACTION
    ON UPDATE CASCADE)
;

grant select, insert, update, delete
	on Piece
	to Fred;

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

CREATE  TABLE IF NOT EXISTS Image (
  ImageId SERIAL ,
  Path VARCHAR(511) NOT NULL ,
  Title VARCHAR(255) NOT NULL DEFAULT '' ,
  PerformanceDate DATE NOT NULL DEFAULT '1900/01/01' ,
  LocationId VARCHAR(3) NOT NULL DEFAULT '' ,
  Comments VARCHAR(8000) NOT NULL DEFAULT '' ,
  CONSTRAINT PK_Image PRIMARY KEY (ImageId) ,
  CONSTRAINT UK_Image_Path UNIQUE (Path) ,
  CONSTRAINT FK_Image_Performance
    FOREIGN KEY (PerformanceDate , LocationId )
    REFERENCES Performance (PerformanceDate , LocationId )
    ON DELETE NO ACTION
    ON UPDATE CASCADE)
;

grant select, insert, update, delete
	on Image
	to Fred;

CREATE  TABLE IF NOT EXISTS ArtistInImage (
  ImageId INT NOT NULL ,
  ArtistName VARCHAR(255) NOT NULL ,
  CONSTRAINT PK_ArtistInImage PRIMARY KEY (ImageId, ArtistName) ,
  CONSTRAINT FK_ArtistInImage_Artist
    FOREIGN KEY (ArtistName )
    REFERENCES Artist (ArtistName )
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
