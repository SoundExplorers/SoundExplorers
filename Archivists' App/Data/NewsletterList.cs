using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SoundExplorers.Data {

    /// <summary>
    /// A list of Newsletters.
    /// </summary>
    internal class NewsletterList : EntityList<Newsletter> {

        #region Public Properties
        /// <summary>
        /// An indexer that returns 
        /// the Newsletter at the specified index in the list.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the Newsletter to get.
        /// </param>
        /// <returns>
        /// The Newsletter at the specified index in the list.
        /// </returns>
        public new Newsletter this[int index] {
            get {
                return base[index] as Newsletter;
            }
        }
        #endregion Public Properties

        #region Constructors
        /// <summary>
        /// Initialises a new instance of the <see cref="NewsletterList"/> class,
        /// populating its list 
        /// with all the Newsletter records on the database.
        /// </summary>
        public NewsletterList() {
        }
        #endregion Constructors
    }//End of class
}//End of namespace