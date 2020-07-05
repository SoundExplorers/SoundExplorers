using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SoundExplorers.Data {

    /// <summary>
    /// Code for accessing the database can be put in this class.
    /// </summary>
    internal static class DataHelper {

        #region Fields
        private static SqlHelper _sqlHelper;
        #endregion Fields

        #region Methods
        /// <summary>
        /// Initialises the database Connection.
        /// </summary>
        public static void Initialise() {
            _sqlHelper = new SqlHelper();
            //_sqlHelper.Timeout = 6000; // 10 minutes.
        }
        #endregion Methods
    }//End of class
}//End of namespace