namespace SoundExplorers.Data {
  public interface IEntity2 {
    EntityBase2? IdentifyingParent { get; }
    Key2 Key { get; }
    string SimpleKey { get; }
  }
}