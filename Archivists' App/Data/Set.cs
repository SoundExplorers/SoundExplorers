using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundExplorers.Data {

    /// <summary>
    /// Set entity.
    /// </summary>
    internal class Set : PieceOwningMediaEntity<Set> {

        #region Public Field Properties
        [PrimaryKeyField]
        public DateTime Date { get; set; }

        [PrimaryKeyField]
        [ReferencedField("Name")]
        public string Location { get; set; }

        [PrimaryKeyField]
        public int SetNo { get; set; }

        [ReferencedField("Name")]
        public string Act { get; set; }

        [ReferencedField("Date")]
        public DateTime Newsletter { get; set; }

        [Field]
        public string Comments { get; set; }
        #endregion Public Field Properties

        #region Constructor
        /// <summary>
        /// Initialises a new instance of the 
        /// <see cref="Set"/> class.
        /// </summary>
        public Set() {
        }
        #endregion Constructor

        #region Public Methods
        /// <summary>
        /// Fetches the Set's Newsletter (i.e. not just the Newsletter date)
        /// from the database.
        /// </summary>
        /// <returns>
        /// The Set's Newsletter.
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
