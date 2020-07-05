SELECT
	Piece.PerformanceDate as Date, 
	Location.Name as Location, 
	Piece.SetNo AS "Set", 
	Piece.PieceNo, 
	"set".ActName as Act, 
	Performance.NewsletterDate as Newsletter, 
	Piece.Title, 
	Piece.AudioPath, 
	Piece.VideoPath, 
	Piece.Comments
FROM Piece
INNER JOIN "set"
	ON "set".PerformanceDate = Piece.PerformanceDate
	and "set".LocationId = Piece.LocationId
	and "set".SetNo = Piece.SetNo
INNER JOIN Location
	ON Location.LocationId = Piece.LocationId
INNER JOIN Performance
	ON Piece.PerformanceDate = Performance.PerformanceDate
	and Piece.LocationId = Performance.LocationId
where "set".ActName = @Act
ORDER BY
	Piece.PerformanceDate, 
	Location.Name,
	Piece.SetNo, 
	Piece.PieceNo