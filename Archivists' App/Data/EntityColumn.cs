using System;

namespace SoundExplorers.Data {
  /// <summary>
  ///   Entity column metadata.
  /// </summary>
  internal class EntityColumn<TEntity> : IEntityColumn
    where TEntity : EntityBase {
    public Type DataType { get; set; }
    public string DisplayName { get; set; }
    public bool IsVisible { get; set; }
    public string PropertyName { get; set; }
    public string ReferencedPropertyName { get; set; }
    public string ReferencedEntityName { get; set; }
  } //End of class
} //End of namespace