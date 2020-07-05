using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Devart.Data.PostgreSql;

namespace SoundExplorers.Data {

    /// <summary>
    /// ArtistInImage entity.
    /// </summary>
    internal class ArtistInImage : Entity<ArtistInImage> {

        #region Public Properties
        [PrimaryKeyField]
        public int ImageId { get; set; }

        [PrimaryKeyField]
        [ReferencedField("Name")]
        public string Artist { get; set; }

        [HiddenField]
        public string Forename { get; set; }

        [HiddenField]
        public string Surname { get; set; }
        #endregion Public Properties

        #region Constructor
        /// <summary>
        /// Initialises a new instance of the 
        /// <see cref="ArtistInImage"/> class.
        /// </summary>
        public ArtistInImage() {
        }
        #endregion Constructor
    }//End of class
}//End of namespace
