using SoundExplorers.Model;

namespace SoundExplorers.Tests.Model {
  public class TestDatabaseConfig : DatabaseConfig {
    public TestDatabaseConfig(string configFilePath,
      string testDatabaseFolderPath) {
      ConfigFilePath = configFilePath;
      TestDatabaseFolderPath = testDatabaseFolderPath;
    }

    private string TestDatabaseFolderPath { get; }

    protected override string GetDatabaseFolderPath() {
      return TestDatabaseFolderPath;
    }
  }
}