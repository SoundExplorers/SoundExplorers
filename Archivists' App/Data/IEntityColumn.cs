using System;

namespace SoundExplorers.Data {
  /// <summary>
  ///   Entity column metadata interface.
  /// </summary>
  public interface IEntityColumn {

    /// <summary>
    ///   Gets or sets the type of data stored in the property.
    /// </summary>
    Type DataType { get; set; }
    
    /// <summary>
    ///   Gets or sets the display name to be used for reporting.
    /// </summary>
    string DisplayName { get; set; }
    
    /// <summary>
    ///   Gets or sets the name of the property of the entity.
    /// </summary>
    string PropertyName { get; set; }

    /// <summary>
    ///   Gets or sets the name of the property on
    ///   the referenced entity.
    ///   Null if the column is not from a referenced entity.
    /// </summary>
    string ReferencedPropertyName { get; set; }

    /// <summary>
    ///   Gets or sets the name of
    ///   the referenced entity type.
    ///   Null if the column is not from a referenced entity.
    /// </summary>
    string ReferencedEntityName { get; set; }
  } //End of class
} //End of namespace