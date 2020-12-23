using JetBrains.Annotations;
using SoundExplorers.Model;

namespace SoundExplorers.Tests.Model {
  public class TestDatabaseConnection : DatabaseConnection {
    public TestDatabaseConnection([NotNull] string configFilePath,
      [NotNull] string databaseFolderPath) {
      ConfigFilePath = configFilePath;
      DatabaseFolderPath = databaseFolderPath;
      ExpectedVersion = 66;
    }

    [NotNull] private string ConfigFilePath { get; }
    [NotNull] private string DatabaseFolderPath { get; }

    protected override DatabaseConfig CreateDatabaseConfig() {
      return new TestDatabaseConfig(ConfigFilePath, DatabaseFolderPath);
    }
  }
}