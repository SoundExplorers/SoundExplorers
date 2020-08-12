namespace SoundExplorersDatabase.Data {
  public interface IRelative {
    IRelative IdentifyingParent { get; }
    Key Key { get; }
    string SimpleKey { get; }
  }
}