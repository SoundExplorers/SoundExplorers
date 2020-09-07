insert into Credit (
	PerformanceDate, 
	LocationId, 
	SetNo,
	PieceNo,
	ArtistName, 
	RoleId
) values (
	coalesce(@Date, cast('01 Jan 1900' as date)),
	(   select LocationId
		from Location
		where Name = coalesce(@Location, '')), 
	@Set,
	@Piece,
	coalesce(@Artist, ''), 
	(   select RoleId
		from Role
		where Name = coalesce(@Role, ''))
)