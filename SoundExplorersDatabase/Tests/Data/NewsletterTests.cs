using System;
using System.Data.Linq;
using NUnit.Framework;
using SoundExplorersDatabase.Data;

namespace SoundExplorersDatabase.Tests.Data {
  [TestFixture]
  public class NewsletterTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
      Location1 = new Location {
        QueryHelper = QueryHelper,
        Name = Location1Name
      };
      Newsletter1 = new Newsletter {
        QueryHelper = QueryHelper,
        Date = Newsletter1Date,
        Path = Newsletter1Path
      };
      Newsletter2 = new Newsletter {
        QueryHelper = QueryHelper,
        Date = Newsletter2Date
      };
      Event1 = new Event {
        QueryHelper = QueryHelper,
        Date = Event1Date
      };
      Event2 = new Event {
        QueryHelper = QueryHelper,
        Date = Event2Date
      };
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        session.Persist(Location1);
        session.Persist(Newsletter1);
        session.Persist(Newsletter2);
        Location1.Events.Add(Event1);
        Location1.Events.Add(Event2);
        Newsletter1.Events.Add(Event1);
        session.Persist(Event1);
        session.Persist(Event2);
        session.Commit();
      }
    }

    [TearDown]
    public void TearDown() {
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }

    private const string Location1Name = "Pyramid Club";
    private const string Newsletter1SimpleKey = "2013/04/05";
    private const string Newsletter1Path = "Path One";
    private const string Newsletter2Key = "2016/07/08";

    private string DatabaseFolderPath { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private Event Event1 { get; set; }
    private static DateTime Event1Date => DateTime.Today.AddDays(-1);
    private Event Event2 { get; set; }
    private static DateTime Event2Date => DateTime.Today;
    private Location Location1 { get; set; }
    private Newsletter Newsletter1 { get; set; }

    private static readonly DateTime Newsletter1Date =
      DateTime.Parse(Newsletter1SimpleKey);

    private Newsletter Newsletter2 { get; set; }

    private static readonly DateTime Newsletter2Date =
      DateTime.Parse(Newsletter2Key);

    [Test]
    public void T010_Initial() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        Newsletter1 =
          QueryHelper.Read<Newsletter>(Newsletter1SimpleKey, session);
        Newsletter2 = QueryHelper.Read<Newsletter>(Newsletter2Key, session);
        Event1 = QueryHelper.Read<Event>(Event1.SimpleKey, Location1, session);
        session.Commit();
      }

      Assert.AreEqual(Newsletter1Date, Newsletter1.Date,
        "Newsletter1.Date initially");
      Assert.AreEqual(Newsletter1SimpleKey, Newsletter1.SimpleKey,
        "Newsletter1.SimpleKey initially");
      Assert.AreEqual(Newsletter1Path, Newsletter1.Path,
        "Newsletter1.Path initially");
      Assert.AreEqual(Newsletter2Date, Newsletter2.Date,
        "Newsletter2.Date initially");
      Assert.AreEqual(1, Newsletter1.Events.Count,
        "Newsletter1.Events.Count initially");
      Assert.AreSame(Newsletter1, Event1.Newsletter,
        "Event1.Newsletter initially");
      Assert.AreEqual(Newsletter1.Date, Event1.Newsletter?.Date,
        "Event1.Newsletter.Date initially");
      Assert.IsNull(Event2.Newsletter, "Event2.Newsletter initially");
    }

    [Test]
    public void T020_RemoveEvent() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Newsletter1 =
          QueryHelper.Read<Newsletter>(Newsletter1.SimpleKey, session);
        Event1 = QueryHelper.Read<Event>(Event1.SimpleKey, Location1, session);
        Newsletter1.Events.Remove(Event1);
        session.Commit();
      }

      Assert.AreEqual(0, Newsletter1.Events.Count,
        "Newsletter1.Events.Count after remove");
      Assert.IsNull(Event1.Newsletter, "Event1.Newsletter after remove");
    }

    [Test]
    public void T030_DisallowDuplicateDate() {
      var duplicate = new Newsletter {
        QueryHelper = QueryHelper,
        Date = Newsletter1Date
      };
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Assert.Throws<DuplicateKeyException>(() =>
          session.Persist(duplicate), "Duplicate Date");
        session.Commit();
      }
    }

    [Test]
    public void T040_DisallowDuplicatePath() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        var duplicate = new Newsletter {
          QueryHelper = QueryHelper,
          Date = DateTime.Today,
          Path = Newsletter1Path
        };
        Assert.Throws<DuplicateKeyException>(() =>
          session.Persist(duplicate), "Duplicate Path");
        session.Commit();
      }
    }
  }
}