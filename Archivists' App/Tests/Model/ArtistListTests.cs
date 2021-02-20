using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests.Model {
  [TestFixture]
  public class ArtistListTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      Data = new TestData(QueryHelper);
      Session = new TestSession();
      List = new ArtistList {Session = Session};
    }

    [TearDown]
    public void TearDown() {
      Session.DeleteDatabaseFolderIfExists();
    }

    private TestData Data { get; set; } = null!;
    private ArtistList List { get; set; } = null!;
    private QueryHelper QueryHelper { get; set; } = null!;
    private TestSession Session { get; set; } = null!;

    [Test]
    public void A010_Initial() {
      Assert.AreEqual("Artist", List.EntityTypeName, "EntityName");
      Assert.AreEqual(3, List.Columns.Count, "Columns.Count");
      Assert.AreEqual("Forename", List.Columns[0].PropertyName, "Columns[0].Name");
      Assert.AreEqual("Surname", List.Columns[1].PropertyName, "Columns[1].Name");
      Assert.AreEqual("Notes", List.Columns[2].PropertyName, "Columns[2].Name");
    }

    [Test]
    public void Edit() {
      Session.BeginUpdate();
      Data.AddArtistsPersisted(1, Session);
      Session.Commit();
      const string forename = "Dai";
      // ReSharper disable once StringLiteralTypo
      const string surname = "Fujikura";
      // ReSharper disable once StringLiteralTypo
      const string notes = "Zawazawa";
      // ReSharper disable once StringLiteralTypo
      const string name = "Dai Fujikura";
      List.Populate();
      var bindingList = List.BindingList;
      List.OnRowEnter(0);
      var bindingItem = bindingList[0]; 
      bindingItem.Forename = forename;
      bindingItem.Surname = surname;
      bindingItem.Notes = notes;
      Assert.AreEqual(name, bindingItem.CreateKey().ToString(), "CreateKey");
      var artist = List[0];
      Assert.AreEqual(forename, artist.Forename, "Forename");
      Assert.AreEqual(surname, artist.Surname, "Surname");
      Assert.AreEqual(notes, artist.Notes, "Notes");
      Assert.AreEqual(name, artist.Name, "Name");
    }
  }
}