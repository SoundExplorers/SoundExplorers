using System;
using NUnit.Framework;
using SoundExplorers.Controller;
using SoundExplorers.Tests.Model;

namespace SoundExplorers.Tests.Controller {
  [TestFixture]
  public class OptionsControllerTests {
    [SetUp]
    public void Setup() {
      DatabaseConfig = new MockDatabaseConfig();
      View = new MockView<OptionsController>();
      Controller = new TestOptionsController(View, DatabaseConfig);
      DatabaseConfig.TestConfigFilePath = "Test Config File";
      DatabaseConfig.TestDatabaseFolderPath = "Test Database Folder";
    }

    private TestOptionsController Controller { get; set; }
    private MockDatabaseConfig DatabaseConfig { get; set; }
    private MockView<OptionsController> View { get; set; }

    [Test]
    public void A010_Initial() {
      Assert.AreSame(Controller, View.Controller, "View.Controller");
    }

    [Test]
    public void Error() {
      DatabaseConfig.ApplicationExceptionOnLoad =
        new ApplicationException("Error Message");
      Controller.LoadDatabaseConfig();
      Assert.IsEmpty(Controller.DatabaseFolderPath, "DatabaseFolderPath");
      Assert.IsTrue(
        Controller.Message.StartsWith(DatabaseConfig
          .ApplicationExceptionOnLoad.Message),
        "Message");
    }

    [Test]
    public void Normal() {
      Controller.LoadDatabaseConfig();
      Assert.AreEqual(DatabaseConfig.TestDatabaseFolderPath,
        Controller.DatabaseFolderPath, "DatabaseFolderPath");
      Assert.IsTrue(
        Controller.Message.Contains(DatabaseConfig.TestConfigFilePath),
        "Message");
    }
  }
}