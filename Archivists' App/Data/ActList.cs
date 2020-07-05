using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SoundExplorers.Data {

    /// <summary>
    /// A list of Acts.
    /// </summary>
    internal class ActList : EntityList<Act> {

        #region Public Properties
        /// <summary>
        /// An indexer that returns 
        /// the Act at the specified index in the list.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the Act to get.
        /// </param>
        /// <returns>
        /// The Act at the specified index in the list.
        /// </returns>
        public new Act this[int index] {
            get {
                return base[index] as Act;
            }
        }
        #endregion Public Properties

        #region Constructors
        /// <summary>
        /// Initialises a new instance of the <see cref="ActList"/> class,
        /// populating its list 
        /// with all the Act records on the database.
        /// </summary>
        public ActList() {
        }
        #endregion Constructors
    }//End of class
}//End of namespace