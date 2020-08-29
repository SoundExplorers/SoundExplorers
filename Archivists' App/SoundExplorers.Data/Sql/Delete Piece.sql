delete from Piece
where PerformanceDate = @Date
and LocationId in (
    select LocationId
    from Location
    where Location.Name = @Location)
and SetNo = @Set
and PieceNo = @PieceNo