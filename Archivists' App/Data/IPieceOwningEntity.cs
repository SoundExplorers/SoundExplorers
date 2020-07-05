using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SoundExplorers.Data {

    /// <summary>
    /// Interface for 
    /// an Entity that can have mutliple Pieces associated with it.
    /// </summary>
    internal interface IPieceOwningEntity : IEntity {

        #region Properties
        /// <summary>
        /// Gets or sets the entity's Pieces.
        /// Will be fetched from the database if not previously set when got.
        /// </summary>
        PieceList Pieces { get; set; }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Fetches the entity's Pieces from the database.
        /// </summary>
        /// <returns>
        /// The entitye's Pieces.
        /// </returns>
        PieceList FetchPieces();
        #endregion Methods
    }//End of class
}//End of namespace