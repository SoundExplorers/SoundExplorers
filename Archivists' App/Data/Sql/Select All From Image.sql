SELECT
	Image.ImageId,
	Location.Name as Location, 
	Image.PerformanceDate as Date, 
	Image.Title, 
	Image.Path, 
	Image.Comments
FROM Image
INNER JOIN Location
	ON Location.LocationId = Image.LocationId
ORDER BY
	Location.Name, 
	Image.PerformanceDate, 
	Image.Title, 
	Image.Path