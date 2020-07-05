using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SoundExplorers.Data {

    /// <summary>
    /// A list of Artists.
    /// </summary>
    internal class ArtistList : EntityList<Artist> {

        #region Public Properties
        /// <summary>
        /// An indexer that returns 
        /// the Artist at the specified index in the list.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the Artist to get.
        /// </param>
        /// <returns>
        /// The Artist at the specified index in the list.
        /// </returns>
        public new Artist this[int index] {
            get {
                return base[index] as Artist;
            }
        }
        #endregion Public Properties

        #region Constructors
        /// <summary>
        /// Initialises a new instance of the <see cref="ArtistList"/> class,
        /// populating its list 
        /// with all the Artist records on the database.
        /// </summary>
        public ArtistList() {
        }
        #endregion Constructors
    }//End of class
}//End of namespace