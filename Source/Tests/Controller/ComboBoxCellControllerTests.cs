﻿using System;
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
        Data.AddActsPersisted(3, Session);
        Data.AddEventTypesPersisted(1, Session);
        Data.AddLocationsPersisted(2, Session);
        Data.AddNewslettersPersisted(3, Session);
        Data.AddSeriesPersisted(1, Session);
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
      Session.BeginUpdate();
      Data.Newsletters[0].Date = DateTime.Parse("1960-09-13");
      Session.Commit();
      string key = ComboBoxCellController.GetKey(Data.Newsletters[0])!;
      Assert.AreEqual("1960/09/13", key, "Date key");
      CellController = CreateCellController("Series");
      Assert.AreEqual("Event", CellController.TableName, "TableName");
      var seriesItems = CellController.GetItems();
      Assert.AreEqual(1, seriesItems.Length, "seriesItems.Length");
      Assert.AreEqual(0, EditorView.ShowWarningMessageCount, "ShowWarningMessageCount");
      CellController = CreateCellController("Location");
      var locationItems = CellController.GetItems();
      Assert.AreEqual(2, locationItems.Length, "locationItems.Length");
      CellController = CreateCellController("NewsLetter");
      var newsLetterItems = CellController.GetItems();
      // Includes default newsletter
      Assert.AreEqual(3, newsLetterItems.Length, "newsLetterItems.Length");
    }

    [Test]
    public void NoAvailableReferences() {
      CreateControllers(typeof(SetList));
      CellController = CreateCellController("Genre");
      Assert.AreEqual("Set", CellController.TableName, "TableName");
      CellController.Column.FetchReferenceableItems();
      var genreItems = CellController.GetItems();
      Assert.AreEqual(0, genreItems.Length, "genreItems.Length");
      Assert.AreEqual(1, EditorView.ShowWarningMessageCount, "ShowWarningMessageCount");
      Assert.AreEqual(
        "There are no Genre Names to choose between. You need to add at least one row " +
        "to the Genre table before you can select a Genre for a Set.",
        EditorView.LastWarningMessage, "LastWarningMessage");
    }

    [Test]
    public void StringKeyValue() {
      const string keyValue = "ABC";
      string key = ComboBoxCellController.GetKey(keyValue)!;
      Assert.AreEqual(keyValue, key);
    }

    private ComboBoxCellController CreateCellController(string columnName) {
      return new ComboBoxCellController(CellView, MainGridController, columnName);
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