using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace SoundExplorers.Data {

    /// <summary>
    /// An audio file.
    /// </summary>
    /// <remarks>
    /// Those metadata tags that we can usefully map to info on the database
    /// and that are also available with all audio file types
    /// are made available for update.
    /// </remarks>
    internal class AudioFile : IDisposable {

        #region Public Properties
        /// <summary>
        /// Gets whether the file exists.
        /// </summary>
        public virtual bool Exists {
            get {
                return
                    !string.IsNullOrEmpty(Path)
                    && File.Exists(Path);
            }
        }

        /// <summary>
        /// Gets the file's path.
        /// </summary>
        public virtual string Path { get; private set; }

        /// <summary>
        /// Gets the values actually or intended to be
        /// held in the audio file of 
        /// those audio metadata tags 
        /// that we can usefully map to info on the database
        /// and that are also available with all audio file types.
        /// </summary>
        /// <remarks>
        /// To save all the metadata tags to the audio file,
        /// invoke the <see cref="SaveTags"/> method.
        /// </remarks>
        public virtual AudioFileTags Tags { get; private set; }
        #endregion Public Properties

        #region Private Properties
        private AudioTags OldTags { get; set; }
        private TagLib.File TagFile { get; set; }
        #endregion Private Properties

        #region Constructor
        /// <summary>
        /// Initialises a new instance of the 
        /// <see cref="AudioFile"/> class, 
        /// reading those metadata tags that we may update.
        /// </summary>
        /// <param name="path">
        /// The file's path.
        /// </param>
        /// <remarks>
        /// The file must exist.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// The path argument is null.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// The file specified by the path argument cannot be found.
        /// </exception>
        public AudioFile(string path) {
            Path = path;
            if (Path == null) {
                throw new ArgumentNullException(
                    "path", "The path argument is null.");
            }
            if (!Exists) {
                throw new FileNotFoundException(
                    "File \"" + Path + "\" cannot be found.", Path);
            }
            TagFile = TagLib.File.Create(Path);
            Tags = new AudioFileTags(TagFile.Tag);
            OldTags = new AudioTags();
            OldTags.Album = Tags.Album;
            OldTags.Artist = Tags.Artist;
            OldTags.Comment = Tags.Comment;
            OldTags.Title = Tags.Title;
            OldTags.Track = Tags.Track;
            OldTags.Year = Tags.Year;
        }
        #endregion Constructor

        #region Public Methods
        /// <summary>
        /// If any of the tag properties have changed value
        /// since they were read from the file,
        /// saves all the metadata tags to the audio file
        /// with the values held in the corresponding properties.
        /// </summary>
        /// <returns>
        /// Whether there were any actual changes and they
        /// were therefore saved to the file.
        /// </returns>
        /// <exception cref="ApplicationException">
        /// An error occured while attempting to save the tags.
        /// </exception>
        public virtual bool SaveTags() {
            if (!Exists) {
                // This should never happen,
                // as PieceAudioTags.SaveToFile won't invoke this method
                // if the file does not exit.
                // So it does not need to be trapped.
                throw new FileNotFoundException(
                    "File \"" + Path + "\" cannot be found.", Path);
            }
            if (Tags.Album == OldTags.Album
            &&  Tags.Artist == OldTags.Artist
            &&  Tags.Comment == OldTags.Comment
            &&  Tags.Title == OldTags.Title
            &&  Tags.Track == OldTags.Track
            &&  Tags.Year == OldTags.Year) {
                // None of the tag values have changed,
                // so we don't need to update the file.
                return false;
            }
            try {
                TagFile.Save();
            } catch (Exception ex) {
                // I have no idea what type of Exception 
                // TagLib.File.Save might throw.
                throw new ApplicationException(
                    "The following error occured while attempting to save audio tags to file \"" 
                    + Path + "\":" + Environment.NewLine
                    + ex.Message, ex);
            }
            return true;
        }
        #endregion Public Methods

        #region IDisposable Members
        public void Dispose() {
            TagFile.Dispose();
        }
        #endregion
    }//End of class
}//End of namespace