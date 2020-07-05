using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundExplorers.Data {

    /// <summary>
    /// Credit entity.
    /// </summary>
    internal class Credit : Entity<Credit>, IMediaEntity {

        #region Public Field Properties
        [PrimaryKeyField]
        public DateTime Date { get; set; }

        [PrimaryKeyField]
        [ReferencedField("Name")]
        public string Location { get; set; }

        [PrimaryKeyField]
        [ReferencedField("SetNo")]
        public int Set { get; set; }

        [PrimaryKeyField]
        [ReferencedField("PieceNo")]
        public int Piece { get; set; }

        [PrimaryKeyField]
        [ReferencedField("Name")]
        public string Artist { get; set; }

        [PrimaryKeyField]
        [ReferencedField("Name")]
        public string Role { get; set; }

        [Field]
        public string Act { get; set; }

        [HiddenField]
        public string Forename { get; set; }

        [HiddenField]
        public string Surname { get; set; }
        #endregion Public Field Properties

        #region Constructor
        /// <summary>
        /// Initialises a new instance of the 
        /// <see cref="Credit"/> class.
        /// </summary>
        public Credit() {
        }
        #endregion Constructor

        #region Public Methods
        /// <summary>
        /// Updates the metadata tags of any audio file
        /// associated with the Credit's Piece with appropriate data from the database.
        /// </summary>
        /// <returns>
        /// A message describing the update or,
        /// if no audio file has been updated,
        /// an empty string.
        /// </returns>
        /// <exception cref="ApplicationException">
        /// An error occured while attempting to save the tags.
        /// </exception>
        public string UpdateTags() {
            var piece = new Piece();
            piece.Date = Date;
            piece.Location = Location;
            piece.Set = Set;
            piece.PieceNo = Piece;
            piece.Fetch();
            return piece.UpdateTags();
        }
        #endregion Public Methods
    }//End of class
}//End of namespace
