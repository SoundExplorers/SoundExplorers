using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundExplorers.Data {

    /// <summary>
    /// PerformanceSimple entity.
    /// </summary>
    internal class PerformanceSimple : Entity<PerformanceSimple> {

        #region Properties
        [KeyField]
        public DateTime Date { get; set; }
        [KeyField]
        public string LocationId { get; set; }
        [Field]
        public string SeriesId { get; set; }
        [Field]
        public DateTime Newsletter { get; set; }
        [Field]
        public string Comments { get; set; }
        #endregion Properties

        #region Constructor
        /// <summary>
        /// Initialises a new instance of the 
        /// <see cref="PerformanceSimple"/> class.
        /// </summary>
        public PerformanceSimple() {
        }
        #endregion Constructor
    }//End of class
}//End of namespace
