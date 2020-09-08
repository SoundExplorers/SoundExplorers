namespace SoundExplorers.Data {
  public interface IEntity {
    IEntityColumnList Columns { get; }
    EntityBase IdentifyingParent { get; }
    Key Key { get; }
    string SimpleKey { get; }
  }
}