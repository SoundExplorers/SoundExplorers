insert into "set" (
	PerformanceDate, 
	LocationId, 
	SetNo,
	ActName, 
	Comments
) values (
	coalesce(@Date, cast('01 Jan 1900' as date)),
	(   select LocationId
		from Location
		where Name = coalesce(@Location, '')), 
	@SetNo,
	coalesce(@Act, ''), 
	coalesce(@Comments, '')
)