using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundExplorers.Data {

    /// <summary>
    /// PerformanceJoined entity.
    /// </summary>
    internal class PerformanceJoined : Entity<PerformanceJoined> {

        #region Properties
        [KeyField]
        public DateTime Date { get; set; }
        [KeyField]
        public string Location { get; set; }
        [Field]
        public string Series { get; set; }
        [Field]
        public DateTime Newsletter { get; set; }
        [Field]
        public string Comments { get; set; }
        #endregion Properties

        #region Constructor
        /// <summary>
        /// Initialises a new instance of the 
        /// <see cref="PerformanceJoined"/> class.
        /// </summary>
        public PerformanceJoined() {
        }
        #endregion Constructor
    }//End of class
}//End of namespace
