namespace SoundExplorersDatabase.Data {
  public interface IKeyedRelative {
    IKeyedRelative IdentifyingParent { get; }
    Key Key { get; }
    string SimpleKey { get; }
  }
}