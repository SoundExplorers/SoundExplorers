using System;
using JetBrains.Annotations;
using NUnit.Framework;
using SoundExplorers.Controller;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;
using SoundExplorers.Tests.Model;

namespace SoundExplorers.Tests.Controller {
  [TestFixture]
  public class ComboBoxCellControllerTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      Data = new TestData(QueryHelper);
      Session = new TestSession();
      Editor = new TestEditor<Event, EventBindingItem>(QueryHelper, Session);
      EditorView = new MockEditorView();
      EditorController = CreateEditorController(typeof(EventList));
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
      EditorController.FetchData(); // Populate grid
      Editor.SetBindingList(EditorController.MainBindingList);
    }

    [TearDown]
    public void TearDown() {
      Session.DeleteDatabaseFolderIfExists();
    }

    private TestComboBoxCellController CellController { get; set; }
    private MockView<ComboBoxCellController> CellView { get; set; }
    private TestEditor<Event, EventBindingItem> Editor { get; set; }
    private MockEditorView EditorView { get; set; }
    private TestData Data { get; set; }
    private TestEditorController EditorController { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private TestSession Session { get; set; }

    [Test]
    public void FetchItems() {
      string key = ComboBoxCellController.GetKey(null, null);
      Assert.IsNull(key, "Null key");
      Session.BeginUpdate();
      Data.Newsletters[0].Date = DateTime.Parse("1960-09-13");
      Session.Commit();
      key = ComboBoxCellController.GetKey(Data.Newsletters[0], "dd MMM yyyy");
      Assert.AreEqual("13 Sep 1960", key, "Date key");
      CellController = CreateCellController("Series");
      Assert.AreEqual("Event", CellController.TableName, "TableName");
      var seriesItems = CellController.FetchItems(null);
      Assert.AreEqual(0, seriesItems.Length, "seriesItems.Length");
      Assert.AreEqual(1, EditorView.ShowWarningMessageCount, "ShowWarningMessageCount");
      CellController = CreateCellController("Location");
      var locationItems = CellController.FetchItems(null);
      Assert.AreEqual(2, locationItems.Length, "locationItems.Length");
      CellController = CreateCellController("NewsLetter");
      var newsLetterItems = CellController.FetchItems("dd MMM yyyy");
      Assert.AreEqual(3, newsLetterItems.Length, "newsLetterItems.Length");
    }

    [Test]
    public void StringKeyValue() {
      const string keyValue = "ABC";
      string key = ComboBoxCellController.GetKey(keyValue, null);
      Assert.AreEqual(keyValue, key);
    }

    [NotNull]
    private TestComboBoxCellController CreateCellController([NotNull] string columnName) {
      return new TestComboBoxCellController(CellView, EditorController, columnName,
        Session);
    }

    [NotNull]
    private TestEditorController CreateEditorController([NotNull] Type mainListType) {
      return new TestEditorController(EditorView, mainListType, QueryHelper, Session);
    }
  }
}