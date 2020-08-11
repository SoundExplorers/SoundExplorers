using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  public interface IKey {
    RelativeBase IdentifyingParent { get; }
    string SimpleKey { get; }
    bool Matches([CanBeNull] IKey otherKey);
    string ToString();
  }
}