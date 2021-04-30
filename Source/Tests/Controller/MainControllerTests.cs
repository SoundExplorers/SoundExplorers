using NUnit.Framework;
using SoundExplorers.Tests.Data;
using SoundExplorers.Tests.Model;

namespace SoundExplorers.Tests.Controller {
  [TestFixture]
  public class MainControllerTests : TestFixtureBase {
    [SetUp]
    public override void Setup() {
      base.Setup();
      Connection = new MockDatabaseConnection();
    }

    private MockDatabaseConnection Connection { get; set; } = null!;

    [Test]
    public void TheTest() {
      const string tableName = "Set";
      var view = new MockMainView();
      var controller = new TestMainController(view, QueryHelper, Session);
      Assert.AreSame(controller, view.Controller, "view.Controller");
      controller.DatabaseConnection = Connection;
      controller.ConnectToDatabase();
      Assert.AreEqual(1, Connection.OpenCount, "OpenCount");
      Assert.IsTrue(controller.IsToolBarVisible, "IsToolBarVisible initially");
      Assert.IsTrue(controller.IsStatusBarVisible, "IsStatusBarVisible initially");
      controller.IsToolBarVisible = false;
      controller.IsStatusBarVisible = false;
      controller.TableName = tableName;
      controller = new TestMainController(view, QueryHelper, Session);
      Assert.IsFalse(controller.IsToolBarVisible, "IsToolBarVisible");
      Assert.IsFalse(controller.IsStatusBarVisible, "IsStatusBarVisible");
      Assert.AreEqual(tableName, controller.TableName, "TableName");
    }
  }
}