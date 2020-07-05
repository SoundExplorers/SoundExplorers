using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundExplorers.Data {

    /// <summary>
    /// Act entity.
    /// </summary>
    internal class Act : PieceOwningMediaEntity<Act> {

        #region Public Field Properties
        [PrimaryKeyField]
        public string Name { get; set; }

        [Field]
        public string Comments { get; set; }
        #endregion Public Field Properties
    }//End of class
}//End of namespace
