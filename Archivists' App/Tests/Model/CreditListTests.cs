using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests.Model {
  [TestFixture]
  public class CreditListTests {
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
    private CreditList List { get; set; } = null!;
    private PieceList ParentList { get; set; } = null!;

    [Test]
    public void A010_Initialise() {
      List = CreateCreditList(false);
      Assert.AreEqual("Credit", List.EntityTypeName, "EntityName");
      Assert.AreEqual(typeof(PieceList), List.ParentListType, "ParentListType");
      Assert.AreEqual(3, List.Columns.Count, "Columns.Count");
      Assert.AreEqual("CreditNo", List.Columns[0].PropertyName, 
        "Columns[0].PropertyName");
      Assert.AreEqual("Artist", List.Columns[1].PropertyName, "Columns[1].PropertyName");
      Assert.AreEqual(typeof(ArtistList), List.Columns[1].ReferencedEntityListType,
        "Columns[1].ReferencedEntityListType");
      Assert.AreEqual("Name", List.Columns[1].ReferencedPropertyName,
        "Columns[1].ReferencedColumnName");
      Assert.AreEqual("Artist", List.Columns[1].ReferencedTableName,
        "Columns[1].ReferencedTableName");
      Assert.AreEqual("Role", List.Columns[2].PropertyName, "Columns[2].PropertyName");
      Assert.AreEqual(typeof(RoleList), List.Columns[2].ReferencedEntityListType,
        "Columns[2].ReferencedEntityListType");
      Assert.AreEqual("Name", List.Columns[2].ReferencedPropertyName,
        "Columns[2].ReferencedColumnName");
      Assert.AreEqual("Role", List.Columns[2].ReferencedTableName,
        "Columns[2].ReferencedTableName");
    }

    [Test]
    public void AddCredit() {
      List = CreateCreditList();
      Assert.IsTrue(List.IsChildList, "IsChildList");
      var piece = Data.Pieces[0];
      Populate();
      var bindingList = List.BindingList;
      bindingList.AddNew();
      List.OnRowEnter(3);
      bindingList[3].Artist = Data.Artists[0].Name;
      bindingList[3].Role = Data.Roles[0].Name;
      List.OnRowValidated(3);
      Assert.AreEqual(4, piece.Credits.Count, "Count");
      Populate();
      Assert.AreEqual("4", bindingList[3].CreditNo, "CreditNo in binding list");
      Assert.AreEqual(4, List[3].CreditNo, "CreditNo in List");
      Assert.AreEqual(4, piece.Credits[3].CreditNo, "CreditNo in Event.Credits");
      Assert.AreEqual(4, Data.Artists[0].Credits[3].CreditNo, 
        "CreditNo in Artist.Credits");
      Assert.AreEqual(4, Data.Roles[0].Credits[3].CreditNo, "CreditNo in Roles.Credits");
    }
 
    [Test]
    public void ChangeCreditNo() {
      List = CreateCreditList();
      Populate();
      List.OnRowEnter(2);
      Assert.AreEqual("3", List.BindingList[2].CreditNo, 
        "Binding CreditNo after populate");
      List.BindingList[2].CreditNo = "9";
      Assert.AreEqual(9, Data.Credits[2].CreditNo, "List CreditNo after change");
    }

    [Test]
    public void DefaultCreditNoWithExistingCredits() {
      List = CreateCreditList();
      Populate();
      List.BindingList.AddNew();
      Assert.AreEqual("4", List.BindingList[3].CreditNo);
    }

    [Test]
    public void DefaultCreditNoWithNoExistingCredits() {
      List = CreateCreditList(false);
      Populate();
      List.BindingList.AddNew();
      Assert.AreEqual("1", List.BindingList[0].CreditNo);
    }

    [Test]
    public void DisallowAddCreditWithoutArtist() {
      List = CreateCreditList(false);
      Populate();
      var bindingList = List.BindingList;
      bindingList.AddNew();
      List.OnRowEnter(0);
      bindingList[0].Role = Data.Roles[0].Name;
      var exception = Assert.Catch<DatabaseUpdateErrorException>(
        () => List.OnRowValidated(0),
        "Adding Credit without Artist disallowed.");
      Assert.AreEqual(
        $"Credit '{bindingList[0].Key}' cannot be added because its Artist has not been specified.",
        exception.Message, "Error message");
    }

    [Test]
    public void DisallowAddCreditWithoutRole() {
      List = CreateCreditList(false);
      Populate();
      var bindingList = List.BindingList;
      bindingList.AddNew();
      List.OnRowEnter(0);
      bindingList[0].Artist = Data.Artists[0].Name;
      var exception = Assert.Catch<DatabaseUpdateErrorException>(
        () => List.OnRowValidated(0),
        "Adding Credit without Role disallowed.");
      Assert.AreEqual(
        $"Credit '{bindingList[0].Key}' cannot be added because its Role has not been specified.",
        exception.Message, "Error message");
    }

    private void AddData(bool includingCredits = true) {
      Session.BeginUpdate();
      Data.AddEventTypesPersisted(1, Session);
      Data.AddLocationsPersisted(1, Session);
      Data.AddNewslettersPersisted(1, Session);
      Data.AddSeriesPersisted(1, Session);
      Data.AddEventsPersisted(1, Session);
      Data.AddActsPersisted(1, Session);
      Data.AddGenresPersisted(1, Session);
      Data.AddSetsPersisted(1, Session, Data.Events[0]);
      Data.AddArtistsPersisted(1, Session);
      Data.AddRolesPersisted(1, Session);
      Data.AddPiecesPersisted(1, Session, Data.Sets[0]);
      if (includingCredits) {
        Data.AddCreditsPersisted(3, Session, Data.Pieces[0]);
      }
      Session.Commit();
    }

    private CreditList CreateCreditList(bool addCredits = true) {
      AddData(addCredits);
      var result = new CreditList {Session = Session};
      ParentList = (result.CreateParentList() as PieceList)!;
      return result;
    }

    private void Populate() {
      ParentList.Populate();
      List.Populate(ParentList.GetIdentifyingParentChildrenForMainList(
        ParentList.Count - 1));
    }
  }
}