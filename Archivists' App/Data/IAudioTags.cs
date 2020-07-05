using System;

namespace SoundExplorers.Data {

    /// <summary>
    /// The audio metadata tags 
    /// that we can usefully map to info on the database
    /// and that are also available with all audio file types.
    /// </summary>
    internal interface IAudioTags {

        #region Properties
        /// <summary>
        /// Gets the Album audio metadata tag.
        /// </summary>
        string Album { get; }

        /// <summary>
        /// Gets the Artist audio metadata tag.
        /// </summary>
        string Artist { get; }

        /// <summary>
        /// Gets the Comment audio metadata tag.
        /// </summary>
        string Comment { get; }

        /// <summary>
        /// Gets the Track audio metadata tag.
        /// </summary>
        uint Track { get; }

        /// <summary>
        /// Gets the Title audio metadata tag.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets the Year audio metadata tag.
        /// </summary>
        uint Year { get; }
        #endregion Properties
    }//End of interface
}//End of namespace