using System;
using JetBrains.Annotations;
using SoundExplorers.Model;

namespace SoundExplorers.Tests.Model {
  public class MockDatabaseConfig : IDatabaseConfig {
    internal ApplicationException ApplicationExceptionOnLoad { get; set; }
    internal string TestConfigFilePath { get; set; }
    internal string TestDatabaseFolderPath { get; set; }
    public string ConfigFilePath { get; private set; }
    public string DatabaseFolderPath { get; private set; }

    public void Load() {
      if (ApplicationExceptionOnLoad != null) {
        throw ApplicationExceptionOnLoad;
      }
      ConfigFilePath = TestConfigFilePath;
      DatabaseFolderPath = TestDatabaseFolderPath;
    }
  }
}