using System;
using System.Data;
using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests.Model {
  [TestFixture]
  public class SetListTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      Data = new TestData(QueryHelper);
      Session = new TestSession();
    }

    [TearDown]
    public void TearDown() {
      Session.DeleteDatabaseFolderIfExists();
    }

    private TestData Data { get; set; } = null!;
    private QueryHelper QueryHelper { get; set; } = null!;
    private TestSession Session { get; set; } = null!;
    private SetList List { get; set; } = null!;
    private EventList ParentList { get; set; } = null!;

    [Test]
    public void A010_InitialiseAsChildList() {
      List = CreateSetList(false, false);
      Assert.AreEqual("Set", List.EntityTypeName, "EntityName");
      Assert.AreEqual(typeof(EventList), List.ParentListType, "ParentListType");
      Assert.AreEqual(7, List.Columns.Count, "Columns.Count");
      Assert.AreEqual("Date", List.Columns[0].PropertyName, "Columns[0].PropertyName");
      Assert.IsFalse(List.Columns[0].IsVisible, "Columns[0].IsVisible");
      Assert.AreEqual("Location", List.Columns[1].PropertyName,
        "Columns[1].PropertyName");
      Assert.IsFalse(List.Columns[1].IsVisible, "Columns[1].IsVisible");
      Assert.AreEqual("SetNo", List.Columns[2].PropertyName, "Columns[2].PropertyName");
      Assert.AreEqual("Act", List.Columns[3].PropertyName, "Columns[3].PropertyName");
      Assert.AreEqual(typeof(ActList), List.Columns[3].ReferencedEntityListType,
        "Columns[3].ReferencedEntityListType");
      Assert.AreEqual("Name", List.Columns[3].ReferencedPropertyName,
        "Columns[3].ReferencedColumnName");
      Assert.AreEqual("Act", List.Columns[3].ReferencedTableName,
        "Columns[3].ReferencedTableName");
      Assert.AreEqual("Genre", List.Columns[4].PropertyName, "Columns[4].PropertyName");
      Assert.AreEqual(typeof(GenreList), List.Columns[4].ReferencedEntityListType,
        "Columns[4].ReferencedEntityListType");
      Assert.AreEqual("Name", List.Columns[4].ReferencedPropertyName,
        "Columns[4].ReferencedColumnName");
      Assert.AreEqual("Public?", List.Columns[5].DisplayName, "Columns[5].DisplayName");
      Assert.AreEqual("IsPublic", List.Columns[5].PropertyName,
        "Columns[5].PropertyName");
      Assert.AreEqual("Notes", List.Columns[6].PropertyName, "Columns[6].PropertyName");
    }

    [Test]
    public void AddDefaultAct() {
      List = CreateSetList(false, false);
      Session.BeginRead();
      var defaultAct = QueryHelper.Find<Act>(string.Empty, Session);
      Session.Commit();
      Assert.IsNull(defaultAct, "Default Act before Populate");
      Populate();
      Session.BeginRead();
      defaultAct = QueryHelper.Find<Act>(string.Empty, Session);
      Session.Commit();
      Assert.IsNotNull(defaultAct, "Default Act after Populate");
      Assert.AreEqual("Required default", defaultAct!.Notes, "Notes");
    }

    [Test]
    public void AddSet() {
      List = CreateSetList();
      Assert.IsTrue(List.IsChildList, "IsChildList");
      var @event = Data.Events[0];
      Populate();
      var bindingList = List.BindingList;
      bindingList.AddNew();
      List.OnRowEnter(3);
      bindingList[3].Genre = Data.Genres[0].Name;
      List.OnRowValidated(3);
      Assert.AreEqual(4, @event.Sets.Count, "Count");
      Populate();
      Assert.AreEqual("4", bindingList[3].SetNo, "SetNo in binding list");
      Assert.AreEqual(4, List[3].SetNo, "SetNo in List");
      Assert.AreEqual(4, @event.Sets[3].SetNo, "SetNo in Event.Sets");
      Assert.AreEqual(4, Data.Genres[0].Sets[3].SetNo, "SetNo in Genre.Sets");
    }

    [Test]
    public void ChangeActToDefault() {
      List = CreateSetList();
      var defaultAct = Data.Acts[0];
      var nonDefaultAct = Data.Acts[1];
      Session.BeginUpdate();
      Data.Sets[0].Act = nonDefaultAct;
      Session.Commit();
      Populate();
      List.OnRowEnter(0);
      Assert.AreEqual(nonDefaultAct.Name, List.BindingList[0].Act,
        "Binding Act after populate");
      List.BindingList[0].Act = "    ";
      Assert.AreSame(defaultAct, Data.Sets[0].Act,
        "List Act after change to default");
    }

    [Test]
    public void ChangeSetNo() {
      List = CreateSetList();
      Populate();
      List.OnRowEnter(2);
      Assert.AreEqual("3", List.BindingList[2].SetNo, "Binding SetNo after populate");
      List.BindingList[2].SetNo = "9";
      Assert.AreEqual(9, Data.Sets[2].SetNo, "List SetNo after change");
    }

    [Test]
    public void DefaultSetNoWithExistingSets() {
      List = CreateSetList();
      Populate();
      List.BindingList.AddNew();
      Assert.AreEqual("4", List.BindingList[3].SetNo);
    }

    [Test]
    public void DefaultSetNoWithNoExistingSets() {
      List = CreateSetList(false);
      Populate();
      List.BindingList.AddNew();
      Assert.AreEqual("1", List.BindingList[0].SetNo);
    }

    [Test]
    public void DisallowAddSetWithoutGenre() {
      List = CreateSetList(false);
      Populate();
      var bindingList = List.BindingList;
      bindingList.AddNew();
      List.OnRowEnter(0);
      bindingList[0].Act = Data.Acts[0].Name;
      var exception = Assert.Catch<DatabaseUpdateErrorException>(
        () => List.OnRowValidated(0),
        "Adding Set without Genre disallowed.");
      Assert.AreEqual(
        $"Set '{bindingList[0].Key}' cannot be added because its Genre has not been specified.",
        exception.Message, "Error message");
    }
    
    [Test]
    public void DisallowAddSetWithInvalidSetNo() {
      List = CreateSetList();
      Populate();
      var bindingList = List.BindingList;
      bindingList.AddNew();
      const string defaultSetNo = "4";
      const int newRowIndex = 3;
      List.OnRowEnter(newRowIndex);
      Assert.AreEqual(defaultSetNo, bindingList[newRowIndex].SetNo, "SetNo initially"); 
      bindingList[newRowIndex].SetNo = "ABC"; // Invalid format
      var exception = Assert.Catch<DatabaseUpdateErrorException>(
        () => List.OnRowValidated(newRowIndex),
        "Adding Set with invalid format SetNo disallowed.");
      Assert.AreEqual("SetNo must be an integer between 1 and 99.",
        exception.Message, "Error message when SetNo invalid format");
      // Simulate insertion cancellation on error.
      // AddNew does occur at this point when the user commits an insertion. As the SetNo
      // is invalid, the AddNew will exercise the format error handling logic in
      // BindingItemBase.SimpleKeyToInteger.
      bindingList.AddNew(); 
      List.OnRowEnter(newRowIndex - 1);
      bindingList.RemoveAt(newRowIndex);
      List.OnRowEnter(newRowIndex);
      // After the AddNew on a grid, MainGridController.CancelInsertion restores the error
      // value to the new row for correction or cancellation of the insertion. Here, we 
      // are checking before that step, which we don't need to simulate, to confirm that
      // the AddNew will not have crashed on attempting to work out the default SetNo
      // from a binding list whose last item contains an invalid SetNo. See
      // BindingItemBase.SimpleKeyToInteger.
      Assert.AreEqual(defaultSetNo, bindingList[newRowIndex].SetNo, 
        "SetNo on reentering new row after SetNo format error"); 
      bindingList[newRowIndex].SetNo = "999"; // Out of range
      exception = Assert.Catch<DatabaseUpdateErrorException>(
        () => List.OnRowValidated(newRowIndex),
        "Adding Set with out of range SetNo disallowed.");
      Assert.AreEqual("SetNo must be an integer between 1 and 99.",
        exception.Message, "Error message when SetNo out of range");
      // Simulate insertion cancellation on error.
      // AddNew does occur at this point when the user commits an insertion. As the SetNo
      // is invalid, the AddNew will exercise the out of range error handling logic in
      // BindingItemBase.SimpleKeyToInteger.
      bindingList.AddNew();
      List.OnRowEnter(newRowIndex - 1);
      bindingList.RemoveAt(newRowIndex);
      List.OnRowEnter(newRowIndex);
      // After the AddNew on a grid, MainGridController.CancelInsertion restores the error
      // value to the new row for correction or cancellation of the insertion. Here, we 
      // are checking before that step, which we don't need to simulate, to confirm that
      // the AddNew will not have crashed on attempting to work out the default SetNo
      // from a binding list whose last item contains an invalid SetNo. See
      // BindingItemBase.SimpleKeyToInteger.
      Assert.AreEqual(defaultSetNo, bindingList[newRowIndex].SetNo, 
        "SetNo on reentering new row after SetNo out of range error"); 
    }

    [Test]
    public void DisallowChangeSetNoToInvalidFormat() {
      List = CreateSetList();
      Populate();
      List.OnRowEnter(2);
      var bindingList = List.BindingList;
      Exception exception = Assert.Catch<DatabaseUpdateErrorException>(
        () => bindingList[2].SetNo = "ABC",
        "Changing SetNo to invalid format disallowed");
      Assert.AreEqual("SetNo must be an integer between 1 and 99.",
        exception.Message,
        "Error message on trying to change SetNo to invalid format");
    }

    [Test]
    public void DisallowDuplicateKey() {
      List = CreateSetList();
      Populate();
      List.OnRowEnter(2);
      var bindingList = List.BindingList;
      Assert.DoesNotThrow(() => bindingList[2].SetNo = "3");
      Exception exception = Assert.Catch<DuplicateNameException>(
        () => bindingList[2].SetNo = "1",
        "Changing SetNo to duplicate for Event disallowed");
      Assert.AreEqual($"Another Set with key '{bindingList[2].Key}' already exists.",
        exception.Message,
        "Error message on trying to change SetNo to duplicate for Event");
      bindingList.AddNew();
      List.OnRowEnter(3);
      bindingList[3].SetNo = "2";
      exception = Assert.Catch<DatabaseUpdateErrorException>(() => List.OnRowValidated(3),
        "Adding Set with SetNo duplicate for Event disallowed");
      Assert.AreEqual($"Another Set with key '{bindingList[3].Key}' already exists.",
        exception.Message,
        "Error message on trying to add Set with duplicate SetNo for Event");
    }

    [Test]
    public void GetIdentifyingParentChildrenForMainList() {
      List = CreateSetList();
      Session.BeginUpdate();
      Data.AddPiecesPersisted(5, Session);
      Session.Commit();
      Populate();
      var identifyingParentChildren = List.GetIdentifyingParentChildrenForMainList(0);
      Assert.AreSame(Data.Sets[0], identifyingParentChildren.IdentifyingParent,
        "IdentifyingParent");
      Assert.AreEqual(5, identifyingParentChildren.Children.Count, "Count");
      Assert.IsInstanceOf<Piece>(identifyingParentChildren.Children[0], "Child type");
    }

    [Test]
    public void ReadAsParentList() {
      List = CreateSetList(true, true, false);
      Assert.IsFalse(List.IsChildList, "IsChildList");
      Session.BeginUpdate();
      var set = Data.Sets[2];
      set.Act = Data.Acts[1];
      Session.Commit();
      Populate();
      var bindingList = List.BindingList;
      Assert.AreEqual(set.Event.Date, bindingList[2].Date, "Date");
      Assert.AreEqual(set.Event.Location.Name, bindingList[2].Location, "Location");
      Assert.AreEqual(set.SetNo, int.Parse(bindingList[2].SetNo), "SetNo");
      Assert.AreEqual(set.Act?.Name, bindingList[2].Act, "Act");
      Assert.AreEqual(set.Genre.Name, bindingList[2].Genre, "Genre");
      Assert.AreEqual(set.Notes, bindingList[2].Notes, "Notes");
    }

    private void AddData(bool includingSets = true, bool includingActs = true) {
      Session.BeginUpdate();
      if (includingActs) {
        Data.AddActsPersisted(2, Session);
      }
      Data.AddEventTypesPersisted(1, Session);
      Data.AddLocationsPersisted(1, Session);
      Data.AddNewslettersPersisted(1, Session);
      Data.AddSeriesPersisted(1, Session);
      Data.AddEventsPersisted(1, Session);
      Data.AddGenresPersisted(1, Session);
      if (includingSets) {
        Data.AddSetsPersisted(3, Session);
      }
      Session.Commit();
    }

    private SetList CreateSetList(
      bool addSets = true, bool addActs = true, bool isMainList = true) {
      AddData(addSets, addActs);
      var result = new SetList(isMainList) {Session = Session};
      if (isMainList) {
        ParentList = (result.CreateParentList() as EventList)!;
      }
      return result;
    }

    private void Populate() {
      if (List.IsChildList) {
        ParentList.Populate();
        List.Populate(ParentList.GetIdentifyingParentChildrenForMainList(
          ParentList.Count - 1));
      } else {
        List.Populate();
      }
    }
  }
}