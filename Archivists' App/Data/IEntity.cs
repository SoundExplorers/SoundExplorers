namespace SoundExplorers.Data {
  public interface IEntity {
    EntityColumnList Columns { get; }
    EntityBase IdentifyingParent { get; }
    Key Key { get; }
    string SimpleKey { get; }
  }
}