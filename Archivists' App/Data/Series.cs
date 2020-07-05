using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundExplorers.Data {

    /// <summary>
    /// Series entity.
    /// </summary>
    internal class Series : PieceOwningEntity<Series> {

        #region Properties
        [PrimaryKeyField]
        public string SeriesId { get; set; }

        [UniqueKeyField]
        public string Name { get; set; }

        [Field]
        public string Comments { get; set; }
        #endregion Properties
    }//End of class
}//End of namespace
