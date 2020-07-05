if exists (
    select * from sysobjects
    where id = object_id('#TableName')
    and objectproperty(id, 'IsUserTable') = 1)
  drop table #TableName