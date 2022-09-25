using System.Linq;
using VelocityDb.Collection.BTree;

namespace SoundExplorers.Data; 

/// <summary>
///   A sorted collection of entities of the specified type.
/// </summary>
public class SortedEntityCollection<TEntity> : BTreeMap<Key, TEntity>,
  ISortedEntityCollection
  where TEntity : EntityBase {
  public SortedEntityCollection() : base(new KeyComparer(), null) { }

  /// <summary>
  ///   From VelocityDB User's Guide:
  ///   'It is recommended that you make the following override in your
  ///   OptimizedPersistable subclass for better performance. ...
  ///   We may make this default but it could break existing code
  ///   so it is not a trivial change.'
  /// </summary>
  public override bool AllowOtherTypesOnSamePage => false;

  public TEntity this[int index] => Values.ToList()[index];
  IEntity ISortedEntityCollection.this[Key key] => this[key];

  void ISortedEntityCollection.Add(Key key, IEntity child) {
    Add(key, (TEntity)child);
  }

  void ISortedEntityCollection.Remove(Key key) {
    Remove(key);
  }
}