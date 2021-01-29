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
      List = CreateParentSetList();
      Assert.AreEqual("Set", List.EntityTypeName, "EntityName");
      Assert.AreEqual(typeof(EventList), List.ParentListType, "ParentListType");
      Assert.AreEqual(6, List.Columns.Count, "Columns.Count when parent list");
      Assert.AreEqual("Date", List.Columns[0].PropertyName, "Columns[0].Name");
      Assert.AreEqual("Location", List.Columns[1].PropertyName, "Columns[1].Name");
      Assert.AreEqual("SetNo", List.Columns[2].PropertyName, "Columns[2].Name");
      Assert.AreEqual("Act", List.Columns[3].PropertyName, "Columns[3].Name");
      Assert.AreEqual(typeof(ActList), List.Columns[3].ReferencedEntityListType,
        "Columns[3].ReferencedEntityListType");
      Assert.AreEqual("Name", List.Columns[3].ReferencedPropertyName,
        "Columns[3].ReferencedColumnName");
      Assert.AreEqual("Act", List.Columns[3].ReferencedTableName,
        "Columns[3].ReferencedTableName");
      Assert.AreEqual("Genre", List.Columns[4].PropertyName, "Columns[4].Name");
      Assert.AreEqual(typeof(GenreList), List.Columns[4].ReferencedEntityListType,
        "Columns[4].ReferencedEntityListType");
      Assert.AreEqual("Name", List.Columns[4].ReferencedPropertyName,
        "Columns[4].ReferencedColumnName");
      Assert.AreEqual("Notes", List.Columns[5].PropertyName, "Columns[5].Name");
    }

    [Test]
    public void DefaultSetNoWithExistingSets() {
      List = CreateMainSetList();
      List.Populate();
      List.BindingList!.AddNew();
      Assert.AreEqual(4, List.TypedBindingList[3].SetNo);
    }

    [Test]
    public void DefaultSetNoWithNoExistingSets() {
      List = CreateMainSetList(false);
      List.Populate();
      List.TypedBindingList!.AddNew();
      Assert.AreEqual(1, List.TypedBindingList[0].SetNo);
    }

    [Test]
    public void GetChildrenForMainList() {
      List = CreateParentSetList();
      Session.BeginUpdate();
      Data.AddPiecesPersisted(5, Session);
      Session.Commit();
      List.Populate();
      var children = List.GetChildrenForMainList(0);
      Assert.AreEqual(5, children.Count, "Count");
      Assert.IsInstanceOf<Piece>(children[0], "Child type");
    }

    [Test]
    public void ReadAsParentList() {
      List = CreateParentSetList();
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
      Data.AddEventTypesPersisted(1, Session);
      Data.AddLocationsPersisted(1, Session);
      Data.AddEventsPersisted(1, Session);
      Data.AddGenresPersisted(1, Session);
      if (includingSets) {
        Data.AddSetsPersisted(3, Session);
      }
      Session.Commit();
    }

    private SetList CreateMainSetList(bool addSets = true) {
      AddData(addSets);
      return new SetList {Session = Session};
    }

    private SetList CreateParentSetList() {
      AddData();
      return new SetList {IsParentList = true, Session = Session};
    }
  }
}