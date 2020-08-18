using System.Data;
using System.Data.Linq;
using NUnit.Framework;
using SoundExplorersDatabase.Data;

namespace SoundExplorersDatabase.Tests.Data {
  [TestFixture]
  public class ArtistTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
      RalphJenkins = new Artist {
        QueryHelper = QueryHelper,
        Forename = RalphJenkinsForename,
        Surname = RalphJenkinsSurname,
        Notes = RalphJenkinsNotes
      };
      Clarissa = new Artist {
        QueryHelper = QueryHelper,
        Forename = ClarissaForename
      };
      Baker = new Artist {
        QueryHelper = QueryHelper,
        Surname = BakerSurname
      };
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        session.Persist(RalphJenkins);
        session.Persist(Clarissa);
        session.Persist(Baker);
        session.Commit();
      }
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
    private const string RalphJenkinsName = "Ralph Jenkins";
    private const string RalphJenkinsSurname = " Jenkins ";
    private const string RalphJenkinsNotes = "My notes.";
    private string DatabaseFolderPath { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private Artist RalphJenkins { get; set; }
    private Artist Clarissa { get; set; }
    private Artist Baker { get; set; }

    [Test]
    public void T010_Initial() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        RalphJenkins = QueryHelper.Read<Artist>(RalphJenkinsName, session);
        Clarissa = QueryHelper.Read<Artist>(ClarissaName, session);
        Baker = QueryHelper.Read<Artist>(BakerName, session);
        session.Commit();
      }
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
    }

    [Test]
    public void T020_DisallowSetNameToNull() {
      var nameless = new Artist();
      Assert.Throws<NoNullAllowedException>(() => nameless.Forename = null);
    }

    [Test]
    public void T030_DisallowDuplicate() {
      var duplicate = new Artist {
        QueryHelper = QueryHelper,
        // Clarissa has this as a Forename and has no Surname.
        // So the generated Name / SimpleKey should be the same
        // for both Artists and this one should be a duplicate.
        Surname = ClarissaName
      };
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Assert.Throws<DuplicateKeyException>(() => session.Persist(duplicate));
        session.Commit();
      }
    }
  }
}