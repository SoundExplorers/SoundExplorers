using System;
using System.Data;
using JetBrains.Annotations;
using NUnit.Framework;
using SoundExplorers.Controller;
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
      View = new MockEditorView();
      MainGridController = View.MainGridController = new TestMainGridController(View);
      MainGrid = MainGridController.Grid;
      MainGrid.SetController(MainGridController);
    }

    [TearDown]
    public void TearDown() {
      Session.DeleteDatabaseFolderIfExists();
    }

    private TestData Data { get; set; }
    private MockMainGrid MainGrid { get; set; }
    private TestMainGridController MainGridController { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private TestSession Session { get; set; }
    private MockEditorView View { get; set; }

    [Test]
    public void AddEvents() {
      var event1Date = DateTime.Parse("2020/03/01");
      var event2Date = event1Date.AddDays(1);
      var notFoundNewsletterDate = DateTime.Parse("2345/12/31");
      const string notFoundSeriesName = "Not-Found Name";
      Session.BeginUpdate();
      try {
        Data.AddLocationsPersisted(1, Session);
        Data.AddNewslettersPersisted(2, Session);
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
      Assert.IsNotNull(validLocationName, "validLocationName");
      Assert.IsNotNull(validSeriesName, "validSeriesName");
      var controller = CreateController(typeof(EventList));
      controller.FetchData(); // Show an empty grid
      var bindingList =
        (TypedBindingList<Event, EventBindingItem>)controller.MainList.BindingList;
      MainGridController.CreateAndGoToInsertionRow();
      bindingList[0].Date = event1Date;
      MainGridController.SetComboBoxCellValue(0, "Location", validLocationName);
      // Newsletter
      MainGridController.SetComboBoxCellValue(0, "Newsletter", notFoundNewsletterDate);
      Assert.AreEqual(1, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after not-found Newsletter pasted");
      Assert.AreEqual("Invalid Newsletter:\r\nNewsletter not found: '31 Dec 2345'",
        View.LastErrorMessage,
        "LastErrorMessage after not-found Newsletter pasted");
      MainGridController.SetComboBoxCellValue(0, "Newsletter", validNewsletterDate);
      Assert.AreEqual(1, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after valid Newsletter pasted");
      // Series
      MainGridController.SetComboBoxCellValue(0, "Series", notFoundSeriesName);
      Assert.AreEqual(2, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after not-found Series pasted");
      Assert.AreEqual("Invalid Series:\r\nSeries not found: 'Not-Found Name'",
        View.LastErrorMessage,
        "LastErrorMessage after not-found Series pasted");
      MainGridController.SetComboBoxCellValue(0, "Series", validSeriesName);
      Assert.AreEqual(2, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after valid Series pasted");
      MainGridController.OnRowValidated(0);
      Assert.AreEqual(1, validLocation.Events.Count, "Events.Count after 1st add");
      var event1 = validLocation.Events[0];
      Assert.AreSame(validLocation, event1.Location, "event1.Location");
      Assert.AreSame(validNewsletter, event1.Newsletter, "event1.Newsletter");
      Assert.AreSame(validSeries, event1.Series, "event1.Series");
      MainGridController.CreateAndGoToInsertionRow();
      bindingList[1].Date = event2Date;
      MainGridController.SetComboBoxCellValue(1, "Location", validLocationName);
      // Test that the user can reset the newsletter date to the special date
      // that indicated that the event's newsletter is to be unassigned.
      MainGridController.SetComboBoxCellValue(1, "Newsletter", EntityBase.InitialDate);
      MainGridController.OnRowValidated(1);
      Assert.AreEqual(2, validLocation.Events.Count, "Events.Count after 2nd add");
      var event2 = validLocation.Events[1];
      Assert.IsNull(event2.Newsletter, "event2.Newsletter");
    }

    [Test]
    public void CancelInsertionIfClosingWindow() {
      const string name = "Boogie woogie";
      var controller = CreateController(typeof(GenreList));
      var mainController = controller.MainController;
      controller.FetchData(); // Show an empty grid
      var mainList = controller.MainList;
      var bindingList =
        (TypedBindingList<Genre, NamedBindingItem<Genre>>)controller.MainList.BindingList;
      MainGridController.CreateAndGoToInsertionRow();
      bindingList[0].Name = name;
      controller.IsClosing = true;
      // Called automatically when editor window is closing
      MainGridController.OnRowValidated(0);
      Assert.AreEqual(0, mainList.Count, "Count on database when editor window closing");
      controller.IsClosing = false;
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
    public void Delete() {
      Session.BeginUpdate();
      try {
        Data.AddLocationsPersisted(3, Session);
      } finally {
        Session.Commit();
      }
      // The second Location cannot be deleted because it is a parent of 3 child Events.
      var controller = CreateController(typeof(LocationList));
      controller.FetchData(); // Populate grid
      MainGridController.CreateAndGoToInsertionRow();
      MainGridController.OnRowRemoved(1);
      Assert.AreEqual(1, MainGrid.OnRowAddedOrDeletedCount, "OnRowAddedOrDeletedCount");
      Assert.AreEqual(2, controller.MainList.Count, "MainList.Count");
    }

    [Test]
    public void DisallowAddDuplicateKey() {
      Session.BeginUpdate();
      try {
        Data.AddGenresPersisted(1, Session);
      } finally {
        Session.Commit();
      }
      var controller = CreateController(typeof(GenreList));
      MainGridController.AutoValidate = true;
      controller.FetchData();
      var bindingList =
        (TypedBindingList<Genre, NamedBindingItem<Genre>>)controller.MainList.BindingList;
      MainGridController.CreateAndGoToInsertionRow();
      bindingList[1].Name = bindingList[0].Name;
      MainGridController.OnRowValidated(1);
      Assert.AreEqual(1, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after error message shown for duplicate insert");
      Assert.AreEqual(2, MainGrid.MakeRowCurrentCount,
        "MakeRowCurrentCount after error message shown for duplicate insert");
      Assert.AreEqual(1, MainGrid.MakeRowCurrentRowIndex,
        "MakeRowCurrentRowIndex after error message shown for duplicate insert");
    }

    [Test]
    public void DisallowAddWithoutIdentifyingParent() {
      var controller = CreateController(typeof(EventList));
      controller.FetchData(); // Show an empty grid
      var bindingList =
        (TypedBindingList<Event, EventBindingItem>)controller.MainList.BindingList;
      MainGridController.CreateAndGoToInsertionRow();
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
      var controller = CreateController(typeof(LocationList));
      Assert.IsFalse(controller.IsParentGridToBeShown, "IsParentGridToBeShown");
      controller.FetchData(); // The grid will be empty initially
      Assert.AreEqual("Location", MainGridController.TableName, "Main TableName");
      var bindingList =
        (TypedBindingList<Location, NotablyNamedBindingItem<Location>>)controller.MainList
          .BindingList;
      MainGridController.CreateAndGoToInsertionRow();
      bindingList[0].Name = name1;
      bindingList[0].Notes = "Disestablishmentarianism";
      MainGridController.OnRowValidated(0);
      MainGridController.CreateAndGoToInsertionRow();
      bindingList[1].Name = name2;
      bindingList[1].Notes = "Bob";
      MainGridController.OnRowValidated(1);
      Assert.AreEqual(2, MainGrid.OnRowAddedOrDeletedCount, "OnRowAddedOrDeletedCount");
      controller.FetchData(); // Refresh grid
      bindingList =
        (TypedBindingList<Location, NotablyNamedBindingItem<Location>>)controller.MainList
          .BindingList;
      Assert.AreEqual(2, bindingList.Count, "editor.Count after FetchData #2");
      MainGridController.OnRowEnter(1);
      // Disallow rename to duplicate
      var exception = Assert.Catch<DuplicateNameException>(
        () => bindingList[1].Name = name1,
        "Rename name should have thrown DuplicateNameException.");
      Assert.AreEqual(name1, bindingList[1].Name,
        "Still duplicate name before error message shown for duplicate rename");
      MainGridController.OnExistingRowCellUpdateError(1, "Name", exception);
      Assert.AreEqual(1, MainGrid.MakeCellCurrentCount,
        "MakeCellCurrentCount after error message shown for duplicate rename");
      Assert.AreEqual(0, MainGrid.MakeCellCurrentColumnIndex,
        "MakeCellCurrentColumnIndex after error message shown for duplicate rename");
      Assert.AreEqual(1, MainGrid.MakeCellCurrentRowIndex,
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
      MainGridController.OnExistingRowCellUpdateError(1, "Name", null);
      Assert.AreEqual(1, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after null error");
      // Check that an exception of an unsupported type is rethrown
      Assert.Throws<InvalidOperationException>(
        () => MainGridController.OnExistingRowCellUpdateError(1, "Name",
          new InvalidOperationException()),
        "Unsupported exception type");
    }

    [Test]
    public void ErrorOnDelete() {
      Session.BeginUpdate();
      try {
        Data.AddEventTypesPersisted(1, Session);
        Data.AddLocationsPersisted(2, Session);
        Data.AddEventsPersisted(3, Session, Data.Locations[1]);
      } finally {
        Session.Commit();
      }
      // The second Location cannot be deleted because it is a parent of 3 child Events.
      var controller = CreateController(typeof(LocationList));
      controller.FetchData(); // Populate grid
      MainGridController.CreateAndGoToInsertionRow();
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
      var controller = CreateController(typeof(EventList));
      controller.FetchData(); // Populate grid
      Assert.IsTrue(MainGridController.DoesColumnReferenceAnotherEntity("Newsletter"),
        "Newsletter DoesColumnReferenceAnotherEntity");
      var bindingList =
        (TypedBindingList<Event, EventBindingItem>)controller.MainList.BindingList;
      // Newsletter
      var selectedNewsletter = Data.Newsletters[0];
      var selectedNewsletterDate = selectedNewsletter.Date;
      MainGridController.OnRowEnter(0);
      MainGridController.SetComboBoxCellValue(0, "Newsletter", selectedNewsletterDate);
      Assert.AreEqual(0, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after valid Newsletter selection");
      Assert.AreEqual(selectedNewsletterDate, bindingList[0].Newsletter,
        "Newsletter in editor after valid Newsletter selection");
      Assert.AreSame(selectedNewsletter, ToEvent(controller.MainList[0]).Newsletter,
        "Newsletter entity after valid Newsletter selection");
      var notFoundDate = DateTime.Parse("2345/12/31");
      MainGridController.OnRowEnter(0);
      var exception = Assert.Catch(
        () => MainGridController.SetComboBoxCellValue(
          0, "Newsletter", notFoundDate));
      Assert.IsInstanceOf<RowNotInTableException>(exception,
        "Exception on not-found Newsletter pasted");
      Assert.AreEqual(0, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after not-found Newsletter pasted but before OnExistingRowCellUpdateError");
      MainGridController.OnExistingRowCellUpdateError(0, "Newsletter", exception);
      Assert.AreEqual(1, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after not-found Newsletter pasted and after OnExistingRowCellUpdateError");
      Assert.AreEqual("Invalid Newsletter:\r\nNewsletter not found: '31 Dec 2345'",
        View.LastErrorMessage,
        "LastErrorMessage after not-found Newsletter pasted");
      Assert.AreEqual(selectedNewsletterDate, bindingList[0].Newsletter,
        "Newsletter in editor after not-found Newsletter pasted");
      Assert.AreSame(selectedNewsletter, ToEvent(controller.MainList[0]).Newsletter,
        "Newsletter entity after not-found Newsletter pasted");
      // Series
      var selectedSeries = Data.Series[0];
      string selectedSeriesName = selectedSeries.Name;
      Assert.IsNotNull(selectedSeriesName, "selectedSeriesName");
      MainGridController.OnRowEnter(0);
      MainGridController.SetComboBoxCellValue(0, "Series", selectedSeriesName);
      Assert.AreEqual(1, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after valid Series selection");
      Assert.AreEqual(selectedSeriesName, bindingList[0].Series,
        "Series in editor after valid Series selection");
      Assert.AreSame(selectedSeries, ToEvent(controller.MainList[0]).Series,
        "Series entity after valid Series selection");
      const string notFoundName = "Not-Found Name";
      MainGridController.OnRowEnter(0);
      exception = Assert.Catch(
        () => MainGridController.SetComboBoxCellValue(
          0, "Series", notFoundName));
      Assert.IsInstanceOf<RowNotInTableException>(exception,
        "Exception on not-found Series pasted");
      Assert.AreEqual(1, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after not-found Series pasted but before OnExistingRowCellUpdateError");
      MainGridController.OnExistingRowCellUpdateError(0, "Series", exception);
      Assert.AreEqual(2, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after not-found Series pasted and after OnExistingRowCellUpdateError");
      Assert.AreEqual("Invalid Series:\r\nSeries not found: 'Not-Found Name'",
        View.LastErrorMessage,
        "LastErrorMessage after not-found Series pasted");
      Assert.AreEqual(selectedSeriesName, bindingList[0].Series,
        "Series in editor after not-found Series pasted");
      Assert.AreSame(selectedSeries, ToEvent(controller.MainList[0]).Series,
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
      var controller = CreateController(typeof(NewsletterList));
      controller.FetchData(); // Populate grid
      MainGridController.CreateAndGoToInsertionRow();
      var exception = new FormatException("Potato is not a valid DateTime.");
      MainGridController.OnExistingRowCellUpdateError(3, "Date", exception);
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
      var controller = CreateController(typeof(EventList));
      controller.FetchData(); // Populate grid
      var bindingList =
        (TypedBindingList<Event, EventBindingItem>)controller.MainList.BindingList;
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
      MainGridController.OnExistingRowCellUpdateError(2, "Date", exception);
      Assert.AreEqual(1, View.ShowErrorMessageCount, "ShowErrorMessageCount");
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
      var controller = CreateController(typeof(NewsletterList));
      controller.FetchData(); // Populate grid
      Assert.AreEqual("Date", MainGridController.GetColumnDisplayName("Date"), "Date");
      Assert.AreEqual("URL", MainGridController.GetColumnDisplayName("Url"), "Url");
    }

    [Test]
    public void GridSplitterDistance() {
      const int distance = 123;
      var controller = CreateController(typeof(EventList));
      controller.FetchData();
      controller.GridSplitterDistance = distance;
      Assert.AreEqual(distance, controller.GridSplitterDistance);
    }

    [Test]
    public void NonKeyUpdateError() {
      Session.BeginUpdate();
      Data.AddNewslettersPersisted(2, Session);
      Session.Commit();
      var controller = CreateController(typeof(NewsletterList));
      controller.FetchData(); // Populate grid
      var bindingList =
        (TypedBindingList<Newsletter, NewsletterBindingItem>)controller.MainList
          .BindingList;
      MainGridController.OnRowEnter(1);
      var exception = Assert.Catch<DatabaseUpdateErrorException>(
        () => bindingList[1].Url = bindingList[0].Url,
        "Rename Url should have thrown DatabaseUpdateErrorException.");
      MainGridController.OnExistingRowCellUpdateError(1, "Url", exception);
      Assert.AreEqual(1, MainGrid.RestoreCurrentRowCellErrorValueCount,
        "RestoreCurrentRowCellErrorValueCount");
      Assert.AreEqual(1, MainGrid.EditCurrentCellCount,
        "EditCurrentCellCount");
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
      var controller = CreateController(typeof(SetList));
      controller.FetchData(); // Populate grid
      Assert.AreEqual(2, controller.ParentBindingList?.Count, "Parent list count");
      Assert.AreEqual(5, MainGridController.BindingList?.Count,
        "Main list count initially");
      var parentGridController = new ParentGridController(View);
      parentGridController.OnRowEnter(0);
      Assert.AreEqual(3, MainGridController.BindingList?.Count,
        "Main list count when 1st parent selected");
    }

    [Test]
    public void ShowWarningMessage() {
      MainGridController.ShowWarningMessage("Warning! Warning!");
      Assert.AreEqual(1, View.ShowWarningMessageCount);
    }

    [NotNull]
    private TestEditorController CreateController([NotNull] Type mainListType) {
      return new (mainListType,
        View, QueryHelper, Session);
    }

    [NotNull]
    private static Event ToEvent([CanBeNull] object value) {
      return 
        value as Event 
        ?? throw new InvalidOperationException($"{value} is not an Event.");
    } 
  }
}