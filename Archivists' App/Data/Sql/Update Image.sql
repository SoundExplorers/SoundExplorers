update Image set
	Path = coalesce(@Path, ''), 
	Title = coalesce(@Title, ''), 
	PerformanceDate = @Date, 
	LocationId = (
		select LocationId
		from Location
		where Name = @Location), 
	Comments = coalesce(@Comments, '')
where ImageId = @OLD_ImageId