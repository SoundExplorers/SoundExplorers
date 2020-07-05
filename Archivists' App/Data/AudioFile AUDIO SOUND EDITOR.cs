using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AudioSoundEditor;

namespace SoundExplorers.Data {

    /// <summary>
    /// An audio file.
    /// </summary>
    /// <remarks>
    /// Those metadata tags that we can usefully map to info on the database
    /// and that are also available with all audio file types
    /// are made available for update.
    /// </remarks>
    internal class AudioFile {

        #region Private Fields
        //private static AudioSoundEditor.AudioSoundEditor _soundEditor;
        #endregion Private Fields

        #region Metadata Tag Public Properties
        /// <summary>
        /// Gets or sets the file's Album metadata tag.
        /// An empty string if not specified.
        /// </summary>
        /// <remarks>
        /// Piece.Date, Piece.Location, Piece.SetNo.
        /// To save all the metadata tags to the file,
        /// invoke the <see cref="SaveTags"/> method.
        /// </remarks>
        public string Album { get; set; }

        /// <summary>
        /// Gets or sets the file's Artist metadata tag.
        /// An empty string if not specified.
        /// </summary>
        /// <remarks>
        /// Piece.Act if specified.
        /// Otherwise surnames from Credit.ArtistName
        /// delimited by /.
        /// To save all the metadata tags to the file,
        /// invoke the <see cref="SaveTags"/> method.
        /// </remarks>
        public string Artist { get; set; }

        /// <summary>
        /// Gets or sets the file's Comment metadata tag.
        /// An empty string if not specified.
        /// </summary>
        /// <remarks>
        /// Credits.
        /// To save all the metadata tags to the file,
        /// invoke the <see cref="SaveTags"/> method.
        /// </remarks>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the file's Track metadata tag.
        /// Zero string if not specified.
        /// </summary>
        /// <remarks>
        /// Piece.PieceNo padded with leading zero to two digits.
        /// To save all the metadata tags to the file,
        /// invoke the <see cref="SaveTags"/> method.
        /// </remarks>
        public byte Track { get; set; }

        /// <summary>
        /// Gets or sets the file's Title metadata tag.
        /// An empty string if not specified.
        /// </summary>
        /// <remarks>
        /// Piece.Title.
        /// To save all the metadata tags to the file,
        /// invoke the <see cref="SaveTags"/> method.
        /// </remarks>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the file's Year metadata tag.
        /// Zero if not specified.
        /// </summary>
        /// <remarks>
        /// Year of Piece.Date.
        /// To save all the metadata tags to the file,
        /// invoke the <see cref="SaveTags"/> method.
        /// </remarks>
        public short Year { get; set; }
        #endregion Metadata Tag Public Properties

        #region Other Public Properties
        /// <summary>
        /// Gets the file's path.
        /// </summary>
        public string Path { get; private set; }

        public static AudioSoundEditor.AudioSoundEditor SoundEditor { get; set; }
        #endregion Other Public Properties

        #region Metadata Tag Original Value Private Properties
        private string OldAlbum { get; set; }
        private string OldArtist { get; set; }
        private string OldComment { get; set; }
        private byte OldTrack { get; set; }
        private string OldTitle { get; set; }
        private short OldYear { get; set; }
        #endregion Metadata Tag Original Value Private Properties

        #region Other Private Properties
        /// <summary>
        /// The path of the audio file that was last
        /// analysed by the tag editor.
        /// </summary>
        private static string LastAnalyzedPath { get; set; }

        ///// <summary>
        ///// Gets the Audio Sound Editor for .Net component
        ///// used for editing tags.
        ///// </summary>
        ///// <remarks>
        ///// Initialising each instance of the Audio Sound Editor will pop up
        ///// a nag message while we are using an evaluation copy of it.
        ///// So, for now at least, we will share this single instance between
        ///// all AudioFiles.
        ///// </remarks>
        //private static AudioSoundEditor.AudioSoundEditor SoundEditor {
        //    get {
        //        if (_soundEditor == null) {
        //            _soundEditor = new AudioSoundEditor.AudioSoundEditor();
        //            // While we are using an evaluation copy of 
        //            // Audio Sound Editor for .Net,
        //            // this pops up a nag message box.
        //            _soundEditor.InitEditor();
        //        }
        //        return _soundEditor;
        //    }
        //}
        #endregion Other Private Properties

