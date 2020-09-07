insert into Artist (
	ArtistName, 
	Forename,
	Surname
) values (
	ltrim(rtrim(coalesce(@Forename, '') || ' ' || coalesce(@Surname, ''))),
	coalesce(@Forename, ''), 
	coalesce(@Surname, '')
)