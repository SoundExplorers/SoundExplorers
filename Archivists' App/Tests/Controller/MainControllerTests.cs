using NUnit.Framework;
using SoundExplorers.Controller;
using SoundExplorers.Data;
using SoundExplorers.Tests.Data;
using SoundExplorers.Tests.Model;

namespace SoundExplorers.Tests.Controller {
  [TestFixture]
  public class MainControllerTests {
    [SetUp]
    public void Setup() {
      Connection = new MockDatabaseConnection();
      QueryHelper = new QueryHelper();
      Session = new TestSession();
    }

    [TearDown]
    public void TearDown() {
      Session.DeleteDatabaseFolderIfExists();
    }

    private MockDatabaseConnection Connection { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private TestSession Session { get; set; }

    [Test]
    public void TheTest() {
      const string tableName = "Set";
      var view = new MockView<MainController>();
      var controller = new TestMainController(view, QueryHelper, Session);
      Assert.AreSame(controller, view.Controller, "view.Controller");
      controller.DatabaseConnection = Connection;
      controller.ConnectToDatabase();
      Assert.AreEqual(1, Connection.OpenCount, "OpenCount");
      Assert.IsTrue(controller.IsStatusBarVisible, "IsStatusBarVisible initially");
      Assert.IsTrue(controller.IsToolBarVisible, "IsToolBarVisible initially");
      controller.IsStatusBarVisible = false;
      controller.IsToolBarVisible = false;
      controller.TableName = tableName;
      controller = new TestMainController(view, QueryHelper, Session);
      Assert.IsFalse(controller.IsStatusBarVisible, "IsStatusBarVisible");
      Assert.IsFalse(controller.IsToolBarVisible, "IsToolBarVisible");
      Assert.AreEqual(tableName, controller.TableName, "TableName");
    }
  }
}