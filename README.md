# Sound Explorers Audio Archive README

**Sound Explorers Audio Archive** is an open source Windows application that facilitates the archiving of audio recordings, together with associated documentation and videos.  Support for associated images will be added in a future release.

Its initial use is to archive the recordings that have been accumulated over several years by New Zealand's [Sound and Light Exploration Society](http://www.soundexplorers.co.nz/) ('**Sound Explorers**').  The recordings are mostly of musical performances presented by Sound Explorers at various locations, mostly its main venue, which is currently the [Pyramid Club](https://www.pyramidclub.org.nz) in Wellington .

However, though development is for Sound Explorers in the first instance, the nature of the data would lend itself to use by other organisations or individuals to archive similar collections of recordings, probably mostly of music.

### ARCHITECTURE

Archivists collate audio and, when available, video recordings of performances and upload them to appropriate web repositories.

The archivists use a Windows desktop application to document the recordings in a database that contains web links both to the recordings and to related archived documents, currently just newsletters.  So the database may be considered to contain 'metadata'.

A separate web application, to be for developed for Sound Explorers later, will make the media, metadata and other documents available to the public.  So the archivists' application will include a facility to export the data to the web application's data store.

The archivists' application is written in C# for .Net 5.0, with a Windows Forms graphical user interface.  Data is saved to a [VelocityDB](https://velocitydb.com/) object-oriented database, which has a very fast and versatile architecture.  As .Net 5.0 and VelocityDB support multiple operating systems, there is potential for enhancing the application to run on operating systems other than Windows, by implementing another type of user interface.

The application has a release site at SourceForge: https://sourceforge.net/projects/sound-explorers-audio-archive/.

### DATA MODEL

The entities currently stored on the  database are Act (usually a music ensemble); Artist (usually a musician); Location (usually a venue); Event (usually a performance but could be a field recording, rehearsal or interview); Event Type (such as 'Performance' and 'Rehearsal'); Genre;  Newsletter (links to a newsletter that documents one or more events);  Piece (of which a recording is available); Role (usually the instrument a musician played on a piece), Series (such as a festival), Set.

For details, see Documentation/Entity Relationship Diagram.pdf.

We are also looking at including links to other related documents and images such as fliers, posters and photos.

### COLLABORATION

I'm not looking for development collaborators at this stage, as I need to focus on implementing further requirements specifically for Sound Explorers.  However, for expressions of interest, please do get in touch.

### CONTACT

For further information, please contact me via this link: https://simon.ororke.net/contact.

**Simon O'Rorke**