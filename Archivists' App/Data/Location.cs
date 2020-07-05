using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundExplorers.Data {

    /// <summary>
    /// Location entity.
    /// </summary>
    internal class Location : PieceOwningMediaEntity<Location> {

        #region Public Field Properties
        [PrimaryKeyField]
        public string LocationId { get; set; }

        [UniqueKeyField]
        public string Name { get; set; }

        [Field]
        public string Comments { get; set; }
        #endregion Public Field roperties
    }//End of class
}//End of namespace
