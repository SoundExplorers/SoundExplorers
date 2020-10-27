using JetBrains.Annotations;
using SoundExplorers.Model;

namespace SoundExplorers.Tests.Model {
  public class TestDatabaseConfig : DatabaseConfig {
    public TestDatabaseConfig([NotNull] string configFilePath,
      [NotNull] string testDatabaseFolderPath) {
      ConfigFilePath = configFilePath;
      TestDatabaseFolderPath = testDatabaseFolderPath;
    }

    [NotNull] private string TestDatabaseFolderPath { get; }

    protected override string SetDatabaseFolderPath() {
      return TestDatabaseFolderPath;
    }
  }
}