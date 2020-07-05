using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundExplorers.Data {

    /// <summary>
    /// Performance entity.
    /// </summary>
    internal class Performance : PieceOwningMediaEntity<Performance> {

        #region Public Field Properties
        [PrimaryKeyField]
        public DateTime Date { get; set; }

        [PrimaryKeyField]
        [ReferencedField("Name")]
        public string Location { get; set; }

        [ReferencedField("Name")]
        public string Series { get; set; }

        [ReferencedField("Date")]
        public DateTime Newsletter { get; set; }

        [Field]
        public string Comments { get; set; }
        #endregion Public Field Properties

        #region Constructor
        /// <summary>
        /// Initialises a new instance of the 
        /// <see cref="Performance"/> class.
        /// </summary>
        public Performance() {
        }
        #endregion Constructor

        #region Public Methods
        /// <summary>
        /// Fetches the Performance's Newsletter (i.e. not just the Newsletter date)
        /// from the database.
        /// </summary>
        /// <returns>
        /// The Performance's Newsletter.
        /// </returns>
        public virtual Newsletter FetchNewsletter() {
            var newsletter = new Newsletter();
            newsletter.Date = Newsletter;
            newsletter.Fetch();
            return newsletter;
        }
        #endregion Public Methods
    }//End of class
}//End of namespace
