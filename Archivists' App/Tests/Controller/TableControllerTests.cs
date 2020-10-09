using System;
using System.Collections;
using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;
using SoundExplorers.Tests.Model;

namespace SoundExplorers.Tests.Controller {
  [TestFixture]
  public class TableControllerTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      Data = new TestData(QueryHelper);
      Session = new TestSession();
      View = new MockTableView();
    }

    [TearDown]
    public void TearDown() {
      Session.DeleteDatabaseFolderIfExists();
    }

    private TestTableController Controller { get; set; }
    private TestData Data { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private TestSession Session { get; set; }
    private MockTableView View { get; set; }

    [Test]
    public void Edit() {
      const string name1 = "Auntie";
      const string name2 = "Uncle";
      var editor = new TestEditor<NotablyNamedBindingItem>();
      Controller = new TestTableController(View, typeof(LocationList), Session);
      Controller.FetchData(); // The grid will be empty initially
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
      Assert.AreEqual(2, View.OnRowInsertedOrDeletedCount, "OnRowInsertedOrDeletedCount");
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
      Assert.AreEqual(1, View.FocusMainGridCellCount,
        "FocusMainGridCellCount after error message shown for duplicate rename");
      Assert.AreEqual(0, View.FocusMainGridCellColumnIndex,
        "FocusMainGridCellColumnIndex after error message shown for duplicate rename");
      Assert.AreEqual(1, View.FocusMainGridCellRowIndex,
        "FocusMainGridCellRowIndex after error message shown for duplicate rename");
      Assert.AreEqual(name2, editor[1].Name,
        "Original name restored after error message shown for duplicate rename");
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
      Assert.AreEqual(2, View.OnRowInsertedOrDeletedCount,
        "OnRowInsertedOrDeletedCount unchanged after duplicate insert");
      Assert.AreEqual(3, editor.Count,
        "editor.Count before error message shown for duplicate insert");
      Controller.OnMainGridRowValidated(2);
      Assert.AreEqual(2, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after error message shown for duplicate insert");
      Assert.AreEqual(2, editor.Count,
        "editor.Count after error message shown for duplicate insert");
      Assert.AreEqual(1, View.MakeMainGridInsertionRowCurrentCount,
        "MakeMainGridInsertionRowCurrentCount after error message shown for duplicate insert");
      // Delete the second item
      Controller.OnMainGridRowEnter(1);
      Controller.OnMainGridRowRemoved(1);
      Assert.AreEqual(3, View.OnRowInsertedOrDeletedCount, "OnRowInsertedOrDeletedCount after delete");
      Controller.FetchData(); // Refresh grid
      editor.SetBindingList(Controller.MainBindingList);
      Assert.AreEqual(1, editor.Count, "editor.Count after FetchData #3");
    }

    [Test]
    public void ErrorOnDelete() {
      var editor = new TestEditor<NotablyNamedBindingItem>();
      Session.BeginUpdate();
      try {
        Data.AddEventTypesPersisted(1, Session);
        Data.AddLocationsPersisted(2, Session);
        Data.AddEventsPersisted(3, Session, Data.Locations[1]);
        Session.Commit();
      } catch {
        Session.Abort();
        throw;
      }
      // The second Location cannot be deleted because it is a parent of 3 child Events.
      Controller = new TestTableController(View, typeof(LocationList), Session);
      Controller.CreateEntityListData(typeof(LocationList), (IList)Data.Locations);
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
  }
}