        #region Constructor
        /// <summary>
        /// Initialises a new instance of the 
        /// <see cref="AudioFile"/> class, 
        /// reading those metadata tags that we may update.
        /// </summary>
        /// <param name="path">
        /// The file's path.
        /// </param>
        /// <exception cref="ApplicationException">
        /// Audio tag editor error.
        /// </exception>
        public AudioFile(string path) {
            Path = path;
            //Editor = new AudioSoundEditor.AudioSoundEditor();
            AnalyseTags();
            LastAnalyzedPath = Path;
            Album = SoundEditor.TagsEditor.ALL_CommonFrameGet(
                enumTagFields.TAG_FIELD_ALBUM);
            OldAlbum = Album;
            Artist = SoundEditor.TagsEditor.ALL_CommonFrameGet(
                enumTagFields.TAG_FIELD_ARTIST);
            OldArtist = Artist;
            Comment = SoundEditor.TagsEditor.ALL_CommonFrameGet(
                enumTagFields.TAG_FIELD_COMMENT);
            OldComment = Comment;
            Title = SoundEditor.TagsEditor.ALL_CommonFrameGet(
                enumTagFields.TAG_FIELD_TITLE);
            OldTitle = Title;
            string trackString = SoundEditor.TagsEditor.ALL_CommonFrameGet(
                enumTagFields.TAG_FIELD_TRACK);
            byte track = 0;
            byte.TryParse(trackString, out track);
            Track = track;
            OldTrack = Track;
            string yearString = SoundEditor.TagsEditor.ALL_CommonFrameGet(
                enumTagFields.TAG_FIELD_YEAR);
            short year = 0;
            short.TryParse(yearString, out year);
            Year = year;
            OldYear = Year;
            //Debug.WriteLine("Title = " + title);
            //Debug.WriteLine("Artist = " + artist);
            //Debug.WriteLine("Album = " + album);
            //Debug.WriteLine("Track = " + track);
            //Debug.WriteLine("Year = " + year);
            //Debug.WriteLine("Comment = " + comment);
        }
        #endregion Constructor

        #region Public Methods
        /// <summary>
        /// If any of the tag properties have changed value
        /// since they were read from the file,
        /// saves all the metadata tags to the audio file
        /// with the values held in the corresponding properties.
        /// </summary>
        /// <exception cref="ApplicationException">
        /// Audio tag editor error.
        /// </exception>
        public void SaveTags() {
            if (Album == OldAlbum
            &&  Artist == OldArtist
            &&  Comment == OldComment
            &&  Title == OldTitle
            &&  Track == OldTrack
            &&  Year == OldYear) {
                // None of the tag values have changed,
                // so we don't need to update the file.
                return;
            }
            // As we are sharing a sound editor between audio files,
            // we need to re-analyse the tags before saving them
            // if another audo file has ben analysed since
            // this one was instantiated.
            if (LastAnalyzedPath != Path) {
                AnalyseTags();
            }
            SetTag(enumTagFields.TAG_FIELD_ALBUM, "Album", Album);
            SetTag(enumTagFields.TAG_FIELD_ARTIST, "Artist", Artist);
            SetTag(enumTagFields.TAG_FIELD_COMMENT, "Comment", Comment);
            SetTag(enumTagFields.TAG_FIELD_TITLE, "Title", Title);
            if (Track != 0) {
                SetTag(
                    enumTagFields.TAG_FIELD_TRACK, 
                    "Track", 
                    Track.ToString().PadLeft(2, '0'));
            } else {
                SetTag(
                    enumTagFields.TAG_FIELD_TRACK, 
                    "Track", 
                    string.Empty);
            }
            if (Year != 0) {
                SetTag(
                    enumTagFields.TAG_FIELD_YEAR,
                    "Year",
                    Year.ToString());
            } else {
                SetTag(
                    enumTagFields.TAG_FIELD_YEAR,
                    "Year",
                    string.Empty);
            }
            enumErrorCodes errorCode = SoundEditor.TagsEditor.ALL_SaveChanges();
            if (errorCode != enumErrorCodes.ERR_NOERROR) {
                throw new ApplicationException(
                    "Audio tag editor error " + errorCode.ToString()
                    + " occured on attempting to save the metadata tags to audio file \""
                    + Path + "\".");
            }
        }
        #endregion Public Methods

        #region Private Methods
        /// <summary>
        /// Analyses the audio file's metadata tags.
        /// </summary>
        /// <remarks>
        /// This has to be done before we can get the values
        /// of individual tags.
        /// </remarks>
        /// <exception cref="ApplicationException">
        /// Audio tag editor error.
        /// </exception>
        private void AnalyseTags() {
            enumTagAvailable availableTagTypes = 
                SoundEditor.TagsEditor.ALL_AnalyzeSoundFile(Path);
            if ((int)availableTagTypes < 0) {
                throw new ApplicationException(
                    "Audio tag editor error " + SoundEditor.LastError.ToString()
                    + " occured on attempting to analyze file \""
                    + Path + "\".");
            } else if (availableTagTypes == enumTagAvailable.TAG_FLAG_NONE) {
                throw new ApplicationException(
                    "\"" + Path + "\" is not recognised as an audio file.");
            }
        }

        /// <summary>
        /// Sets a metadata tag.
        /// </summary>
        /// <param name="tagField">
        /// The identifier of the tag field.
        /// </param>
        /// <param name="propertyName">
        /// The name of the property that holds the tag value.
        /// </param>
        /// <param name="value">
        /// The value currently in the specified property,
        /// formatted to a string if required.
        /// </param>
        /// <exception cref="ApplicationException">
        /// Audio tag editor error.
        /// </exception>
        private void SetTag(enumTagFields tagField, string propertyName, string value) {
            enumErrorCodes errorCode = SoundEditor.TagsEditor.ALL_CommonFrameSet(
                tagField, value);
            if (errorCode != enumErrorCodes.ERR_NOERROR) {
                throw new ApplicationException(
                    "Audio tag editor error " + errorCode.ToString()
                    + " occured on attempting to set file \""
                    + Path + "\"'s " + propertyName 
                    + " metadata tag to \"" + value + "\".");
            }
        }
        #endregion Private Methods
    }//End of class
}//End of namespace