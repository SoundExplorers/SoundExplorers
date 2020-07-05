SELECT
	"set".PerformanceDate as Date, 
	Location.Name as Location, 
	"set".SetNo, 
	"set".ActName as Act, 
	Performance.NewsletterDate as Newsletter, 
	"set".Comments
FROM "set"
INNER JOIN Location
	ON "set".LocationId = Location.LocationId
INNER JOIN Performance
	ON "set".PerformanceDate = Performance.PerformanceDate
	and "set".LocationId = Performance.LocationId
ORDER BY
	"set".PerformanceDate, 
	Location.Name, 
	"set".SetNo