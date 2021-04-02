using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Tests.Data;
using SoundExplorers.Tests.Model;

namespace SoundExplorers.Tests.Controller {
  [TestFixture]
  public class MainControllerTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      Data = new TestData(QueryHelper);
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
      Session = new TestSession(DatabaseFolderPath);
      Session.BeginUpdate();
      Data.AddRootsPersistedIfRequired(Session);
      Session.Commit();
      Connection = new MockDatabaseConnection();
    }

    [TearDown]
    public void TearDown() {
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }

    private QueryHelper QueryHelper { get; set; } = null!;
    private TestData Data { get; set; } = null!;
    private string DatabaseFolderPath { get; set; } = null!;
    private TestSession Session { get; set; } = null!;
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