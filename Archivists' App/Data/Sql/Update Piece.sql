update Piece set
	PerformanceDate = @Date, 
	LocationId = (
		select LocationId
		from Location
		where Name = @Location), 
	SetNo = @Set,
	PieceNo = @PieceNo,
	Title = coalesce(@Title, ''), 
	AudioPath = coalesce(@AudioPath, ''), 
	VideoPath = coalesce(@VideoPath, ''), 
	Comments = coalesce(@Comments, '')
where PerformanceDate = @OLD_Date
and LocationId = (
	select LocationId
	from Location
	where Name = @OLD_Location)
and SetNo = @OLD_Set
and PieceNo = @OLD_PieceNo