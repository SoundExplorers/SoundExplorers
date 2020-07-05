﻿SELECT
	Performance.PerformanceDate as Date, 
	Location.Name as Location, 
	Series.Name as Series,
	Performance.NewsletterDate as Newsletter, 
	Performance.Comments
FROM Performance
INNER JOIN Location
	ON Performance.LocationId = Location.LocationId
INNER JOIN Series
	ON Performance.SeriesId = Series.SeriesId
where Performance.PerformanceDate = @Date
and Performance.LocationId in (
    select LocationId
    from Location
    where Location.Name = @Location)
ORDER BY
	Performance.PerformanceDate, 
	Location.Name
