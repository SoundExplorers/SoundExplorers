using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SoundExplorers.Data {

    /// <summary>
    /// A list of Roles.
    /// </summary>
    internal class RoleList : EntityList<Role> {

        #region Public Properties
        /// <summary>
        /// An indexer that returns 
        /// the Role at the specified index in the list.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the Role to get.
        /// </param>
        /// <returns>
        /// The Role at the specified index in the list.
        /// </returns>
        public new Role this[int index] {
            get {
                return base[index] as Role;
            }
        }
        #endregion Public Properties

        #region Constructors
        /// <summary>
        /// Initialises a new instance of the <see cref="RoleList"/> class,
        /// populating its list 
        /// with all the Role records on the database.
        /// </summary>
        public RoleList() {
        }
        #endregion Constructors
    }//End of class
}//End of namespace