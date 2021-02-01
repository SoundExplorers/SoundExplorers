using System;
using System.Data;
using System.Linq;
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
    private SetList List { get; set; } = null!;
    private QueryHelper QueryHelper { get; set; } = null!;
    private TestSession Session { get; set; } = null!;

    [Test]
    public void A010_InitialiseAsParentList() {
      List = CreateSetList();
      Assert.AreEqual("Set", List.EntityTypeName, "EntityName");
      Assert.AreEqual(typeof(EventList), List.ParentListType, "ParentListType");
      Assert.AreEqual(7, List.Columns.Count, "Columns.Count when parent list");
      Assert.AreEqual("Date", List.Columns[0].PropertyName, "Columns[0].PropertyName");
      Assert.AreEqual("Location", List.Columns[1].PropertyName,
        "Columns[1].PropertyName");
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
    public void AddSet() {
      List = CreateSetList();
      var @event = Data.Events[0];
      var eventChildren = CreateEventChildren(@event);
      List.Populate(eventChildren);
      var bindingList = List.TypedBindingList!;
      bindingList.AddNew();
      List.OnRowEnter(3);
      bindingList[3].Genre = Data.Genres[0].Name;
      List.OnRowValidated(3);
      Assert.AreEqual(4, @event.Sets.Count, "Count");
      eventChildren = CreateEventChildren(@event);
      List.Populate(eventChildren);
      Assert.AreEqual(4, List[3].SetNo, "SetNo in List");
      Assert.AreEqual(4, bindingList[3].SetNo, "SetNo in binding list");
      Assert.AreEqual(4, @event.Sets[3].SetNo, "SetNo in Event.Sets");
    }

    [Test]
    public void DefaultSetNoWithExistingSets() {
      List = CreateSetList();
      List.Populate();
      List.BindingList!.AddNew();
      Assert.AreEqual(4, List.TypedBindingList[3].SetNo);
    }

    [Test]
    public void DefaultSetNoWithNoExistingSets() {
      List = CreateSetList(false);
      List.Populate();
      List.TypedBindingList!.AddNew();
      Assert.AreEqual(1, List.TypedBindingList[0].SetNo);
    }

    [Test]
    public void DisallowAddSetWithoutGenre() {
      List = CreateSetList(false);
      var @event = Data.Events[0];
      var eventChildren = CreateEventChildren(@event);
      List.Populate(eventChildren);
      var bindingList = List.TypedBindingList!;
      bindingList.AddNew();
      List.OnRowEnter(0);
      bindingList[0].Act = Data.Acts[0].Name;
      var exception = Assert.Catch<DatabaseUpdateErrorException>(()=> List.OnRowValidated(0), 
        "Adding Set without Genre disallowed.");
      Assert.AreEqual(
        "Set '01 | 2020/01/09 | Athens' cannot be added because its Genre has not been specified.",
        exception.Message, "Error message");
    }

    [Test]
    public void DisallowDuplicateKey() {
      List = CreateSetList();
      var eventChildren = CreateEventChildren(Data.Events[0]);
      List.Populate(eventChildren);
      List.OnRowEnter(2);
      var bindingList = List.TypedBindingList!;
      Exception exception = Assert.Catch<DuplicateNameException>(()=> bindingList[2].SetNo = 1, 
        "Changing SetNo to duplicate for Event disallowed");
      Assert.AreEqual("Another Set with key '01 | 2020/01/09 | Athens' already exists.",
        exception.Message, 
        "Error message on trying to change SetNo to duplicate for Event");
      bindingList.AddNew();
      List.OnRowEnter(3);
      bindingList[3].SetNo = 2;
      exception = Assert.Catch<DatabaseUpdateErrorException>(()=> List.OnRowValidated(3), 
        "Adding Set with SetNo duplicate for Event disallowed");
      Assert.AreEqual("Another Set with key '02 | 2020/01/09 | Athens' already exists.",
        exception.Message, 
        "Error message on trying to add Set with duplicate SetNo for Event");
    }

    [Test]
    public void GetIdentifyingParentChildrenForMainList() {
      List = CreateSetList();
      Session.BeginUpdate();
      Data.AddPiecesPersisted(5, Session);
      Session.Commit();
      List.Populate();
      var identifyingParentChildren = List.GetIdentifyingParentChildrenForMainList(0);
      Assert.AreSame(Data.Sets[0], identifyingParentChildren.IdentifyingParent, 
        "IdentifyingParent");
      Assert.AreEqual(5, identifyingParentChildren.Children.Count, "Count");
      Assert.IsInstanceOf<Piece>(identifyingParentChildren.Children[0], "Child type");
    }

    [Test]
    public void InvalidFormatSetNo() {
      List = CreateSetList();
      List.Populate();
      List.OnValidationError(1, "SetNo",
        new FormatException("Value cannot be cast to Int32"));
      Assert.AreEqual("SetNo must be an integer between 1 and 99.",
        List.LastDatabaseUpdateErrorException!.Message, "Error message");
    }

    [Test]
    public void ReadAsParentList() {
      List = CreateSetList();
      Session.BeginUpdate();
      Data.AddActsPersisted(2, Session);
      var set = Data.Sets[2];
      set.Act = Data.Acts[1];
      Session.Commit();
      List.Populate();
      var bindingList = List.TypedBindingList;
      Assert.AreEqual(set.Event.Date, bindingList[2].Date, "Date");
      Assert.AreEqual(set.Event.Location.Name, bindingList[2].Location, "Location");
      Assert.AreEqual(set.SetNo, bindingList[2].SetNo, "SetNo");
      Assert.AreEqual(set.Act?.Name, bindingList[2].Act, "Act");
      Assert.AreEqual(set.Genre.Name, bindingList[2].Genre, "Genre");
      Assert.AreEqual(set.Notes, bindingList[2].Notes, "Notes");
    }

    private void AddData(bool includingSets = true) {
      Session.BeginUpdate();
      Data.AddActsPersisted(1, Session);
      Data.AddEventTypesPersisted(1, Session);
      Data.AddLocationsPersisted(1, Session);
      Data.AddEventsPersisted(1, Session);
      Data.AddGenresPersisted(1, Session);
      if (includingSets) {
        Data.AddSetsPersisted(3, Session);
      }
      Session.Commit();
    }

    private static IdentifyingParentChildren CreateEventChildren(Event @event) {
      var eventChildren = new IdentifyingParentChildren(
        @event, @event.Sets.Values.ToList());
      return eventChildren;
    }

    private SetList CreateSetList(bool addSets = true) {
      AddData(addSets);
      return new SetList {Session = Session};
    }
  }
}