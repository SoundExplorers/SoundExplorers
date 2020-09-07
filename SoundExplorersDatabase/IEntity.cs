namespace SoundExplorersDatabase {
  public interface IEntity {
    EntityBase IdentifyingParent { get; }
    Key Key { get; }
    string SimpleKey { get; }
  }
}