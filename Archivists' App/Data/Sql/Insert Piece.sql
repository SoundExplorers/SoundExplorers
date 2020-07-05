insert into Piece (
	PerformanceDate, 
	LocationId, 
	SetNo,
	PieceNo,
	Title, 
	AudioPath, 
	VideoPath, 
	Comments
) values (
	coalesce(@Date, cast('01 Jan 1900' as date)),
	(   select LocationId
		from Location
		where Name = coalesce(@Location, '')), 
	@Set,
	@PieceNo,
	coalesce(@Title, ''), 
	coalesce(@AudioPath, ''), 
	coalesce(@VideoPath, ''), 
	coalesce(@Comments, '')
)