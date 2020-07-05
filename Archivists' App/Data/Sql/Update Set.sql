update "set" set
	PerformanceDate = @Date, 
	LocationId = (
		select LocationId
		from Location
		where Name = @Location), 
	SetNo = @SetNo,
	ActName = coalesce(@Act, ''), 
	Comments = coalesce(@Comments, '')
where PerformanceDate = @OLD_Date
and LocationId = (
	select LocationId
	from Location
	where Name = @OLD_Location)
and SetNo = @OLD_SetNo