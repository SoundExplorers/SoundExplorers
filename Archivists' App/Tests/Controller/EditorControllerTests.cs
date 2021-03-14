using System;
using System.Data;
using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests.Controller {
  [TestFixture]
  public class EditorControllerTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      Data = new TestData(QueryHelper);
      Session = new TestSession();
      MainGrid = new MockMainGrid();
      ParentGrid = new MockParentGrid();
      View = new MockEditorView(MainGrid, ParentGrid);
    }

    [TearDown]
    public void TearDown() {
      Session.DeleteDatabaseFolderIfExists();
    }

    private TestEditorController Controller { get; set; } = null!;
    private TestData Data { get; set; } = null!;
    private MockMainGrid MainGrid { get; set; } = null!;
    private TestMainGridController MainGridController { get; set; } = null!;
    private MockParentGrid ParentGrid { get; set; } = null!;
    private TestParentGridController ParentGridController { get; set; } = null!;
    private QueryHelper QueryHelper { get; set; } = null!;
    private TestSession Session { get; set; } = null!;
    private MockEditorView View { get; set; } = null!;

    [Test]
    public void AddEvents() {
      var event1Date = DateTime.Parse("2020/03/01");
      var event2Date = event1Date.AddDays(1);
      Session.BeginUpdate();
      try {
        Data.AddLocationsPersisted(1, Session);
        Data.AddNewslettersPersisted(2, Session);
        Data.AddSeriesPersisted(2, Session);
      } finally {
        Session.Commit();
      }
      var defaultNewsletter = Data.Newsletters[0];
      var validLocation = Data.Locations[0];
      string validLocationName = validLocation.Name!;
      var validNewsletter = Data.Newsletters[1];
      var validNewsletterDate = validNewsletter.Date;
      var validSeries = Data.Series[1];
      string validSeriesName = validSeries.Name!;
      Assert.IsNotNull(validLocationName, "validLocationName");
      Assert.IsNotNull(validSeriesName, "validSeriesName");
      CreateControllers(typeof(EventList));
      Controller.Populate(); // Show an empty grid
      // No need to make last row current when there is only the new row.
      Assert.AreEqual(0, MainGrid.MakeRowCurrentCount,
        "MakeRowCurrentCount after Populate");
      Assert.AreEqual(1, View.SetMouseCursorToDefaultCount,
        "SetMouseCursorToDefaultCount after Populate");
      var bindingList =
        (TypedBindingList<Event, EventBindingItem>)Controller.MainList.BindingList!;
      MainGridController.CreateAndGoToNewRow();
      bindingList[0].Date = event1Date;
      bindingList[0].Location = validLocationName;
      // Newsletter
      bindingList[0].Newsletter = validNewsletterDate;
      bindingList[0].Series = validSeriesName;
      MainGridController.OnRowValidated(0);
      Assert.AreEqual(1, validLocation.Events.Count, "Events.Count after 1st add");
      Session.BeginRead();
      var event1 = validLocation.Events[0];
      Session.Commit();
      Assert.AreSame(validLocation, event1.Location, "event1.Location");
      Assert.AreSame(validNewsletter, event1.Newsletter, "event1.Newsletter");
      Assert.AreSame(validSeries, event1.Series, "event1.Series");
      MainGridController.CreateAndGoToNewRow();
      bindingList[1].Date = event2Date;
      bindingList[1].Location = validLocationName;
      // Test that the user can reset the newsletter date to the special date
      // that indicated that the event's newsletter is to be unassigned.
      bindingList[1].Newsletter = EntityBase.DefaultDate;
      MainGridController.OnRowValidated(1);
      Assert.AreEqual(2, validLocation.Events.Count, "Events.Count after 2nd add");
      Session.BeginRead();
      var event2 = validLocation.Events[1];
      Session.Commit();
      Assert.AreSame(defaultNewsletter, event2.Newsletter, "event2.Newsletter");
    }

    [Test]
    public void CancelInsertionIfClosingWindow() {
      const string name = "Boogie woogie";
      CreateControllers(typeof(GenreList));
      var mainController = Controller.MainController;
      Controller.Populate(); // Show an empty grid
      var mainList = Controller.MainList;
      var bindingList =
        (TypedBindingList<Genre, NamedBindingItem<Genre>>)Controller.MainList
          .BindingList!;
      MainGridController.CreateAndGoToNewRow();
      bindingList[0].Name = name;
      Controller.IsClosing = true;
      // Called automatically when editor window is closing
      MainGridController.OnRowValidated(0);
      Assert.AreEqual(0, mainList.Count, "Count on database when editor window closing");
      Controller.IsClosing = false;
      mainController.IsClosing = true;
      // Called automatically when main window is closing
      MainGridController.OnRowValidated(0);
      Assert.AreEqual(0, mainList.Count, "Count on database when main window closing");
      mainController.IsClosing = false;
      MainGridController.OnRowValidated(0);
      Assert.AreEqual(1, mainList.Count,
        "Count on database when a window is not closing");
    }

    [Test]
    public void CloseChildEditorIfNoParents() {
      CreateControllers(typeof(SetList));
      Controller.Populate(); // Populate grid
      Assert.AreEqual(1, View.CloseCount, "CloseCount");
      Assert.AreEqual(1, View.ShowErrorMessageCount, "ShowErrorMessageCount");
      Assert.AreEqual(
        "The Set editor cannot be used yet because the Event table is empty.",
        View.LastErrorMessage, "LastErrorMessage");
    }

    [Test]
    public void DeleteEvent() {
      AddDataForEventList();
      CreateControllers(typeof(EventList));
      Controller.Populate(); // Populate grid
      Assert.AreEqual(3, Controller.MainList.Count, "Count after Populate");
      MainGridController.OnRowRemoved(1);
      Assert.AreEqual(1, MainGrid.OnRowAddedOrDeletedCount, "OnRowAddedOrDeletedCount");
      Assert.AreEqual(2, Controller.MainList.Count, "Count after delete");
    }

    [Test]
    public void DeleteLocation() {
      Session.BeginUpdate();
      Data.AddLocationsPersisted(3, Session);
      Session.Commit();
      CreateControllers(typeof(LocationList));
      Controller.Populate(); // Populate grid
      Assert.AreEqual(3, Controller.MainList.Count, "Count after Populate");
      MainGridController.OnRowRemoved(1);
      Assert.AreEqual(1, MainGrid.OnRowAddedOrDeletedCount, "OnRowAddedOrDeletedCount");
      Assert.AreEqual(2, Controller.MainList.Count, "Count after delete");
    }

    [Test]
    public void DeleteSet() {
      AddDataForSetList();
      CreateControllers(typeof(SetList));
      Controller.Populate(); // Populate parent Event and child Set grids
      // The last Event parent's Sets should be in the main grid.
      Assert.AreEqual(5, Controller.MainList.Count, "Count after Populate");
      MainGridController.OnRowRemoved(1);
      Assert.AreEqual(1, MainGrid.OnRowAddedOrDeletedCount, "OnRowAddedOrDeletedCount");
      Assert.AreEqual(4, Controller.MainList.Count, "Count after delete");
    }

    [Test]
    public void DisallowAddDuplicateKey() {
      Session.BeginUpdate();
      try {
        Data.AddGenresPersisted(1, Session);
      } finally {
        Session.Commit();
      }
      CreateControllers(typeof(GenreList));
      Controller.Populate();
      Assert.AreEqual(1, MainGrid.MakeRowCurrentCount,
        "MakeRowCurrentCount after Populate");
      Assert.AreEqual(1, MainGrid.CurrentRowIndex,
        "CurrentRowIndex after Populate");
      var bindingList =
        (TypedBindingList<Genre, NamedBindingItem<Genre>>)Controller.MainList
          .BindingList!;
      bindingList[1].Name = bindingList[0].Name;
      MainGridController.OnRowValidated(1);
      Assert.AreEqual(1, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after error message shown for duplicate insert");
      Assert.AreEqual(3, MainGrid.MakeRowCurrentCount,
        "MakeRowCurrentCount after error message shown for duplicate insert");
      Assert.AreEqual(1, MainGrid.CurrentRowIndex,
        "CurrentRowIndex after error message shown for duplicate insert");
      Assert.AreEqual(bindingList[0].Name, bindingList[1].Name,
        "Duplicate Name restored to insertion row for correction or edit cancellation after message shown");
    }

    [Test]
    public void DisallowAddWithoutIdentifyingParent() {
      CreateControllers(typeof(EventList));
      Controller.Populate(); // Show an empty grid
      var bindingList =
        (TypedBindingList<Event, EventBindingItem>)Controller.MainList.BindingList!;
      MainGridController.CreateAndGoToNewRow();
      var event1Date = DateTime.Parse("2020/03/01");
      bindingList[0].Date = event1Date;
      MainGridController.OnRowValidated(0);
      Assert.AreEqual(1, View.ShowErrorMessageCount, "ShowErrorMessageCount");
      Assert.AreEqual(
        "Event '2020/03/01' cannot be added because its Location has not been specified.",
        View.LastErrorMessage, "LastErrorMessage");
    }

    [Test]
    public void Edit() {
      const string name1 = "Auntie";
      const string name2 = "Uncle";
      CreateControllers(typeof(LocationList));
      Assert.IsFalse(Controller.IsParentGridToBeShown, "IsParentGridToBeShown");
      Assert.IsFalse(MainGrid.Focused, "MainGrid.Focused initially");
      Controller.Populate(); // The grid will be empty initially
      Assert.AreEqual("Location", MainGridController.TableName, "Main TableName");
      var bindingList =
        (TypedBindingList<Location, NotablyNamedBindingItem<Location>>)Controller.MainList
          .BindingList!;
      MainGridController.CreateAndGoToNewRow();
      bindingList[0].Name = name1;
      bindingList[0].Notes = "Disestablishmentarianism";
      MainGridController.OnRowValidated(0);
      MainGridController.CreateAndGoToNewRow();
      bindingList[1].Name = name2;
      bindingList[1].Notes = "Bob";
      MainGridController.OnRowValidated(1);
      Assert.AreEqual(2, MainGrid.OnRowAddedOrDeletedCount, "OnRowAddedOrDeletedCount");
      MainGrid.Focused = false;
      Controller.Populate(); // Refresh grid
      bindingList =
        (TypedBindingList<Location, NotablyNamedBindingItem<Location>>)Controller.MainList
          .BindingList!;
      Assert.AreEqual(3, bindingList.Count, "bindingList.Count after Refresh");
      MainGridController.OnRowEnter(1);
      // Disallow rename to duplicate
      var exception = Assert.Catch<DuplicateNameException>(
        () => bindingList[1].Name = name1,
        "Rename name should have thrown DuplicateNameException.");
      Assert.AreEqual(name1, bindingList[1].Name,
        "Still duplicate name before error message shown for duplicate rename");
      MainGridController.OnCellEditException(1, "Name", exception);
      Assert.AreEqual(1, MainGrid.MakeCellCurrentCount,
        "MakeCellCurrentCount after error message shown for duplicate rename");
      Assert.AreEqual(0, MainGrid.MakeCellCurrentColumnIndex,
        "MakeCellCurrentColumnIndex after error message shown for duplicate rename");
      Assert.AreEqual(1, MainGrid.CurrentRowIndex,
        "MakeCellCurrentRowIndex after error message shown for duplicate rename");
      Assert.AreEqual(name2, bindingList[1].Name,
        "Original name restored after error message shown for duplicate rename");
      Assert.AreEqual(1, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after error message shown for duplicate rename");
      // For unknown reason, the the error handling is set up,
      // this event gets raise twice if there's a cell edit error,
      // as in the case of this rename,
      // the second time with a null exception.
      // So check that this is allowed for and has no effect.
      MainGridController.OnCellEditException(1, "Name", null);
      Assert.AreEqual(1, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after null error");
      // Check that an exception of an unsupported type is rethrown
      Assert.Throws<InvalidOperationException>(
        () => MainGridController.OnCellEditException(1, "Name",
          new InvalidOperationException()),
        "Unsupported exception type");
    }

    [Test]
    public void ErrorOnDelete() {
      AddDataForEventList();
      Session.BeginUpdate();
      Data.AddEventsPersisted(3, Session, Data.Locations[1]);
      Session.Commit();
      // The second Location cannot be deleted because it is a parent of 3 child Events.
      CreateControllers(typeof(LocationList));
      Controller.Populate(); // Populate grid
      MainGridController.CreateAndGoToNewRow();
      MainGridController.OnRowEnter(1);
      MainGridController.OnRowRemoved(1);
      Assert.AreEqual(1, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after error message shown for disallowed delete");
      Assert.AreEqual(1, MainGrid.SelectCurrentRowOnlyCount,
        "SelectCurrentRowOnlyCount after error message shown for disallowed delete");
      MainGridController.OnRowEnter(1);
      MainGridController.TestUnsupportedLastChangeAction = true;
      Assert.Throws<NotSupportedException>(() => MainGridController.OnRowRemoved(1),
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
      CreateControllers(typeof(EventList));
      Controller.Populate(); // Populate grid
      Assert.AreEqual(0, MainGridController.FirstVisibleColumnIndex,
        "FirstVisibleColumnIndex");
      var newsletterColumn = MainGridController.GetBindingColumn("Newsletter");
      Assert.IsTrue(newsletterColumn.ReferencesAnotherEntity,
        "Newsletter ReferencesAnotherEntity");
      Assert.AreEqual(typeof(DateTime), newsletterColumn.ValueType,
        "Newsletter ValueType");
      var bindingList =
        (TypedBindingList<Event, EventBindingItem>)Controller.MainList.BindingList!;
      // Newsletter
      var selectedNewsletter = Data.Newsletters[0];
      var selectedNewsletterDate = selectedNewsletter.Date;
      MainGridController.OnRowEnter(0);
      bindingList[0].Newsletter = selectedNewsletterDate;
      // MainGridController.SetComboBoxCellValue(0, "Newsletter", selectedNewsletterDate);
      Assert.AreEqual(0, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after valid Newsletter selection");
      Assert.AreEqual(selectedNewsletterDate, bindingList[0].Newsletter,
        "Newsletter in editor after valid Newsletter selection");
      Assert.AreSame(selectedNewsletter, ToEvent(Controller.MainList[0]!).Newsletter,
        "Newsletter entity after valid Newsletter selection");
      var notFoundDate = DateTime.Parse("2345/12/31");
      MainGridController.OnRowEnter(0);
      var exception = Assert.Catch(
        () => bindingList[0].Newsletter = notFoundDate, 
        "Not found Newsletter disallowed");
      Assert.IsInstanceOf<RowNotInTableException>(exception,
        "Exception on not-found Newsletter pasted");
      Assert.AreEqual(0, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after not-found Newsletter pasted but before OnCellEditException");
      MainGridController.OnCellEditException(0, "Newsletter", exception);
      Assert.AreEqual(1, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after not-found Newsletter pasted and after OnCellEditException");
      Assert.AreEqual("Newsletter not found: '31 Dec 2345'",
        View.LastErrorMessage,
        "LastErrorMessage after not-found Newsletter pasted");
      Assert.AreEqual(selectedNewsletterDate, bindingList[0].Newsletter,
        "Newsletter in editor after not-found Newsletter pasted");
      Assert.AreSame(selectedNewsletter, ToEvent(Controller.MainList[0]!).Newsletter,
        "Newsletter entity after not-found Newsletter pasted");
      // Series
      var selectedSeries = Data.Series[0];
      string selectedSeriesName = selectedSeries.Name!;
      Assert.IsNotNull(selectedSeriesName, "selectedSeriesName");
      MainGridController.OnRowEnter(0);
      bindingList[0].Series = selectedSeriesName;
      Assert.AreEqual(1, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after valid Series selection");
      Assert.AreEqual(selectedSeriesName, bindingList[0].Series,
        "Series in editor after valid Series selection");
      Assert.AreSame(selectedSeries, ToEvent(Controller.MainList[0]!).Series,
        "Series entity after valid Series selection");
      const string notFoundName = "Not-Found Name";
      MainGridController.OnRowEnter(0);
      exception = Assert.Catch(
        () => bindingList[0].Series = notFoundName, 
        "Not found Series disallowed");
      Assert.IsInstanceOf<RowNotInTableException>(exception,
        "Exception on not-found Series pasted");
      Assert.AreEqual(1, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after not-found Series pasted but before OnCellEditException");
      MainGridController.OnCellEditException(0, "Series", exception);
      Assert.AreEqual(2, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after not-found Series pasted and after OnCellEditException");
      Assert.AreEqual("Series not found: 'Not-Found Name'",
        View.LastErrorMessage,
        "LastErrorMessage after not-found Series pasted");
      Assert.AreEqual(selectedSeriesName, bindingList[0].Series,
        "Series in editor after not-found Series pasted");
      Assert.AreSame(selectedSeries, ToEvent(Controller.MainList[0]!).Series,
        "Series entity after not-found Series pasted");
    }

    [Test]
    public void FixMainGridCurrentRowIndexOnRefocus() {
      AddDataForSetList();
      CreateControllers(typeof(SetList));
      ParentGrid.Focus();
      Assert.AreEqual(1, View.SetMouseCursorToDefaultCount,
        "SetMouseCursorToDefaultCount after focusing parent grid");
      Controller.Populate(); // Populate parent and main grids
      Assert.IsTrue(ParentGrid.Focused, "ParentGrid.Focused after Populate");
      Assert.AreEqual(2, View.SetMouseCursorToDefaultCount,
        "SetMouseCursorToDefaultCount after Populate");
      MainGrid.Focus();
      Assert.AreEqual(3, View.SetMouseCursorToDefaultCount,
        "SetMouseCursorToDefaultCount after focusing main grid");
      MainGridController.CreateAndGoToNewRow();
      int mainGridNewRowIndex = MainGrid.CurrentRowIndex;
      ParentGrid.Focus();
      // Simulate the problem that occurs when switching focus from the main grid to the
      // parent grid. If the main grid's current row is the new row before focusing the
      // parent grid, then, on focusing the parent grid, the new row is removed, so the
      // main grid's last existing row becomes its current row.
      MainGrid.MakeRowCurrent(mainGridNewRowIndex - 1);
      Assert.AreEqual(mainGridNewRowIndex - 1, MainGrid.CurrentRowIndex,
        "Main grid current row index after focus switched back to parent grid");
      // Now test the fix. When focus is switched back to the main grid, we want to
      // restore currency to the new row.
      MainGrid.Focus();
      Assert.AreEqual(mainGridNewRowIndex, MainGrid.CurrentRowIndex,
        "Main grid current row index after focus switched back to main grid");
      Assert.IsTrue(ParentGrid.Enabled,
        "Parent grid enabled after focus switched back to main grid");
    }

    [Test]
    public void FocusCurrentGrid() {
      CreateControllers(typeof(SetList));
      View.CurrentGrid = null;
      Assert.IsFalse(ParentGrid.Focused, "ParentGrid.Focused before #1");
      Controller.FocusCurrentGrid();
      Assert.IsTrue(ParentGrid.Focused, "ParentGrid.Focused after #1");
      View.CurrentGrid = MainGrid;
      Assert.IsFalse(MainGrid.Focused, "MainGrid.Focused before #2");
      Controller.FocusCurrentGrid();
      Assert.IsTrue(MainGrid.Focused, "MainGrid.Focused after #2");
    }

    [Test]
    public void FocusGrids() {
      CreateControllers(typeof(SetList));
      ParentGrid.Focus();
      Assert.IsFalse(MainGrid.Focused,
        "MainGrid.Focused after ParentGrid.Focus");
      Assert.AreEqual(1, MainGrid.CellColorScheme.InvertCount,
        "MainGrid.CellColorScheme.InvertCount after ParentGrid.Focus");
      Assert.AreEqual(1, MainGrid.DisableCount,
        "MainGrid.DisableCount after ParentGrid.Focus");
      Assert.AreEqual(1, MainGrid.EnableCount,
        "MainGrid.EnableCount after ParentGrid.Focus");
      Assert.IsTrue(MainGrid.Enabled, "MainGrid.Enabled after ParentGrid.Focus");
      Assert.IsTrue(ParentGrid.Focused,
        "ParentGrid.Focused after ParentGrid.Focus");
      Assert.AreEqual(1, ParentGrid.CellColorScheme.RestoreToDefaultCount,
        "ParentGrid.CellColorScheme.RestoreToDefaultCount after ParentGrid.Focus");
      Assert.AreEqual(1, View.SetMouseCursorToWaitCount,
        "SetMouseCursorToWaitCount after ParentGrid.Focus");
      Assert.AreEqual(1, View.SetMouseCursorToDefaultCount,
        "SetMouseCursorToDefaultCount after ParentGrid.Focus");
      Controller.FocusUnfocusedGridIfAny();
      Assert.IsTrue(MainGrid.Focused,
        "MainGrid.Focused after FocusUnfocusedGridIfAny #1");
      Assert.AreEqual(1, MainGrid.CellColorScheme.RestoreToDefaultCount,
        "MainGrid.CellColorScheme.RestoreToDefaultCount after FocusUnfocusedGridIfAny #1");
      Assert.IsFalse(ParentGrid.Focused,
        "ParentGrid.Focused after FocusUnfocusedGridIfAny #1");
      Assert.AreEqual(1, ParentGrid.CellColorScheme.InvertCount,
        "ParentGrid.CellColorScheme.InvertCount after FocusUnfocusedGridIfAny #1");
      Controller.FocusUnfocusedGridIfAny();
      Assert.IsFalse(MainGrid.Focused,
        "MainGrid.Focused after FocusUnfocusedGridIfAny #2");
      Assert.AreEqual(2, MainGrid.CellColorScheme.InvertCount,
        "MainGrid.CellColorScheme.InvertCount after FocusUnfocusedGridIfAny #2");
      Assert.IsTrue(ParentGrid.Focused,
        "ParentGrid.Focused after FocusUnfocusedGridIfAny #2");
      Assert.AreEqual(2, ParentGrid.CellColorScheme.RestoreToDefaultCount,
        "ParentGrid.CellColorScheme.RestoreToDefaultCount after FocusUnfocusedGridIfAny #2");
      Controller.FocusUnfocusedGridIfAny();
      Assert.IsTrue(MainGrid.Focused,
        "MainGrid.Focused after FocusUnfocusedGridIfAny #3");
    }

    [Test]
    public void FormatExceptionOnInsert() {
      Session.BeginUpdate();
      try {
        Data.AddNewslettersPersisted(3, Session);
      } finally {
        Session.Commit();
      }
      CreateControllers(typeof(NewsletterList));
      Controller.Populate(); // Populate grid
      MainGridController.CreateAndGoToNewRow();
      var exception = new FormatException("Potato is not a valid value for DateTime.");
      MainGridController.OnCellEditException(3, "Date", exception);
      Assert.AreEqual(1, View.ShowErrorMessageCount, "ShowErrorMessageCount");
      Assert.AreEqual("Invalid Date:\r\nPotato is not a valid value for DateTime.", 
        View.LastErrorMessage, "LastErrorMessage");
    }

    [Test]
    public void FormatExceptionOnUpdate() {
      AddDataForEventList();
      CreateControllers(typeof(EventList));
      Controller.Populate(); // Populate grid
      var bindingList =
        (TypedBindingList<Event, EventBindingItem>)Controller.MainList.BindingList!;
      MainGridController.OnRowEnter(2);
      string changedEventType = Data.EventTypes[1].Name;
      string changedLocation = Data.Locations[1].Name;
      var changedNewsletter = Data.Newsletters[0].Date;
      const string changedNotes = "Changed notes";
      string changedSeries = Data.Series[0].Name;
      bindingList[2].EventType = changedEventType;
      bindingList[2].Location = changedLocation;
      bindingList[2].Newsletter = changedNewsletter;
      bindingList[2].Notes = changedNotes;
      bindingList[2].Series = changedSeries;
      // Simulate pasting text into the Date cell.
      var exception = new FormatException("Potato is not a valid value for DateTime.");
      MainGridController.OnCellEditException(2, "Date", exception);
      Assert.AreEqual(1, View.ShowErrorMessageCount, "ShowErrorMessageCount");
      Assert.AreEqual("Invalid Date:\r\nPotato is not a valid value for DateTime.", 
        View.LastErrorMessage, "LastErrorMessage");
      Assert.AreEqual(changedEventType, bindingList[2].EventType, "EventType");
      Assert.AreEqual(changedLocation, bindingList[2].Location, "Location");
      Assert.AreEqual(changedNewsletter, bindingList[2].Newsletter, "Newsletter");
      Assert.AreEqual(changedNotes, bindingList[2].Notes, "Notes");
      Assert.AreEqual(changedSeries, bindingList[2].Series, "Series");
    }

    [Test]
    public void GetColumnDisplayName() {
      Session.BeginUpdate();
      Data.AddNewslettersPersisted(1, Session);
      Session.Commit();
      CreateControllers(typeof(NewsletterList));
      Controller.Populate(); // Populate grid
      Assert.AreEqual("Date", MainGridController.GetBindingColumn("Date").DisplayName,
        "Date");
      Assert.AreEqual("URL", MainGridController.GetBindingColumn("Url").DisplayName,
        "Url");
    }

    [Test]
    public void GridSplitterDistance() {
      const int distance = 123;
      CreateControllers(typeof(EventList));
      Controller.Populate();
      Controller.GridSplitterDistance = distance;
      Assert.AreEqual(distance, Controller.GridSplitterDistance);
    }

    [Test]
    public void NonKeyUpdateError() {
      Session.BeginUpdate();
      Data.AddNewslettersPersisted(2, Session);
      Session.Commit();
      CreateControllers(typeof(NewsletterList));
      Controller.Populate(); // Populate grid
      var bindingList =
        (TypedBindingList<Newsletter, NewsletterBindingItem>)Controller.MainList
          .BindingList!;
      MainGridController.OnRowEnter(1);
      var exception = Assert.Catch<DatabaseUpdateErrorException>(
        () => bindingList[1].Url = bindingList[0].Url,
        "Rename Url should have thrown DatabaseUpdateErrorException.");
      MainGridController.OnCellEditException(1, "Url", exception);
      Assert.AreEqual(1, MainGrid.RestoreCurrentRowCellErrorValueCount,
        "RestoreCurrentRowCellErrorValueCount");
      Assert.AreEqual(1, MainGrid.EditCurrentCellCount,
        "EditCurrentCellCount");
    }

    [Test]
    public void OnParentGridRowEntered() {
      AddDataForSetList();
      CreateControllers(typeof(SetList));
      Assert.IsFalse(MainGrid.Focused, "MainGrid.Focused initially");
      Assert.IsFalse(ParentGrid.Focused, "ParentGrid.Focused initially");
      Assert.IsFalse(MainGridController.IsFixingFocus, "IsFixingFocus initially");
      MainGrid.Focus();
      Assert.AreEqual(1, View.SetMouseCursorToDefaultCount,
        "SetMouseCursorToDefaultCount after focusing main grid");
      Controller.Populate(); // Populate parent and main grids
      Assert.AreEqual("Event 2 of 2", View.StatusBarText,
        "StatusBarText after Populate");
      Assert.AreEqual(1, View.OnParentAndMainGridsShownAsyncCount,
        "OnParentAndMainGridsShownAsyncCount after Populate");
      Assert.IsFalse(MainGrid.Focused, "MainGrid.Focused after Populate");
      Assert.IsTrue(ParentGrid.Focused, "ParentGrid.Focused after Populate");
      Assert.AreEqual(2, MainGrid.CellColorScheme.InvertCount,
        "MainGrid.CellColorScheme.InvertCount after Populate");
      Assert.AreEqual(2, ParentGridController.BindingList.Count,
        "Parent list count after Populate");
      Assert.AreEqual(2, MainGridController.FirstVisibleColumnIndex,
        "Main grid FirstVisibleColumnIndex after Populate");
      Assert.IsFalse(MainGridController.GetBindingColumn("Date").IsVisible,
        "Is Date column to be shown?");
      Assert.IsTrue(MainGridController.GetBindingColumn("SetNo").IsVisible,
        "Is SetNo column to be shown?");
      Assert.AreEqual(6, MainGridController.BindingList.Count,
        "Main list count after Populate"); // Includes new row 
      Assert.IsTrue(MainGridController.IsFixingFocus, "IsFixingFocus after Populate");
      Assert.AreEqual(0, ParentGridController.FirstVisibleColumnIndex,
        "Main grid FirstVisibleColumnIndex after Populate");
      Assert.AreEqual(2, View.SetMouseCursorToDefaultCount,
        "SetMouseCursorToDefaultCount after Populate");
      // Emulate the unwanted internal logic of DataGridView that, following the end of
      // Populate, causes an extra validation of the first main grid row, even though the
      // parent grid is focused.  Without the workaround we are about to test, this
      // would switch focus to the main grid and make its first row current, neither of
      // which we want. See the documentation for MainGridController.IsFixingFocus. 
      MainGridController.OnRowValidated(0);
      // Confirm that the workaround was executed.
      Assert.IsFalse(MainGridController.IsFixingFocus,
        "IsFixingFocus after OnRowValidated");
      MainGrid.Focus();
      Assert.AreEqual("Set 6 of 6", View.StatusBarText,
        "StatusBarText when main grid focused");
      MainGridController.OnRowEnter(1);
      Assert.AreEqual("Set 2 of 6", View.StatusBarText,
        "StatusBarText when 2nd main grid row entered");
      ParentGrid.Focus();
      Assert.AreEqual("Event 2 of 2", View.StatusBarText,
        "StatusBarText after focusing parent grid");
      ParentGridController.OnRowEnter(0);
      Assert.AreEqual("Event 1 of 2", View.StatusBarText,
        "StatusBarText when 1st parent selected");
      Assert.AreEqual(4, MainGridController.BindingList.Count,
        "Main list count when 1st parent selected"); // Includes insertion row
      View.StatusBarText = string.Empty; // Simulates editor window deactivated.
      ParentGridController.OnWindowActivated();
      Assert.AreEqual("Event 1 of 2", View.StatusBarText,
        "StatusBarText when editor window reactivated");
    }

    [Test]
    public void RestoreValueOnUpdateError() {
      AddDataForPieceList();
      CreateControllers(typeof(PieceList));
      Controller.Populate(); // Populate grid
      Assert.AreEqual(5, Controller.MainList.Count, "Count after Populate");
      var bindingList =
        (TypedBindingList<Piece, PieceBindingItem>)Controller.MainList
          .BindingList!;
      // Leave insertion row, which will be current on Populate
      MainGridController.OnRowValidated(5);
      MainGridController.OnRowEnter(1);
      const string invalidDuration = "abc";
      var exception = Assert.Catch<DatabaseUpdateErrorException>(
        () => bindingList[1].Duration = invalidDuration,
        "Invalid Duration disallowed");
      MainGridController.OnCellEditException(1, "Duration", exception);
      Assert.AreEqual(1, MainGridController.ReplaceErrorBindingValueWithOriginalCount,
        "ReplaceErrorBindingValueWithOriginalCount");
      Assert.AreEqual(1, MainGrid.RestoreCurrentRowCellErrorValueCount,
        "RestoreCurrentRowCellErrorValueCount");
      Assert.AreEqual(1, MainGrid.EditCurrentCellCount,
        "EditCurrentCellCount");
    }

    [Test]
    public void ShowWarningMessage() {
      CreateControllers(typeof(NewsletterList));
      MainGridController.ShowWarningMessage("Warning! Warning!");
      Assert.AreEqual(1, View.ShowWarningMessageCount);
    }

    private void AddDataForEventList() {
      Session.BeginUpdate();
      Data.AddEventTypesPersisted(2, Session);
      Data.AddLocationsPersisted(2, Session);
      Data.AddNewslettersPersisted(1, Session);
      Data.AddSeriesPersisted(1, Session);
      Data.AddEventsPersisted(3, Session);
      Session.Commit();
    }

    private void AddDataForPieceList() {
      Session.BeginUpdate();
      Data.AddActsPersisted(1, Session);
      Data.AddSeriesPersisted(1, Session);
      Data.AddEventTypesPersisted(1, Session);
      Data.AddGenresPersisted(1, Session);
      Data.AddLocationsPersisted(1, Session);
      Data.AddNewslettersPersisted(1, Session);
      Data.AddEventsPersisted(1, Session);
      Data.AddSetsPersisted(2, Session);
      Data.AddPiecesPersisted(3, Session, Data.Sets[0]);
      Data.AddPiecesPersisted(5, Session, Data.Sets[1]);
      Session.Commit();
    }

    private void AddDataForSetList() {
      Session.BeginUpdate();
      Data.AddActsPersisted(1, Session);
      Data.AddSeriesPersisted(1, Session);
      Data.AddEventTypesPersisted(1, Session);
      Data.AddGenresPersisted(1, Session);
      Data.AddLocationsPersisted(1, Session);
      Data.AddNewslettersPersisted(1, Session);
      Data.AddEventsPersisted(2, Session);
      Data.AddSetsPersisted(3, Session, Data.Events[0]);
      Data.AddSetsPersisted(5, Session, Data.Events[1]);
      Session.Commit();
    }

    private void CreateControllers(Type mainListType) {
      Controller = new TestEditorController(mainListType, View, QueryHelper, Session);
      MainGridController = View.MainGridController =
        new TestMainGridController(MainGrid, Controller);
      MainGrid.SetController(MainGridController);
      if (Controller.IsParentGridToBeShown) {
        ParentGridController = new TestParentGridController(ParentGrid, Controller);
        ParentGrid.SetController(ParentGridController);
      }
    }

    private static Event ToEvent(object? value) {
      return (value as Event)!;
    }
  }
}