﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SoundExplorers.Data {

    /// <summary>
    /// Entity interface.
    /// </summary>
    internal interface IEntity : IEntityColumnContainer, ICloneable {

        #region Methods
        /// <summary>
        /// Deletes the row of the table whose primary key field values
        /// match the values of the corresponding primary key column properties.
        /// </summary>
        void Delete();

        /// <summary>
        /// Populates those field properties that are not in the primary key
        /// to the corresponding field values
        /// on the row, if found, of the table whose primary key field values
        /// match the values of the corresponding primary key column properties.
        /// </summary>
        /// <returns>
        /// Whether a matching row on the table was found.
        /// </returns>
        bool Fetch();

        /// <summary>
        /// If the record exists,
        /// updates the field values of the
        /// record with the corresponding 
        /// column property values.
        /// Otherwise inserts a new record.
        /// </summary>
        void Save();
        #endregion Methods
    }//End of class
}//End of namespace