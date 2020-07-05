insert into Performance (
	PerformanceDate, 
	LocationId, 
	SeriesId,
	NewsletterDate, 
	Comments
) values (
	coalesce(@Date, cast('01 Jan 1900' as date)),
	(   select LocationId
		from Location
		where Name = coalesce(@Location, '')), 
	(   select SeriesId
		from Series
		where Name = coalesce(@Series, '')),
	coalesce(@Newsletter, cast('01 Jan 1900' as date)), 
	coalesce(@Comments, '')
)