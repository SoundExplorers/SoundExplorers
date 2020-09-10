namespace SoundExplorers.Data {
  public interface IEntity {
    /// <summary>
    /// TODO Removed once old EntityList is removed.
    /// </summary>
    EntityColumnList Columns { get; }
    EntityBase IdentifyingParent { get; }
    Key Key { get; }
    string SimpleKey { get; }
  }
}