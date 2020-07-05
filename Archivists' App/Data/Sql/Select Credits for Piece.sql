SELECT
	Credit.PerformanceDate as Date, 
	Location.Name as Location, 
	Credit.SetNo AS "Set", 
	Credit.PieceNo, 
	Credit.ArtistName as Artist, 
	Role.Name as Role,
	"set".ActName as Act,
	Artist.Forename,
	Artist.Surname
FROM Credit
INNER JOIN Artist
	ON Artist.ArtistName = Credit.ArtistName
INNER JOIN Location
	ON Location.LocationId = Credit.LocationId
INNER JOIN Role
	ON Role.RoleId = Credit.RoleId
INNER JOIN "set"
	ON "set".PerformanceDate = Credit.PerformanceDate
	and "set".LocationId = Credit.LocationId
	and "set".SetNo = Credit.SetNo
where Credit.PerformanceDate = @Date
and Location.Name = @Location
and Credit.SetNo = @Set
and Credit.PieceNo = @Piece
ORDER BY
	Artist.Surname,
	Artist.Forename,
	Role.Name
