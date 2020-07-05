using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SoundExplorers.Data {

    /// <summary>
    /// A list of Serieses.
    /// </summary>
    internal class SeriesList : EntityList<Series> {

        #region Public Properties
        /// <summary>
        /// An indexer that returns 
        /// the Series at the specified index in the list.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the Series to get.
        /// </param>
        /// <returns>
        /// The Series at the specified index in the list.
        /// </returns>
        public new Series this[int index] {
            get {
                return base[index] as Series;
            }
        }
        #endregion Public Properties

        #region Constructors
        /// <summary>
        /// Initialises a new instance of the <see cref="SeriesList"/> class,
        /// populating it with all the Series records on the database.
        /// </summary>
        public SeriesList() {
        }
        #endregion Constructors
    }//End of class
}//End of namespace