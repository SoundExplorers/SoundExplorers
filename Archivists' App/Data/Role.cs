using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundExplorers.Data {

    /// <summary>
    /// Role entity.
    /// </summary>
    internal class Role : Entity<Role> {

        #region Properties
        [PrimaryKeyField]
        public string RoleId { get; set; }

        [UniqueKeyField]
        public string Name { get; set; }
        #endregion Properties
    }//End of class
}//End of namespace
