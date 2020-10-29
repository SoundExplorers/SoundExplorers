using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;
using SoundExplorers.Tests.Model;

namespace SoundExplorers.Tests.Controller {
  [TestFixture]
  public class EditorControllerTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      Data = new TestData(QueryHelper);
      Session = new TestSession();
      View = new MockEditorView();
    }

    [TearDown]
    public void TearDown() {
      Session.DeleteDatabaseFolderIfExists();
    }

    private TestEditorController Controller { get; set; }
    private TestData Data { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private TestSession Session { get; set; }
    private MockEditorView View { get; set; }

    [Test]
    public void Edit() {
      const string name1 = "Auntie";
      const string name2 = "Uncle";
      var editor = new TestEditor<Location, NotablyNamedBindingItem<Location>>();
      Controller = CreateTestEditorController(typeof(LocationList));
      Assert.IsFalse(Controller.IsParentTableToBeShown, "IsParentTableToBeShown");
      Controller.FetchData(); // The grid will be empty initially
      Assert.AreEqual("Location", Controller.MainTableName, "MainTableName");
      editor.SetBindingList(Controller.MainBindingList);
      editor.AddNew();
      Controller.OnMainGridRowEnter(0); // Go to insertion row
      editor[0].Name = name1;
      editor[0].Notes = "Disestablishmentarianism";
      Controller.OnMainGridRowValidated(0);
      editor.AddNew();
      Controller.OnMainGridRowEnter(1);
      editor[1].Name = name2;
      editor[1].Notes = "Bob";
      Controller.OnMainGridRowValidated(1);
      Assert.AreEqual(2, View.OnRowAddedOrDeletedCount, "OnRowAddedOrDeletedCount");
      Controller.FetchData(); // Refresh grid
      editor.SetBindingList(Controller.MainBindingList);
      Assert.AreEqual(2, editor.Count, "editor.Count after FetchData #2");
      Controller.OnMainGridRowEnter(1);
      // Disallow rename to duplicate
      try {
        editor[1].Name = name1;
        Assert.Fail(
          "Rename should have thrown DatabaseUpdateErrorException.");
      } catch (DatabaseUpdateErrorException exception) {
        Assert.AreEqual(name1, editor[1].Name,
          "Still duplicate name before error message shown for duplicate rename");
        Controller.OnMainGridDataError(exception);
      }
      Assert.AreEqual(1, View.EditMainGridCurrentCellCount,
        "EditMainGridCurrentCellCount after error message shown for duplicate rename");
      Assert.AreEqual(1, View.FocusMainGridCellCount,
        "FocusMainGridCellCount after error message shown for duplicate rename");
      Assert.AreEqual(0, View.FocusMainGridCellColumnIndex,
        "FocusMainGridCellColumnIndex after error message shown for duplicate rename");
      Assert.AreEqual(1, View.FocusMainGridCellRowIndex,
        "FocusMainGridCellRowIndex after error message shown for duplicate rename");
      Assert.AreEqual(name2, editor[1].Name,
        "Original name restored after error message shown for duplicate rename");
      Assert.AreEqual(1, View.RestoreMainGridCurrentRowCellErrorValueCount,
        "RestoreMainGridCurrentRowCellErrorValueCount after error message shown for duplicate rename");
      Assert.AreEqual(1, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after error message shown for duplicate rename");
      // For unknown reason, the the error handling is set up,
      // this event gets raise twice if there's a cell edit error,
      // as in the case of this rename,
      // the second time with a null exception.
      // So check that this is allowed for and has no effect.
      Controller.OnMainGridDataError(null);
      Assert.AreEqual(1, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after null error");
      // Check that an exception of an unsupported type rethrown
      Assert.Throws<InvalidOperationException>(
        () => Controller.OnMainGridDataError(new InvalidOperationException()),
        "Unsupported exception type");
      // Disallow insert with duplicate name
      editor.AddNew();
      Controller.OnMainGridRowEnter(2); // Go to insertion row
      editor[2].Name = name1;
      Assert.AreEqual(2, View.OnRowAddedOrDeletedCount,
        "OnRowAddedOrDeletedCount unchanged after duplicate insert");
      Assert.AreEqual(3, editor.Count,
        "editor.Count before error message shown for duplicate insert");
      Controller.OnMainGridRowValidated(2);
      Assert.AreEqual(2, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after error message shown for duplicate insert");
      Assert.AreEqual(1, View.MakeMainGridInsertionRowCurrentCount,
        "MakeMainGridInsertionRowCurrentCount after error message shown for duplicate insert");
      // When the insertion error message was shown,
      // focus was forced back to the error row,
      // now no longer the insertion row,
      // in EditorController.ShowDatabaseUpdateError.
      // That would raise the EditorView.MainGridOnRowEnter event,
      // which we have to simulate here.
      Controller.OnMainGridRowEnter(2);
      // Then the user opted to cancel out of adding the new row
      // rather than fixing it so that the add would work.
      // That would raise the EditorView.MainRowValidated event, 
      // even though nothing has changed.
      // We have to simulate that here to test to test
      // that the unwanted new row would get removed from the grid
      // (if there was a real grid).
      Controller.OnMainGridRowValidated(2);
      Assert.AreEqual(3, View.OnRowAddedOrDeletedCount,
        "OnRowAddedOrDeletedCount after cancel from insert error row");
      Assert.AreEqual(2, editor.Count,
        "editor.Count after cancel from insert error row");
      // Delete the second item
      Controller.OnMainGridRowEnter(1);
      Controller.OnMainGridRowRemoved(1);
      Assert.AreEqual(4, View.OnRowAddedOrDeletedCount,
        "OnRowAddedOrDeletedCount after delete");
      Controller.FetchData(); // Refresh grid
      editor.SetBindingList(Controller.MainBindingList);
      Assert.AreEqual(1, editor.Count, "editor.Count after FetchData #3");
    }

    [Test]
    public void ErrorOnDelete() {
      var editor = new TestEditor<Location, NotablyNamedBindingItem<Location>>();
      Session.BeginUpdate();
      try {
        Data.AddEventTypesPersisted(1, Session);
        Data.AddLocationsPersisted(2, Session);
        Data.AddEventsPersisted(3, Session, Data.Locations[1]);
      } finally {
        Session.Commit();
      }
      // The second Location cannot be deleted because it is a parent of 3 child Events.
      Controller = CreateTestEditorController(typeof(LocationList));
      //Controller.CreateEntityListData(typeof(LocationList), (IList)Data.Locations);
      Controller.FetchData(); // Populate grid
      editor.SetBindingList(Controller.MainBindingList);
      editor.AddNew(); // Show data load is complete.  Otherwise delete won't work.
      Controller.OnMainGridRowEnter(1);
      Controller.OnMainGridRowRemoved(1);
      Assert.AreEqual(1, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after error message shown for disallowed delete");
      Assert.AreEqual(1, View.SelectCurrentRowOnlyCount,
        "SelectCurrentRowOnlyCount after error message shown for disallowed delete");
      Controller.OnMainGridRowEnter(1);
      Controller.TestUnsupportedLastChangeAction = true;
      Assert.Throws<NotSupportedException>(() => Controller.OnMainGridRowRemoved(1),
        "Unsupported last change action");
    }

    [Test]
    public void GetColumnDisplayName() {
      Session.BeginUpdate();
      Data.AddNewslettersPersisted(1, Session);
      Session.Commit();
      Controller = CreateTestEditorController(typeof(NewsletterList));
      Controller.FetchData(); // Populate grid
      Assert.AreEqual("URL", Controller.GetColumnDisplayName("Url"));
    }

    [Test]
    public void GridSplitterDistance() {
      const int distance = 123;
      Controller = CreateTestEditorController(typeof(EventList));
      Controller.GridSplitterDistance = distance;
      Assert.AreEqual(distance, Controller.GridSplitterDistance);
    }

    [Test]
    public void SetParent() {
      Session.BeginUpdate();
      try {
        Data.AddEventTypesPersisted(1, Session);
        Data.AddLocationsPersisted(1, Session);
        Data.AddSeriesPersisted(1, Session);
        Data.AddEventsPersisted(1, Session);
      } finally {
        Session.Commit();
      }
      Controller = CreateTestEditorController(typeof(EventList));
      Controller.FetchData(); // Populate grid
      Assert.IsTrue(Controller.DoesColumnReferenceAnotherEntity("Series"),
        "DoesColumnReferenceAnotherEntity");
      var editor = new TestEditor<Event, EventBindingItem>(Controller.MainBindingList);
      string selectedSeriesName = Data.Series[0].Name;
      var selectedSeries = Data.Series[0];
      var selectedItem =
        new KeyValuePair<string, IEntity>(selectedSeriesName, selectedSeries);
      Controller.SetParent(0, "Series", selectedItem);
      editor[0].Series = selectedSeriesName;
      Assert.AreSame(selectedSeries, ((Event)Controller.GetMainList()[0]).Series,
        "Series");
    }

    [Test]
    public void ShowWarningMessage() {
      Controller = CreateTestEditorController(typeof(EventList));
      Controller.ShowWarningMessage("Warning! Warning!");
      Assert.AreEqual(1, View.ShowWarningMessageCount);
    }

    [NotNull]
    private TestEditorController CreateTestEditorController([NotNull] Type mainListType) {
      return new TestEditorController(View, mainListType, QueryHelper, Session);
    }
  }
}