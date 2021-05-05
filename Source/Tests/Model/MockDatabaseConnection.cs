using SoundExplorers.Model;

namespace SoundExplorers.Tests.Model {
  public class MockDatabaseConnection : IDatabaseConnection {
    public int OpenCount { get; private set; }
    public bool MustBackup { get; set; }

    public void Open() {
      OpenCount++;
    }
  }
}