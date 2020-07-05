update Credit set
	PerformanceDate = @Date, 
	LocationId = (
		select LocationId
		from Location
		where Name = @Location), 
	SetNo = @Set,
	PieceNo = @Piece,
	ArtistName = @Artist, 
	RoleId = (   
		select RoleId
		from Role
		where Name = @Role)
where PerformanceDate = @OLD_Date
and LocationId = (
	select LocationId
	from Location
	where Name = @OLD_Location)
and SetNo = @OLD_Set
and PieceNo = @OLD_Piece
and ArtistName = @OLD_Artist
and RoleId = (
	select RoleId
	from Role
	where Name = @OLD_Role)