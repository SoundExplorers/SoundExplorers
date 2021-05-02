using SoundExplorers.Model;

namespace SoundExplorers.Tests.Model {
  public class TestDatabaseConnection : DatabaseConnection {
    public TestDatabaseConnection(string configFilePath,
      string databaseFolderPath) {
      ConfigFilePath = configFilePath;
      DatabaseFolderPath = databaseFolderPath;
      ExpectedSchemaVersion = 66;
    }

    internal new DatabaseConfig DatabaseConfig => base.DatabaseConfig; 
    private string ConfigFilePath { get; }
    private string DatabaseFolderPath { get; }

    protected override DatabaseConfig CreateDatabaseConfig() {
      return new TestDatabaseConfig(ConfigFilePath, DatabaseFolderPath);
    }
  }
}