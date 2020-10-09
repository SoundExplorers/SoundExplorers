using System;
using NUnit.Framework;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;
using SoundExplorers.Tests.Model;

namespace SoundExplorers.Tests.Controller {
  [TestFixture]
  public class TableControllerTests {
    [SetUp]
    public void Setup() {
      // QueryHelper = new QueryHelper();
      // Data = new TestData(QueryHelper);
      Session = new TestSession();
      View = new MockTableView();
    }

    [TearDown]
    public void TearDown() {
      Session.DeleteDatabaseFolderIfExists();
    }

    private TestTableController Controller { get; set; }

    // private TestData Data { get; set; }
    // private QueryHelper QueryHelper { get; set; }
    private TestSession Session { get; set; }
    private MockTableView View { get; set; }

    [Test]
    public void TheTest() {
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
      Assert.AreEqual(2, View.OnRowUpdatedCount, "OnRowUpdatedCount");
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
        Controller.OnMainGridDataError(exception);
      }
      Assert.AreEqual(1, View.StartDatabaseUpdateErrorTimerCount,
        "StartDatabaseUpdateErrorTimerCount after rename to duplicate");
      Assert.AreEqual(name1, editor[1].Name,
        "Still duplicate name before error message shown for duplicate rename");
      Controller.ShowDatabaseUpdateError();
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
      Assert.Throws<InvalidOperationException>(() =>
        Controller.OnMainGridDataError(new InvalidOperationException()));
      // Disallow insert with duplicate name
      editor.AddNew();
      Controller.OnMainGridRowEnter(2); // Go to insertion row
      editor[2].Name = name1;
      Controller.OnMainGridRowValidated(2);
      Assert.AreEqual(2, View.OnRowUpdatedCount,
        "OnRowUpdatedCount unchanged after duplicate insert");
      Assert.AreEqual(2, View.StartDatabaseUpdateErrorTimerCount,
        "StartDatabaseUpdateErrorTimerCount after duplicate insert");
      Assert.AreEqual(3, editor.Count,
        "editor.Count before error message shown for duplicate insert");
      Controller.ShowDatabaseUpdateError();
      Assert.AreEqual(2, View.ShowErrorMessageCount,
        "ShowErrorMessageCount after error message shown for duplicate insert");
      Assert.AreEqual(2, editor.Count,
        "editor.Count after error message shown for duplicate insert");
      Assert.AreEqual(1, View.MakeMainGridInsertionRowCurrentCount,
        "MakeMainGridInsertionRowCurrentCount after error message shown for duplicate insert");
    }
  }
}