using System;
using NUnit.Framework;
using SoundExplorers.Controller;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests.Controller {
  [TestFixture]
  public class ComboBoxCellControllerTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      Data = new TestData(QueryHelper);
      Session = new TestSession();
      EditorView = new MockEditorView(new MockMainGrid(), new MockParentGrid());
      CreateControllers(typeof(EventList));
      CellView = new MockView<ComboBoxCellController>();
      Session.BeginUpdate();
      try {
        Data.AddEventTypesPersisted(1, Session);
        Data.AddLocationsPersisted(2, Session);
        Data.AddNewslettersPersisted(3, Session);
        Data.AddEventsPersisted(5, Session);
      } finally {
        Session.Commit();
      }
      EditorController.Populate(); // Populate grid
    }

    [TearDown]
    public void TearDown() {
      Session.DeleteDatabaseFolderIfExists();
    }

    private ComboBoxCellController CellController { get; set; } = null!;
    private MockView<ComboBoxCellController> CellView { get; set; } = null!;
    private TestMainGridController MainGridController { get; set; } = null!;
    private TestEditorController EditorController { get; set; } = null!;
    private MockEditorView EditorView { get; set; } = null!;
    private TestData Data { get; set; } = null!;
    private QueryHelper QueryHelper { get; set; } = null!;
    private TestSession Session { get; set; } = null!;

    [Test]
    public void FetchItems() {
      string key = ComboBoxCellController.GetKey(null)!;
      Assert.IsNull(key, "Null key");
      Session.BeginUpdate();
      Data.Newsletters[0].Date = DateTime.Parse("1960-09-13");
      Session.Commit();
      key = ComboBoxCellController.GetKey(Data.Newsletters[0])!;
      Assert.AreEqual("1960/09/13", key, "Date key");
      CellController = CreateCellController("Series");
      Assert.AreEqual("Event", CellController.TableName, "TableName");
      var seriesItems = CellController.GetItems();
      Assert.AreEqual(0, seriesItems.Length, "seriesItems.Length");
      Assert.AreEqual(1, EditorView.ShowWarningMessageCount, "ShowWarningMessageCount");
      CellController = CreateCellController("Location");
      var locationItems = CellController.GetItems();
      Assert.AreEqual(2, locationItems.Length, "locationItems.Length");
      CellController = CreateCellController("NewsLetter");
      var newsLetterItems = CellController.GetItems();
      // Includes dummy newsletter
      Assert.AreEqual(4, newsLetterItems.Length, "newsLetterItems.Length");
    }

    [Test]
    public void StringKeyValue() {
      const string keyValue = "ABC";
      string key = ComboBoxCellController.GetKey(keyValue)!;
      Assert.AreEqual(keyValue, key);
    }

    private ComboBoxCellController CreateCellController(string columnName) {
      return new(CellView, MainGridController, columnName);
    }

    private void CreateControllers(Type mainListType) {
      EditorController = new TestEditorController(mainListType, EditorView, QueryHelper,
        Session);
      MainGridController = EditorView.MainGridController =
        new TestMainGridController(EditorView.MainGrid, EditorController);
      EditorView.MainGrid.SetController(MainGridController);
    }
  }
}