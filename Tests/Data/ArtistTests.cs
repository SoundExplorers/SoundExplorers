using System;
using System.Data;
using NUnit.Framework;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data {
  [TestFixture]
  public class ArtistTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
      Data = new TestData(QueryHelper);
      Session = new TestSession(DatabaseFolderPath);
      Session.BeginUpdate();
      Data.AddRootsPersistedIfRequired(Session);
      Session.Commit();
      DefaultAct = Act.CreateDefault(Data.ActRoot);
      DefaultNewsletter = Newsletter.CreateDefault(Data.NewsletterRoot);
      DefaultSeries = Series.CreateDefault(Data.SeriesRoot);
      RalphJenkins = new Artist(Data.ArtistRoot) {
        QueryHelper = QueryHelper,
        Forename = RalphJenkinsForename,
        Surname = RalphJenkinsSurname,
        Notes = RalphJenkinsNotes
      };
      Clarissa = new Artist(Data.ArtistRoot) {
        QueryHelper = QueryHelper,
        Forename = ClarissaForename
      };
      Baker = new Artist(Data.ArtistRoot) {
        QueryHelper = QueryHelper,
        Surname = BakerSurname
      };
      Role1 = new Role(Data.RoleRoot) {
        QueryHelper = QueryHelper,
        Name = Role1Name
      };
      Location1 = new Location(Data.LocationRoot) {
        QueryHelper = QueryHelper,
        Name = Location1Name
      };
      Event1 = new Event(Data.EventRoot) {
        QueryHelper = QueryHelper,
        Date = Event1Date
      };
      Set1 = new Set(Data.SetRoot) {
        QueryHelper = QueryHelper,
        SetNo = Set1SetNo
      };
      Piece1 = new TestPiece(Data.PieceRoot) {
        QueryHelper = QueryHelper,
        PieceNo = Piece1PieceNo
      };
      Credit1 = new Credit(Data.CreditRoot) {
        QueryHelper = QueryHelper,
        CreditNo = Credit1CreditNo
      };
      Credit2 = new Credit(Data.CreditRoot) {
        QueryHelper = QueryHelper,
        CreditNo = Credit2CreditNo
      };
      Session.BeginUpdate();
      Session.Persist(DefaultAct);
      Session.Persist(DefaultNewsletter);
      Session.Persist(DefaultSeries);
      Session.Persist(RalphJenkins);
      Session.Persist(Clarissa);
      Session.Persist(Baker);
      Session.Persist(Role1);
      Session.Persist(Location1);
      Data.AddEventTypesPersisted(1, Session);
      Event1.EventType = Data.EventTypes[0];
      Event1.Location = Location1;
      Session.Persist(Event1);
      Set1.Event = Event1;
      Data.AddGenresPersisted(1, Session);
      Set1.Genre = Data.Genres[0];
      Session.Persist(Set1);
      Piece1.Set = Set1;
      Session.Persist(Piece1);
      Credit1.Artist = RalphJenkins;
      Credit1.Role = Role1;
      Credit1.Piece = Piece1;
      Credit2.Artist = RalphJenkins;
      Credit2.Role = Role1;
      Credit2.Piece = Piece1;
      Session.Persist(Credit1);
      Session.Persist(Credit2);
      Session.Commit();
      Session.BeginRead();
      RalphJenkins = QueryHelper.Read<Artist>(RalphJenkinsName, Session);
      Clarissa = QueryHelper.Read<Artist>(ClarissaName, Session);
      Baker = QueryHelper.Read<Artist>(BakerName, Session);
      Credit1 = QueryHelper.Read<Credit>(Credit1.SimpleKey, Piece1, Session);
      Credit2 = QueryHelper.Read<Credit>(Credit2.SimpleKey, Piece1, Session);
      Session.Commit();
    }

    [TearDown]
    public void TearDown() {
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }

    private const string BakerName = "Baker";
    private const string BakerSurname = " Baker ";
    private const string ClarissaForename = " Clarissa ";
    private const string ClarissaName = "Clarissa";
    private const string RalphJenkinsForename = " Ralph ";
    private const string RalphJenkinsName = "Jenkins, Ralph";
    private const string RalphJenkinsSurname = " Jenkins ";
    private const string RalphJenkinsNotes = "My notes.";
    private const int Credit1CreditNo = 1;
    private const int Credit2CreditNo = 2;
    private const string Location1Name = "Pyramid Club";
    private const int Piece1PieceNo = 1;
    private const string Role1Name = "Banjo";
    private const int Set1SetNo = 1;
    private string DatabaseFolderPath { get; set; } = null!;
    private QueryHelper QueryHelper { get; set; } = null!;
    private TestData Data { get; set; } = null!;
    private TestSession Session { get; set; } = null!;
    private Act DefaultAct { get; set; } = null!;
    private Newsletter DefaultNewsletter { get; set; } = null!;
    private Series DefaultSeries { get; set; } = null!;
    private Artist RalphJenkins { get; set; } = null!;
    private Artist Clarissa { get; set; } = null!;
    private Artist Baker { get; set; } = null!;
    private Credit Credit1 { get; set; } = null!;
    private Credit Credit2 { get; set; } = null!;
    private Event Event1 { get; set; } = null!;
    private static DateTime Event1Date => DateTime.Today.AddDays(-1);
    private Location Location1 { get; set; } = null!;
    private Piece Piece1 { get; set; } = null!;
    private Role Role1 { get; set; } = null!;
    private Set Set1 { get; set; } = null!;

    [Test]
    public void A010_Initial() {
      Assert.AreEqual(RalphJenkinsForename, RalphJenkins.Forename,
        "RalphJenkins.Forename");
      Assert.AreEqual(RalphJenkinsSurname, RalphJenkins.Surname,
        "RalphJenkins.Surname");
      Assert.AreEqual(RalphJenkinsName, RalphJenkins.Name, "RalphJenkins.Name");
      Assert.AreEqual(RalphJenkinsNotes, RalphJenkins.Notes,
        "RalphJenkins.Notes");
      Assert.AreEqual(ClarissaForename, Clarissa.Forename, "Clarissa.Forename");
      Assert.IsNull(Clarissa.Surname, "Clarissa.Surname");
      Assert.AreEqual(ClarissaName, Clarissa.Name, "Clarissa.Name");
      Assert.IsNull(Baker.Forename, "Baker.Forename");
      Assert.AreEqual(BakerSurname, Baker.Surname, "Baker.Surname");
      Assert.AreEqual(BakerName, Baker.Name, "Baker.Name");
      Assert.AreEqual(2, RalphJenkins.Credits.Count,
        "RalphJenkins.Credits.Count");
      Assert.AreEqual(2, RalphJenkins.References.Count,
        "RalphJenkins.References.Count");
      Session.BeginRead();
      RalphJenkins = QueryHelper.Read<Artist>(RalphJenkinsName, Session);
      Credit1 = QueryHelper.Read<Credit>(Credit1.SimpleKey, Piece1, Session);
      Credit2 = QueryHelper.Read<Credit>(Credit2.SimpleKey, Piece1, Session);
      Assert.AreSame(Credit1, RalphJenkins.Credits[0],
        "RalphJenkins.Credits[0]");
      Assert.AreSame(Credit2, RalphJenkins.Credits[1],
        "RalphJenkins.Credits[1]");
      Assert.AreSame(RalphJenkins, Credit1.Artist, "Credit1.Artist");
      Assert.AreEqual(RalphJenkinsName, Credit1.Artist.Name,
        "Credit1.Artist.Name");
      Assert.AreSame(RalphJenkins, Credit2.Artist, "Credit2.Artist");
      Session.Commit();
    }

    [Test]
    public void DisallowDuplicate() {
      var duplicate = new Artist(Data.ArtistRoot) {
        QueryHelper = QueryHelper,
        // Clarissa has this as a Forename and has no Surname.
        // So the generated Name / SimpleKey should be the same
        // for both Artists and this one should be a duplicate.
        Surname = ClarissaName
      };
      Session.BeginUpdate();
      Assert.Throws<PropertyConstraintException>(() => Session.Persist(duplicate));
      Session.Commit();
    }

    [Test]
    public void DisallowSetNameToNull() {
      var nameless = new Artist(Data.ArtistRoot);
      Assert.Throws<PropertyConstraintException>(() => nameless.Forename = string.Empty);
    }

    [Test]
    public void DisallowUnpersistArtistWithCredits() {
      Session.BeginUpdate();
      Assert.Throws<ConstraintException>(() =>
        RalphJenkins.Unpersist(Session));
      Session.Commit();
    }

    [Test]
    public void Unpersist() {
      Session.BeginUpdate();
      Assert.DoesNotThrow(() => Session.Unpersist(Clarissa));
      Session.Commit();
    }
  }
}