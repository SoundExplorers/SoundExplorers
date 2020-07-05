using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundExplorers.Data {

    /// <summary>
    /// An Entity that can have mutliple Pieces associated with it.
    /// </summary>
    internal abstract class PieceOwningEntity<T> : Entity<T>, IPieceOwningEntity
        where T : PieceOwningEntity<T> {

        #region Private Fields
        private PieceList _pieces;
        #endregion Private Fields

        #region Public Properties
        /// <summary>
        /// Gets or sets the entity's Pieces.
        /// Will be fetched from the database if not previously set when got.
        /// </summary>
        public virtual PieceList Pieces {
            get {
                if (_pieces == null) {
                    _pieces = FetchPieces();
                }
                return _pieces;
            }
            set {
                _pieces = value;
            }
        }
        #endregion Public Properties

        #region Public Methods
        /// <summary>
        /// Fetches the entity's Pieces from the database.
        /// </summary>
        /// <returns>
        /// The entity's Pieces.
        /// </returns>
        //public abstract PieceList FetchPieces();
        public PieceList FetchPieces() {
            return new PieceList(
                parentListType: null, owner: this);
        }
        #endregion Public Methods
    }//End of class
}//End of namespace
