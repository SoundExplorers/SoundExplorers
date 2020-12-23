using SoundExplorers.Model;

namespace SoundExplorers.Tests.Model {
  public class MockDatabaseConnection : IOpen {
    public int OpenCount { get; private set; }

    public void Open() {
      OpenCount++;
    }
  }
}