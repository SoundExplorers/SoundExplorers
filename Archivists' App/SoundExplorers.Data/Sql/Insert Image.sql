insert into Image (
	ImageId, 
	Path, 
	Title, 
	PerformanceDate, 
	LocationId, 
	Comments
) values (
	@ImageId,
--	nextval('image_imageid_seq'),
	coalesce(@Path, ''), 
	coalesce(@Title, ''), 
	coalesce(@Date, cast('01 Jan 1900' as date)),
	(   select LocationId
		from Location
		where Name = coalesce(@Location, '')), 
	coalesce(@Comments, '')
)