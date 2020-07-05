Sound Explorers Audio Archive is a project under development whose aim is to facilitate the archiving of audio recordings, together with associated documentation and, where available, videos.

Its intended initial use is to archive the recordings that have been accumulated over several years by New Zealand's Sound and Light Exploration Society ('Sound Explorers') http://www.soundexplorers.co.nz/.  The recordings are mostly of musical performances presented by the society at various locations, mostly its main venue, which is currently the Pyramid Club in Wellington.  

However, though development is for Sound Explorers in the first instance, the nature of the data would lend itself to the modification of the software to allow other organisations or individuals to archive similar collections of recordings, probably mostly of music.

ARCHITECTURE

Archivists will collate audio and, when available, video recordings of performances and upload them to appropriate web repositories.

The archivists will use a Windows desktop application to document the recordings in a relational database that will contain web links both to the recordings and to related images and archived documents such as newsletters.  So the database may be considered to contain 'metadata'.

A web application, to be developed later, will make the media, metadata and other documents available to the public.

The archivists' application, development of which is well underway, is written in C#, with a Windows Forms graphical user interface.  The provisional storage strategy is for the archivists' application to save the data to a PC-based Postgresql database and provide a facility to export the data to the web application's data store.  However, once we are closer to starting development on the web application, we can consider a switch to having the archivists' application update the web application's data store directly.

DATA ANALYSIS

The main entities that need to be stored on the relational database are expected to be: Act (usually a music ensemble); Artist (usually a musician); Location (usually a venue); Event (usually a performance but could be a field recording, rehearsal or interview); Newsletter (links to a newsletter that documents one or more events);  Piece (of which a recording is available); Role (usually the instrument a musician played on a piece), Series (such as a festival), Set.

We will also look at including links to other related documents and images such as fliers, posters and photos.

COLLABORATION

I'm not looking for collaborators at this stage.  However, for expressions of interest, please do get in touch.

CONTACT

For further information, please contact me via this link: https://simon.ororke.net/contact.

Simon O'Rorke
