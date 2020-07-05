using System;

namespace SoundExplorers.Data {

    /// <summary>
    /// Indicates that a property of an entity class represents a 
    /// database field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    internal class FieldAttribute : Attribute {
    }//End of class

    /// <summary>
    /// Indicates that a property of an entity class represents a 
    /// database field that is not to be shown in the table editor.
    /// </summary>
    /// <remarks>
    /// Primary key columns that are shown in the parent grid
    /// in the table editor will automatically be hidden in the main grid
    /// and should not have this attribute.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal class HiddenFieldAttribute : FieldAttribute {
    }//End of class

    /// <summary>
    /// Indicates that a property of an entity class represents a 
    /// primary key field required for database access.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal class PrimaryKeyFieldAttribute : FieldAttribute {
    }//End of class

    /// <summary>
    /// Indicates that a property of an entity class represents a 
    /// field required for database access,
    /// where the field is populated from a column
    /// on a referenced database table whose table name
    /// and Entity name is the property name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal class ReferencedFieldAttribute : FieldAttribute {

        #region Public Properties
        /// <summary>
        /// Gets the name of the field property on 
        /// the referenced Entity, 
        /// optionally prefixed by the 
        /// table name followed by a dot.  
        /// </summary>
        /// <remarks>
        /// The table name, if not included,
        /// will be assumed to be the 
        /// name of the field property on 
        /// the referencing Entity.
        /// </remarks>
        public string Name { get; private set; }
        #endregion Public Properties

        #region Constructor
        /// <summary>
        /// Initialises a new instance of the 
        /// <see cref="ReferencedFieldAttribute"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the field property on 
        /// the referenced Entity, 
        /// optionally prefixed by the 
        /// table name followed by a dot.  
        /// </param>
        /// <remarks>
        /// The table name, if not included in
        /// <paramref name="name"/>,
        /// will be assumed to be the 
        /// name of the field property on 
        /// the referencing Entity.
        /// </remarks>
        public ReferencedFieldAttribute(
                string name) {
            Name = name;
        }
        #endregion Constructor
    }//End of class

    ///// <summary>
    ///// Indicates the name of the column as on the database table.
    ///// </summary>
    //[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    //internal class NameOnDbAttribute : FieldAttribute {

    //    #region Public Properties
    //    /// <summary>
    //    /// Gets the name of the column as on the database table.  
    //    /// </summary>
    //    public string Value { get; private set; }
    //    #endregion Public Properties

    //    #region Constructor
    //    /// <summary>
    //    /// Initialises a new instance of the 
    //    /// <see cref="NameOnDbAttribute"/> class.
    //    /// </summary>
    //    /// <param name="name">
    //    /// The name of the column as on the database table.  
    //    /// </param>
    //    public NameOnDbAttribute(
    //            string name) {
    //        Value = name;
    //    }
    //    #endregion Constructor
    //}//End of class

    /// <summary>
    /// Indicates that a property of an entity class represents a 
    /// unique key field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal class UniqueKeyFieldAttribute : FieldAttribute {
    }//End of class
}//End of namespace