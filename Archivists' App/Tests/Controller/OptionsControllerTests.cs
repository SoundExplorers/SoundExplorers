using System;
using NUnit.Framework;
using SoundExplorers.Controller;
using SoundExplorers.Tests.Model;

namespace SoundExplorers.Tests.Controller {
  [TestFixture]
  public class OptionsControllerTests {
    [SetUp]
    public void Setup() {
      View = new MockView<OptionsController>();
      Controller = new TestOptionsController(View,
        new MockDatabaseConfig());
      Controller.MockDatabaseConfig.TestConfigFilePath = "Test Config File";
      Controller.MockDatabaseConfig.TestDatabaseFolderPath = "Test Database Folder";
    }

    private TestOptionsController Controller { get; set; }
    private MockView<OptionsController> View { get; set; }

    [Test]
    public void Initial() {
      Assert.AreSame(Controller, View.Controller, "View.Controller");
    }

    [Test]
    public void Error() {
      Controller.MockDatabaseConfig.ApplicationExceptionOnLoad =
        new ApplicationException("Error Message");
      Controller.LoadDatabaseConfig();
      Assert.IsEmpty(Controller.DatabaseFolderPath, "DatabaseFolderPath");
      Assert.IsTrue(
        Controller.Message.StartsWith(Controller.MockDatabaseConfig
          .ApplicationExceptionOnLoad.Message),
        "Message");
    }

    [Test]
    public void Normal() {
      Controller.LoadDatabaseConfig();
      Assert.AreEqual(Controller.MockDatabaseConfig.TestDatabaseFolderPath,
        Controller.DatabaseFolderPath, "DatabaseFolderPath");
      Assert.IsTrue(
        Controller.Message.Contains(Controller.MockDatabaseConfig.TestConfigFilePath),
        "Message");
    }
  }
}