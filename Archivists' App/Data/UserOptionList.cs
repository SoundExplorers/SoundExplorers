using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SoundExplorers.Data {

    /// <summary>
    /// A list of UserOptions.
    /// </summary>
    internal class UserOptionList : EntityList<UserOption> {

        #region Public Properties
        /// <summary>
        /// An indexer that returns 
        /// the UserOption at the specified index in the list.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the UserOption to get.
        /// </param>
        /// <returns>
        /// The UserOption at the specified index in the list.
        /// </returns>
        public new UserOption this[int index] {
            get {
                return base[index] as UserOption;
            }
        }
        #endregion Public Properties

        #region Constructors
        /// <summary>
        /// Initialises a new instance of the <see cref="UserOptionList"/> class,
        /// populating its list 
        /// with all the UserOption records on the database.
        /// </summary>
        public UserOptionList() {
        }
        #endregion Constructors
    }//End of class
}//End of namespace