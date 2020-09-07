update Performance set
	PerformanceDate = @Date, 
	LocationId = (
		select LocationId
		from Location
		where Name = @Location), 
	SeriesId = (
		select SeriesId
		from Series
		where Name = coalesce(@Series, '')),
	NewsletterDate = coalesce(@Newsletter, cast('01 Jan 1900' as date)), 
	Comments = coalesce(@Comments, '')
where PerformanceDate = @OLD_Date
and LocationId = (
	select LocationId
	from Location
	where Name = @OLD_Location)