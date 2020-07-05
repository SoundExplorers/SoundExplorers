delete from Credit
where PerformanceDate = @Date
and LocationId in (
    select LocationId
    from Location
    where Location.Name = @Location)
and SetNo = @Set
and PieceNo = @Piece
and ArtistName = @Artist
and RoleId  in (
    select RoleId
    from Role
    where Role.Name = @Role)