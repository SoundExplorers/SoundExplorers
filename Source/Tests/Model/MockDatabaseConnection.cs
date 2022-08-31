using SoundExplorers.Model;

namespace SoundExplorers.Tests.Model {
  public class MockDatabaseConnection : IDatabaseConnection {
    public int OpenCount { get; private set; }
    public SchemaUpgradeStatus SchemaUpgradeStatus { get; internal set; }

    public void Open() {
      OpenCount++;
    }
  }
}