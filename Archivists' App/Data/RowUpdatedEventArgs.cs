﻿using System;
using System.Data;
using System.Diagnostics;

namespace SoundExplorers.Data {

    /// <summary>
    /// Provides data for a RowUpdated event
    /// that is raised when a <see cref="DataRow"/>
    /// has been successfully inserted, updated or deleted on the database.
    /// </summary>
    internal class RowUpdatedEventArgs : EventArgs {

        #region Properties
        /// <summary>
        /// Gets the entity that has been inserted, updated or deleted.
        /// </summary>
        public virtual IEntity Entity { get; private set; }

        /// <summary>
        /// Gets a message describing the insertion, update or deletion.
        /// </summary>
        public virtual string Message { get; private set; }

        /// <summary>
        /// Gets the index of the <see cref="DataRow"/> 
        /// that was inserted, updated or deleted.
        /// For inserts and updates,
        /// this is also the index of the Entity in the EntityList.
        /// </summary>
        public virtual int RowIndex { get; private set; }

        /// <summary>
        /// Gets the type of SQL statement executed.
        /// </summary>
        public virtual StatementType StatementType { get; private set; }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Initialises an instance of the 
        /// <see cref="RowUpdatedEventArgs"/> class.
        /// </summary>
        /// <param name="rowIndex">
        /// The index of the <see cref="DataRow"/> 
        /// that was inserted, updated or deleted.
        /// </param>
        /// <param name="entity">
        /// The entity that has been inserted, updated or deleted.
        /// </param>
        /// <param name="statementType">
        /// The type of SQL statement executed.
        /// </param>
        /// <param name="message">
        /// A message describing the insertion, update or deletion.
        /// </param>
        public RowUpdatedEventArgs(
                int rowIndex, 
                IEntity entity,
                StatementType statementType, 
                string message) {
            RowIndex = rowIndex;
            Entity = entity;
            StatementType = statementType;
            Message = message;
        }
        #endregion Constructors
    }//End of class
}//End of namespace