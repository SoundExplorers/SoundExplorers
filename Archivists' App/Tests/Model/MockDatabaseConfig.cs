using System;
using SoundExplorers.Model;

namespace SoundExplorers.Tests.Model {
  public class MockDatabaseConfig : IDatabaseConfig {
    internal ApplicationException ApplicationExceptionOnLoad { get; set; } = null!;
    internal string TestConfigFilePath { get; init; } = null!;
    internal string TestDatabaseFolderPath { get; init; } = null!;
    public string ConfigFilePath { get; private set; } = null!;
    public string DatabaseFolderPath { get; private set; } = null!;

    public void Load() {
      if (ApplicationExceptionOnLoad != null) {
        throw ApplicationExceptionOnLoad;
      }
      ConfigFilePath = TestConfigFilePath;
      DatabaseFolderPath = TestDatabaseFolderPath;
    }
  }
}