using System;
using System.IO;
using NUnit.Framework;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests.Model {
  [TestFixture]
  public class DatabaseConnectionTests {
    private class TestDatabaseConfig : DatabaseConfig {
      public string TestDatabaseFolderPath { get; set; }
      protected override string SetDatabaseFolderPath() {
        return TestDatabaseFolderPath ?? base.SetDatabaseFolderPath();
      }
    }
    private class TestDatabaseConnection : DatabaseConnection {

      protected override DatabaseConfig CreateDatabaseConfig() {
        var result = new TestDatabaseConfig {
          ConfigFilePath = TestSession.DatabaseParentFolderPath +
                           Path.DirectorySeparatorChar + "DatabaseConfig.xml",
          TestDatabaseFolderPath = TestDatabaseFolderPath
        };
        return result;
      }
      
      //public TestDatabaseConfig TestDatabaseConfig => (TestDatabaseConfig)DatabaseConfig;
      public string TestDatabaseFolderPath { get; set; }
    }

    [Test]
    public void DatabaseFolderNotFound() {
      var connection = new TestDatabaseConnection {
        TestDatabaseFolderPath = @"C:\Non-existent folder"
      };
      Assert.Throws<ApplicationException>(()=>connection.Open());
    }
  }
}