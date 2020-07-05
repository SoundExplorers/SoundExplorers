using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace SoundExplorers.Data {

    /// <summary>
    /// Piece entity.
    /// </summary>
    internal class Piece : Entity<Piece>, IMediaEntity {

        #region Private Fields
        private CreditList _credits;
        private static DirectoryInfo _defaultAudioFolder;
        private static Option _defaultAudioFolderOption;
        private static DirectoryInfo _defaultVideoFolder;
        private static Option _defaultVideoFolderOption;
        private Piece _original;
        #endregion Private Fields

        #region Public Field Properties
        [PrimaryKeyField]
        public DateTime Date { get; set; }

        [PrimaryKeyField]
        [ReferencedField("Name")]
        public string Location { get; set; }

        [PrimaryKeyField]
        [ReferencedField("SetNo")]
        public int Set { get; set; }

        [PrimaryKeyField]
        public int PieceNo { get; set; }

        [ReferencedField("Name")]
        public string Act { get; set; }

        [ReferencedField("Date")]
        public DateTime Newsletter { get; set; }

        [Field]
        public string Title { get; set; }

        [Field]
        public string AudioPath { get; set; }

        [Field]
        public string VideoPath { get; set; }

        [Field]
        public string Comments { get; set; }
        #endregion Public Field Properties

        #region Other Public Properties
        /// <summary>
        /// Gets whether the file, if any, specified by
        /// the <see cref="AudioPath"/> field propery exists.
        /// False if a path is not specified or the file does not exist.
        /// </summary>
        public virtual bool AudioFileExists {
            get {
                return
                    !string.IsNullOrEmpty(AudioPath)
                    && File.Exists(AudioPath);
            }
        }

        /// <summary>
        /// Gets the values generated from the data of the Piece and its Credits 
        /// for those audio metadata tags 
        /// that are available all audio file types
        /// and may or may not be saved to an actual audio file.
        /// </summary>
        public PieceAudioTags AudioTags { get; private set; }

        /// <summary>
        /// Gets or sets the Piece's child Credits.
        /// Will be fetched from the database if not previously set when got.
        /// Will be sorted into Surname, Forename, Role order.
        /// </summary>
        public virtual CreditList Credits {
            get {
                if (_credits == null) {
                    _credits = FetchCredits();
                }
                return _credits;
            }
            set {
                _credits = value;
            }
        }

        /// <summary>
        /// Gets or sets a default folder to be used,
        /// if the folder of the path (if any) currently in 
        /// the <see cref="AudioPath"/> property
        /// is not specified or does not exist,
        /// as the initial folder for the Open dialogue that may be
        /// shown to select an audio file to update the path in 
        /// the <see cref="AudioPath"/> property.
        /// </summary>
        public static DirectoryInfo DefaultAudioFolder {
            get {
                if (_defaultAudioFolder == null) {
                    if (DefaultAudioFolderOption.StringValue != string.Empty) {
                        _defaultAudioFolder = new DirectoryInfo(DefaultAudioFolderOption.StringValue);
                    }
                    if (_defaultAudioFolder == null
                    || !_defaultAudioFolder.Exists) {
                        _defaultAudioFolder = new DirectoryInfo(
                            Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
                    }
                }
                return _defaultAudioFolder;
            }
            set {
                _defaultAudioFolder = value;
                DefaultAudioFolderOption.StringValue = value.FullName;
            }
        }

        /// <summary>
        /// Gets or sets a default folder to be used,
        /// if the folder of the path (if any) currently in 
        /// the <see cref="VideoPath"/> property
        /// is not specified or does not exist,
        /// as the initial folder for the Open dialogue that may be
        /// shown to select a video file to update the path in 
        /// the <see cref="VideoPath"/> property.
        /// </summary>
        public static DirectoryInfo DefaultVideoFolder {
            get {
                if (_defaultVideoFolder == null) {
                    if (DefaultVideoFolderOption.StringValue != string.Empty) {
                        _defaultVideoFolder = new DirectoryInfo(DefaultVideoFolderOption.StringValue);
                    }
                    if (_defaultVideoFolder == null
                    || !_defaultVideoFolder.Exists) {
                        _defaultVideoFolder = new DirectoryInfo(
                            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
                    }
                }
                return _defaultVideoFolder;
            }
            set {
                _defaultVideoFolder = value;
                DefaultVideoFolderOption.StringValue = value.FullName;
            }
        }

        /// <summary>
        /// Gets or sets a version of the Piece and its child Credits
        /// with field property values as at
        /// when the corresponding grid row was entered,
        /// i.e. as at before any changed effected through the grid.
        /// </summary>
        public virtual Piece Original {
            get {
                if (_original == null) {
                    _original = this;
                }
                return _original;
            }
            set {
                _original = value;
            }
        }

        /// <summary>
        /// Gets whether the file, if any, specified by
        /// the <see cref="VideoPath"/> field propery exists.
        /// False if a path is not specified or the file does not exist.
        /// </summary>
        public virtual bool VideoFileExists {
            get {
                return
                    !string.IsNullOrEmpty(VideoPath)
                    && File.Exists(VideoPath);
            }
        }
        #endregion Other Public Properties

        #region Private Properties
        private static Option DefaultAudioFolderOption {
            get {
                if (_defaultAudioFolderOption == null) {
                    _defaultAudioFolderOption = new Option(
                       "Piece.DefaultAudioFolder");
                }
                return _defaultAudioFolderOption;
            }
        }

        private static Option DefaultVideoFolderOption {
            get {
                if (_defaultVideoFolderOption == null) {
                    _defaultVideoFolderOption = new Option(
                       "Piece.DefaultVideoFolder");
                }
                return _defaultVideoFolderOption;
            }
        }
        #endregion Private Properties

        #region Constructor
        /// <summary>
        /// Initialises a new instance of the 
        /// <see cref="Piece"/> class.
        /// </summary>
        public Piece() {
            AudioTags = new PieceAudioTags(this);
        }
        #endregion Constructor

        #region Public Methods
        /// <summary>
        /// Fetches the Piece's child Credits from the database.
        /// </summary>
        /// <returns>
        /// The Piece's child Credits.
        /// </returns>
        public virtual CreditList FetchCredits() {
            return new CreditList(
                parentListType: null, piece: this);
        }

        /// <summary>
        /// Fetches the Piece's Newsletter (i.e. not just the Newsletter date)
        /// from the database.
        /// </summary>
        /// <returns>
        /// The Piece's Newsletter.
        /// </returns>
        public virtual Newsletter FetchNewsletter() {
            var newsletter = new Newsletter();
            newsletter.Date = Newsletter;
            newsletter.Fetch();
            return newsletter;
        }

        /// <summary>
        /// Updates the metadata tags of any audio file
        /// associated with the Piece with appropriate data from the database.
        /// </summary>
        /// <returns>
        /// A message describing the update or,
        /// if no audio file has been updated,
        /// an empty string.
        /// </returns>
        /// <exception cref="ApplicationException">
        /// An error occured while attempting to save the tags.
        /// </exception>
        public string UpdateTags() {
            if (AudioTags.SaveToFile()) {
                return 
                    "Tags saved to audio file \""
                    + Path.GetFileName(AudioPath) + "\"";
            } else {
                return string.Empty;
            }
        }
        #endregion Public Methods
    }//End of class
}//End of namespace
