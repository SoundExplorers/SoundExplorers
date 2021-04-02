using NUnit.Framework;
using SoundExplorers.Controller;
using SoundExplorers.Data;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests.Controller {
  [TestFixture]
  public class SizeableFormOptionsControllerTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      Data = new TestData(QueryHelper);
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
      Session = new TestSession(DatabaseFolderPath);
      Session.BeginUpdate();
      Data.AddRootsPersistedIfRequired(Session);
      Session.Commit();
      View = new MockView<SizeableFormOptionsController>();
    }

    [TearDown]
    public void TearDown() {
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }

    private QueryHelper QueryHelper { get; set; } = null!;
    private TestData Data { get; set; } = null!;
    private string DatabaseFolderPath { get; set; } = null!;
    private TestSession Session { get; set; } = null!;
    private SizeableFormOptionsController Controller { get; set; } = null!;
    private MockView<SizeableFormOptionsController> View { get; set; } = null!;

    [Test]
    public void TheTest() {
      const string formName = "TestView";
      const int height = 1;
      const int left = 2;
      const int top = 3;
      const int width = 4;
      const int windowState = 5;
      Controller = CreateController(formName);
      Controller.Height = height;
      Controller.Left = left;
      Controller.Top = top;
      Controller.Width = width;
      Controller.WindowState = windowState;
      Controller = CreateController(formName);
      Assert.AreEqual(height, Controller.Height, "Height");
      Assert.AreEqual(left, Controller.Left, "Left");
      Assert.AreEqual(top, Controller.Top, "Top");
      Assert.AreEqual(width, Controller.Width, "Width");
      Assert.AreEqual(windowState, Controller.WindowState, "WindowState");
    }

    private SizeableFormOptionsController CreateController(string formName) {
      return new TestSizeableFormOptionsController(View, formName, QueryHelper, Session);
    }
  }
}