delete from Performance
where PerformanceDate = @Date
and LocationId in (
    select LocationId
    from Location
    where Location.Name = @Location)