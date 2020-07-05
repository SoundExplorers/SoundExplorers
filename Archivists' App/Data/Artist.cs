using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundExplorers.Data {

    /// <summary>
    /// Artist entity.
    /// </summary>
    internal class Artist : PieceOwningMediaEntity<Artist> {

        //#region Private Fields
        //private string _name;
        //private string _forename;
        //private string _surname;
        //#endregion Private Fields

        #region Public Field Properties
        [PrimaryKeyField]
        [HiddenField]
        public string Name {
            get {
	            // ltrim(rtrim(coalesce(@Forename, '') || ' ' || coalesce(@Surname, ''))),
                return 
                    ((Forename != null ? Forename : string.Empty)
                    + " "
                    + (Surname != null ? Surname : string.Empty)).Trim();
            }
        }

        [UniqueKeyField]
        public string Forename { get; set; }

        [UniqueKeyField]
        public string Surname { get; set; }

        [Field]
        public string Comments { get; set; }
        #endregion Public Field Properties
    }//End of class
}//End of namespace
