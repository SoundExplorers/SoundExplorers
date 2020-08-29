update Artist set
	ArtistName = ltrim(rtrim(coalesce(@Forename, '') || ' ' || coalesce(@Surname, ''))), 
	Forename = coalesce(@Forename, ''),
	Surname = coalesce(@Surname, '')
where ArtistName = @OLD_Name