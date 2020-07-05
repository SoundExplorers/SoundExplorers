using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SoundExplorers.Data {

    /// <summary>
    /// Interface for 
    /// an Entity from whose data media file metadata tags may need to be updated.
    /// </summary>
    internal interface IMediaEntity : IEntity {

        #region Methods
        /// <summary>
        /// Updates the metadata tags of any audio files
        /// associated with the entity with appropriate data from the database.
        /// </summary>
        /// <returns>
        /// A message describing the update or,
        /// if no audio files have been updated,
        /// an empty string.
        /// </returns>
        string UpdateTags();
        #endregion Methods
    }//End of class
}//End of namespace