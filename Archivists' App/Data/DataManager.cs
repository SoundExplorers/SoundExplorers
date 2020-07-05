using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundExplorers.Data {

    internal static class DataManager {

        #region Private Fields
        private static List<IEntityList> _entityLists;
        #endregion Private Fields

        #region Public Properties
        public static List<IEntityList> EntityLists {
            get {
                if (_entityLists == null) {
                    _entityLists = new List<IEntityList>();
                }
                return _entityLists;
            }
        }
        #endregion Public Properties
    }//End of class
}//End of namespace