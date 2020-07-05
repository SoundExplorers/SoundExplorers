using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace SoundExplorers.Data {

    /// <summary>
    /// Entity list interface.
    /// </summary>
    internal interface IEntityList :  IEntityColumnContainer, IList {

        #region Events
        /// <summary>
        /// An event that is raised when there is an error on 
        /// attempting to insert, update or delete a <see cref="DataRow"/>.
        /// </summary>
        event EventHandler<RowErrorEventArgs> RowError;

        /// <summary>
        /// An event that is raised when a <see cref="DataRow"/>
        /// has been successfully inserted or updated on the database.
        /// </summary>
        event EventHandler<RowUpdatedEventArgs> RowUpdated;
        #endregion Events

        #region Properties
        /// <summary>
        /// Gets the data set containing the main <see cref="Table"/>
        /// and, if specified, the parent table.
        /// </summary>
        DataSet DataSet { get; }

        /// <summary>
        /// Gets the list of entities representing the main table's
        /// parent table, if specified.
        /// </summary>
        IEntityList ParentList { get; }

        /// <summary>
        /// Gets name of the the data table containing the database records
        /// represented by the list of entities.
        /// </summary>
        DataTable Table { get; }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Adds an Entity to the end of the list.
        /// </summary>
        /// <param name="entity">The Entity to added</param>
        void Add(IEntity entity);

        /// <summary>
        /// Updates the database table with any changes that have been input
        /// and refreshes the list of <see cref="Entity"/>s.
        /// </summary>
        void Update(Dictionary<string, object> oldKeyFields = null);
        #endregion Methods
    }//End of interface
}//End of interface