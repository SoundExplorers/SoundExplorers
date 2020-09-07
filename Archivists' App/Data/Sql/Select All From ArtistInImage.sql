SELECT
	ArtistInImage.ImageId, 
	ArtistInImage.ArtistName as Artist, 
	Artist.Forename,
	Artist.Surname
FROM ArtistInImage
INNER JOIN Artist
	ON Artist.ArtistName = ArtistInImage.ArtistName
ORDER BY
	ArtistInImage.ImageId, 
	ArtistInImage.ArtistName
