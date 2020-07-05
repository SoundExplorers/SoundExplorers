using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundExplorers.Data {

    /// <summary>
    /// UserOption entity.
    /// </summary>
    internal class UserOption : Entity<UserOption> {

        #region Properties
        [PrimaryKeyField]
        public string UserId { get; set; }

        [PrimaryKeyField]
        public string OptionName { get; set; }

        [Field]
        public string OptionValue { get; set; }
        #endregion Properties
    }//End of class
}//End of namespace
