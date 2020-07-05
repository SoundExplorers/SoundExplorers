using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace SoundExplorers.Data {

    /// <summary>
    /// The values actually or intended to be
    /// held in an audio file of 
    /// those audio metadata tags 
    /// that we can usefully map to info on the database
    /// and that are also available with all audio file types.
    /// </summary>
    internal class AudioFileTags : IAudioTags {

        #region Public Properties
        /// <summary>
        /// Gets or sets the audio file's Album metadata tag.
        /// </summary>
        public string Album {
            get {
                return Tags.Album;
            }
            set {
                Tags.Album = value;
            }
        }

        /// <summary>
        /// Gets or sets the audio file's Artist metadata tag.
        /// </summary>
        public string Artist {
            get {
                return Tags.FirstPerformer;
            }
            set {
                Tags.Performers = new string[] { value };
            }
        }

        /// <summary>
        /// Gets or sets the audio file's Comment metadata tag.
        /// </summary>
        public string Comment {
            get {
                return Tags.Comment;
            }
            set {
                Tags.Comment = value;
            }
        }

        /// <summary>
        /// Gets or sets the audio file's Track metadata tag.
        /// </summary>
        public uint Track {
            get {
                return Tags.Track;
            }
            set {
                Tags.Track = value;
            }
        }

        /// <summary>
        /// Gets or sets the audio file's Title metadata tag.
        /// </summary>
        public string Title {
            get {
                return Tags.Title;
            }
            set {
                Tags.Title = value;
            }
        }

        /// <summary>
        /// Gets or sets the audio file's Year metadata tag.
        /// </summary>
        public uint Year {
            get {
                return Tags.Year;
            }
            set {
                Tags.Year = value;
            }
        }
        #endregion Public Properties

        #region Private Properties
        private TagLib.Tag Tags { get; set; }
        #endregion Other Private Properties

        #region Constructor
        /// <summary>
        /// Initialises a new instance of the 
        /// <see cref="AudioFileTags"/> class.
        /// </summary>
        /// <param name="tags">
        /// The audio file's metadata tags.
        /// </param>
        public AudioFileTags(TagLib.Tag tags) {
            Tags = tags;
        }
        #endregion Constructor
    }//End of class
}//End of namespace