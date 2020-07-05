using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SoundExplorers.Data {

    /// <summary>
    /// A list of Locations.
    /// </summary>
    internal class LocationList : EntityList<Location> {

        #region Public Properties
        /// <summary>
        /// An indexer that returns 
        /// the Location at the specified index in the list.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the Location to get.
        /// </param>
        /// <returns>
        /// The Location at the specified index in the list.
        /// </returns>
        public new Location this[int index] {
            get {
                return base[index] as Location;
            }
        }
        #endregion Public Properties

        #region Constructors
        /// <summary>
        /// Initialises a new instance of the <see cref="LocationList"/> class,
        /// populating its list 
        /// with all the Location records on the database.
        /// </summary>
        public LocationList() {
        }
        #endregion Constructors
    }//End of class
}//End of namespace