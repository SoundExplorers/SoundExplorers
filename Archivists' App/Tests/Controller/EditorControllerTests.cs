using System;
using System.Data;
using JetBrains.Annotations;
using NUnit.Framework;
using SoundExplorers.Controller;
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
    public void AddEvent() {
      var notFoundNewsletterDate = DateTime.Parse("2345/12/31");
      const string notFoundSeriesName = "Not-Found Name";
      Session.BeginUpdate();
      try {
        Data.AddLocationsPersisted(1, Session);
        Data.AddNewslettersPersisted(1, Session);
        Data.AddSeriesPersisted(1, Session);
      } finally {
        Session.Commit();
      }
      var validLocation = Data.Locations[0];
      string validLocationName = validLocation.Name;
      var validNewsletter = Data.Newsletters[0];
      var validNewsletterDate = validNewsletter.Date;
      var validSeries = Data.Series[0];
      string validSeriesName = validSeries.Name;
      Controller = CreateController(typeof(EventList));
      Controller.FetchData(); // Show an empty grid grid
      var editor = new TestEditor<Event, EventBindingItem>(
        Controller.MainBindingList);
      editor.AddNew(); // Create insertion row
      Controller.OnMainGridRowEnter(0); // Go to insertion row
      editor[0].Location = validLocationName;
      // Newsletter
      var comboBoxCellController =
        CreateComboBoxCellControllerWithItems("Newsletter");
      editor[0].Newsletter = notFoundNewsletterDate;
      comboBoxCellController.OnCellValueChanged(0, notFoundNewsletterDate);
      Assert.AreEqual(1, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after not-found Newsletter pasted");
      Assert.AreEqual("Newsletter not found: '31 Dec 2345'",
        View.LastErrorMessage,
        "LastErrorMessage after not-found Newsletter pasted");
      editor[0].Newsletter = validNewsletterDate;
      comboBoxCellController.OnCellValueChanged(0, validNewsletterDate);
      Assert.AreEqual(1, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after valid Newsletter pasted");
      // Series
      comboBoxCellController =
        CreateComboBoxCellControllerWithItems("Series");
      editor[0].Series = notFoundSeriesName;
      comboBoxCellController.OnCellValueChanged(0, notFoundSeriesName);
      Assert.AreEqual(2, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after not-found Series pasted");
      Assert.AreEqual("Series not found: 'Not-Found Name'", View.LastErrorMessage,
        "LastErrorMessage after not-found Series pasted");
      Assert.IsNotNull(validSeriesName, "validSeriesName");
      editor[0].Series = validSeriesName;
      comboBoxCellController.OnCellValueChanged(0, validSeriesName);
      Assert.AreEqual(2, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after valid Series pasted");
      Controller.OnMainGridRowValidated(0);
      Assert.AreEqual(1, validLocation.Events.Count, "Events.Count");
      var event1 = validLocation.Events[0];
      Assert.AreSame(validLocation, event1.Location, "Location");
      Assert.AreSame(validNewsletter, event1.Newsletter, "Newsletter");
      Assert.AreSame(validSeries, event1.Series, "Series");
    }

    [Test]
    public void Edit() {
      const string name1 = "Auntie";
      const string name2 = "Uncle";
      var editor = new TestEditor<Location, NotablyNamedBindingItem<Location>>();
      Controller = CreateController(typeof(LocationList));
      Assert.IsFalse(Controller.IsParentTableToBeShown, "IsParentTableToBeShown");
      Controller.FetchData(); // The grid will be empty initially
      Assert.AreEqual("Location", Controller.MainTableName, "MainTableName");
      editor.SetBindingList(Controller.MainBindingList);
      editor.AddNew(); // Create insertion row
      Controller.OnMainGridRowEnter(0); // Go to insertion row
      editor[0].Name = name1;
      editor[0].Notes = "Disestablishmentarianism";
      Controller.OnMainGridRowValidated(0);
      editor.AddNew(); // Create insertion row
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
      var exception = Assert.Catch<DatabaseUpdateErrorException>(
        () => editor[1].Name = name1,
        "Rename name should have thrown DatabaseUpdateErrorException.");
      Assert.AreEqual(name1, editor[1].Name,
        "Still duplicate name before error message shown for duplicate rename");
      Controller.OnExistingRowCellUpdateError(1, "Name", exception);
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
      Controller.OnExistingRowCellUpdateError(1, "Name", null);
      Assert.AreEqual(1, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after null error");
      // Check that an exception of an unsupported type is rethrown
      Assert.Throws<InvalidOperationException>(
        () => Controller.OnExistingRowCellUpdateError(1, "Name", new InvalidOperationException()),
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
      Controller = CreateController(typeof(LocationList));
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
    public void ExistingEventValidatePastedParents() {
      Session.BeginUpdate();
      try {
        Data.AddEventTypesPersisted(1, Session);
        Data.AddLocationsPersisted(1, Session);
        Data.AddNewslettersPersisted(1, Session);
        Data.AddSeriesPersisted(1, Session);
        Data.AddEventsPersisted(1, Session);
      } finally {
        Session.Commit();
      }
      Controller = CreateController(typeof(EventList));
      Controller.FetchData(); // Populate grid
      Assert.IsTrue(Controller.DoesColumnReferenceAnotherEntity("Newsletter"),
        "Newsletter DoesColumnReferenceAnotherEntity");
      var editor = new TestEditor<Event, EventBindingItem>(
        Controller.MainBindingList);
      // Newsletter
      var comboBoxCellController =
        CreateComboBoxCellControllerWithItems("Newsletter");
      var selectedNewsletter = Data.Newsletters[0];
      var selectedNewsletterDate = selectedNewsletter.Date;
      Controller.OnMainGridRowEnter(0);
      editor[0].Newsletter = selectedNewsletterDate;
      comboBoxCellController.OnCellValueChanged(0, selectedNewsletterDate);
      Assert.AreEqual(0, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after valid Newsletter selection");
      Assert.AreEqual(selectedNewsletterDate, editor[0].Newsletter,
        "Newsletter in editor after valid Newsletter selection");
      Assert.AreSame(selectedNewsletter, ((Event)Controller.GetMainList()[0]).Newsletter,
        "Newsletter entity after valid Newsletter selection");
      var notFoundDate = DateTime.Parse("2345/12/31");
      Controller.OnMainGridRowEnter(0);
      var exception = Assert.Catch(()=>editor[0].Newsletter = notFoundDate);
      Assert.IsInstanceOf<RowNotInTableException>(exception, 
        "Exception on not-found Newsletter pasted");
      // OnCellValueChanged only shows parent-not-found messages for insertions.
      // So it should have no effect here.
      comboBoxCellController.OnCellValueChanged(0, notFoundDate);
      Assert.AreEqual(0, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after not-found Newsletter pasted but before OnExistingRowCellUpdateError");
      Controller.OnExistingRowCellUpdateError(0, "Newsletter", exception);
      Assert.AreEqual(1, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after not-found Newsletter pasted and after OnExistingRowCellUpdateError");
      Assert.AreEqual("Newsletter not found: '31 Dec 2345'",
        View.LastErrorMessage,
        "LastErrorMessage after not-found Newsletter pasted");
      Assert.AreEqual(selectedNewsletterDate, editor[0].Newsletter,
        "Newsletter in editor after not-found Newsletter pasted");
      Assert.AreSame(selectedNewsletter, ((Event)Controller.GetMainList()[0]).Newsletter,
        "Newsletter entity after not-found Newsletter pasted");
      // Series
      comboBoxCellController =
        CreateComboBoxCellControllerWithItems("Series");
      var selectedSeries = Data.Series[0];
      string selectedSeriesName = selectedSeries.Name;
      Assert.IsNotNull(selectedSeriesName, "selectedSeriesName");
      Controller.OnMainGridRowEnter(0);
      editor[0].Series = selectedSeriesName;
      comboBoxCellController.OnCellValueChanged(0, selectedSeriesName);
      Assert.AreEqual(1, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after valid Series selection");
      Assert.AreEqual(selectedSeriesName, editor[0].Series,
        "Series in editor after valid Series selection");
      Assert.AreSame(selectedSeries, ((Event)Controller.GetMainList()[0]).Series,
        "Series entity after valid Series selection");
      const string notFoundName = "Not-Found Name";
      Controller.OnMainGridRowEnter(0);
      exception = Assert.Catch(()=>editor[0].Series = notFoundName);
      Assert.IsInstanceOf<RowNotInTableException>(exception, 
        "Exception on not-found Series pasted");
      // OnCellValueChanged only shows parent-not-found messages for insertions.
      // So it should have no effect here.
      comboBoxCellController.OnCellValueChanged(0, notFoundName);
      Assert.AreEqual(1, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after not-found Series pasted but before OnExistingRowCellUpdateError");
      Controller.OnExistingRowCellUpdateError(0, "Series", exception);
      Assert.AreEqual(2, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after not-found Series pasted and after OnExistingRowCellUpdateError");
      Assert.AreEqual("Series not found: 'Not-Found Name'", View.LastErrorMessage,
        "LastErrorMessage after not-found Series pasted");
      Assert.AreEqual(selectedSeriesName, editor[0].Series,
        "Series in editor after not-found Series pasted");
      Assert.AreSame(selectedSeries, ((Event)Controller.GetMainList()[0]).Series,
        "Series entity after not-found Series pasted");
    }

    [Test]
    public void FormatExceptionOnInsert() {
      Session.BeginUpdate();
      try {
        Data.AddNewslettersPersisted(3, Session);
      } finally {
        Session.Commit();
      }
      Controller = CreateController(typeof(NewsletterList));
      Controller.FetchData(); // Populate grid
      var editor = new TestEditor<Newsletter, NewsletterBindingItem>(
        Controller.MainBindingList);
      editor.AddNew(); // Create insertion row
      Controller.OnMainGridRowEnter(3); // Go to insertion row
      var exception = new FormatException("Potato is not a valid DateTime.");
      Controller.OnExistingRowCellUpdateError(3, "Date", exception);
      Assert.AreEqual(1, View.ShowErrorMessageCount, "ShowErrorMessageCount");
    }

    [Test]
    public void FormatExceptionOnUpdate() {
      Session.BeginUpdate();
      try {
        Data.AddEventTypesPersisted(2, Session);
        Data.AddLocationsPersisted(2, Session);
        Data.AddEventsPersisted(3, Session);
        Data.AddNewslettersPersisted(1, Session);
        Data.AddSeriesPersisted(1, Session);
      } finally {
        Session.Commit();
      }
      Controller = CreateController(typeof(EventList));
      Controller.FetchData(); // Populate grid
      var editor = new TestEditor<Event, EventBindingItem>(
        Controller.MainBindingList);
      Controller.OnMainGridRowEnter(2);
      string changedEventType = Data.EventTypes[1].Name;
      string changedLocation = Data.Locations[1].Name;
      var changedNewsletter = Data.Newsletters[0].Date;
      const string changedNotes = "Changed notes";
      string changedSeries = Data.Series[0].Name;
      editor[2].EventType = changedEventType;
      editor[2].Location = changedLocation;
      editor[2].Newsletter = changedNewsletter;
      editor[2].Notes = changedNotes;
      editor[2].Series = changedSeries;
      // Simulate pasting text into the Date cell.
      var exception = new FormatException("Potato is not a valid value for DateTime.");
      Controller.OnExistingRowCellUpdateError(2, "Date", exception);
      Assert.AreEqual(1, View.ShowErrorMessageCount, "ShowErrorMessageCount");
      Assert.AreEqual(changedEventType, editor[2].EventType, "EventType");
      Assert.AreEqual(changedLocation, editor[2].Location, "Location");
      Assert.AreEqual(changedNewsletter, editor[2].Newsletter, "Newsletter");
      Assert.AreEqual(changedNotes, editor[2].Notes, "Notes");
      Assert.AreEqual(changedSeries, editor[2].Series, "Series");
    }

    [Test]
    public void GetColumnDisplayName() {
      Session.BeginUpdate();
      Data.AddNewslettersPersisted(1, Session);
      Session.Commit();
      Controller = CreateController(typeof(NewsletterList));
      Controller.FetchData(); // Populate grid
      Assert.AreEqual("Date", Controller.GetColumnDisplayName("Date"), "Date");
      Assert.AreEqual("URL", Controller.GetColumnDisplayName("Url"), "Url");
    }

    [Test]
    public void GridSplitterDistance() {
      const int distance = 123;
      Controller = CreateController(typeof(EventList));
      Controller.GridSplitterDistance = distance;
      Assert.AreEqual(distance, Controller.GridSplitterDistance);
    }

    [Test]
    public void OnParentGridRowEntered() {
      Session.BeginUpdate();
      try {
        Data.AddEventTypesPersisted(1, Session);
        Data.AddGenresPersisted(1, Session);
        Data.AddLocationsPersisted(1, Session);
        Data.AddEventsPersisted(2, Session);
        Data.AddSetsPersisted(3, Session, Data.Events[0]);
        Data.AddSetsPersisted(5, Session, Data.Events[1]);
      } finally {
        Session.Commit();
      }
      Controller = CreateController(typeof(SetList));
      Controller.FetchData(); // Populate grid
      Assert.AreEqual(2, Controller.ParentBindingList?.Count, "Parent list count");
      Assert.AreEqual(3, Controller.MainBindingList?.Count, "Main list count initially");
      Controller.OnParentGridRowEntered(1);
      Assert.AreEqual(5, Controller.MainBindingList?.Count,
        "Main list count when 2nd parent selected");
    }

    [Test]
    public void ShowWarningMessage() {
      Controller = CreateController(typeof(EventList));
      Controller.ShowWarningMessage("Warning! Warning!");
      Assert.AreEqual(1, View.ShowWarningMessageCount);
    }

    [NotNull]
    private ComboBoxCellController CreateComboBoxCellControllerWithItems(
      [NotNull] string columnName) {
      var comboBoxCell = new MockView<ComboBoxCellController>();
      var comboBoxCellController =
        new ComboBoxCellController(comboBoxCell, Controller, columnName);
      comboBoxCellController.GetItems();
      return comboBoxCellController;
    }

    [NotNull]
    private TestEditorController CreateController([NotNull] Type mainListType) {
      return new TestEditorController(View, mainListType, QueryHelper, Session);
    }
  }
}