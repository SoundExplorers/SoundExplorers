using SoundExplorers.Data;
using SoundExplorers.Model;
using VelocityDb.Session;

namespace SoundExplorers.Tests.Model {
  public class TestDatabaseConnection : DatabaseConnection {
    public TestDatabaseConnection(string configFilePath,
      string databaseFolderPath, QueryHelper queryHelper) {
      ConfigFilePath = configFilePath;
      DatabaseFolderPath = databaseFolderPath;
      QueryHelper = queryHelper;
      ExpectedSchemaVersion = 66;
    }

    internal new DatabaseConfig DatabaseConfig => base.DatabaseConfig;
    internal MockBackupManager MockBackupManager => (MockBackupManager)BackupManager;
    private string ConfigFilePath { get; }
    private string DatabaseFolderPath { get; }
    private QueryHelper QueryHelper { get; }
    
    protected override IBackupManager CreateBackupManager(SessionBase session) {
      return new MockBackupManager();
    }

    protected override DatabaseConfig CreateDatabaseConfig() {
      return new TestDatabaseConfig(ConfigFilePath, DatabaseFolderPath);
    }

    protected override QueryHelper CreateQueryHelper() {
      return QueryHelper;
    }
  }
}