using System;
using NUnit.Framework;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data {
  [TestFixture]
  public class ActTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
      Data = new TestData(QueryHelper);
      DefaultAct = new Act {
        QueryHelper = QueryHelper,
        Name = Set.DefaultActName
      };
      DefaultNewsletter = new Newsletter {
        QueryHelper = QueryHelper,
        Date = EntityBase.DefaultDate
      };
      DefaultSeries = new Series {
        QueryHelper = QueryHelper,
        Name = Event.DefaultSeriesName
      };
      Location1 = new Location {
        QueryHelper = QueryHelper,
        Name = Location1Name
      };
      Event1 = new Event {
        QueryHelper = QueryHelper,
        Date = Event1Date
      };
      Act1 = new Act {
        QueryHelper = QueryHelper,
        Name = Act1Name,
        Notes = Act1Notes
      };
      Act2 = new Act {
        QueryHelper = QueryHelper,
        Name = Act2Name
      };
      Set1 = new Set {
        QueryHelper = QueryHelper,
        SetNo = Set1SetNo
      };
      Set2 = new Set {
        QueryHelper = QueryHelper,
        SetNo = Set2SetNo
      };
      using var session = new TestSession(DatabaseFolderPath);
      session.BeginUpdate();
      session.Persist(DefaultAct);
      session.Persist(DefaultNewsletter);
      session.Persist(DefaultSeries);
      session.Persist(Location1);
      Data.AddEventTypesPersisted(1, session);
      Event1.EventType = Data.EventTypes[0];
      Event1.Location = Location1;
      session.Persist(Event1);
      session.Persist(Act1);
      session.Persist(Act2);
      Set1.Event = Event1;
      Set2.Event = Event1;
      Data.AddGenresPersisted(1, session);
      Set1.Genre = Data.Genres[0];
      Set2.Genre = Set1.Genre;
      session.Persist(Set1);
      session.Persist(Set2);
      Set1.Act = Act1;
      session.Commit();
    }

    [TearDown]
    public void TearDown() {
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }

    private const string Act1Name = "Ewan Husami";
    private const string Act1Notes = "My notes.";
    private const string Act2Name = "Ivanhoe Britches";
    private const string Event1Key = "2012/03/04";
    private const string Location1Name = "Pyramid Club";
    private const int Set1SetNo = 1;
    private const int Set2SetNo = 2;
    private string DatabaseFolderPath { get; set; } = null!;
    private QueryHelper QueryHelper { get; set; } = null!;
    private TestData Data { get; set; } = null!;
    private Act DefaultAct { get; set; } = null!;
    private Newsletter DefaultNewsletter { get; set; } = null!;
    private Series DefaultSeries { get; set; } = null!;
    private Act Act1 { get; set; } = null!;
    private Act Act2 { get; set; } = null!;
    private Event Event1 { get; set; } = null!;
    private static DateTime Event1Date => DateTime.Parse(Event1Key);
    private Location Location1 { get; set; } = null!;
    private Set Set1 { get; set; } = null!;
    private Set Set2 { get; set; } = null!;

    [Test]
    public void A010_Initial() {
      using var session = new TestSession(DatabaseFolderPath);
      session.BeginRead();
      DefaultAct = QueryHelper.Read<Act>(DefaultAct.Name, session);
      Act1 = QueryHelper.Read<Act>(Act1Name, session);
      Act2 = QueryHelper.Read<Act>(Act2Name, session);
      Set1 = QueryHelper.Read<Set>(Set1.SimpleKey, Event1, session);
      Set2 = QueryHelper.Read<Set>(Set2.SimpleKey, Event1, session);
      session.Commit();
      Assert.AreEqual(Act1Name, Act1.Name, "Act1.Name");
      Assert.AreEqual(Act1Notes, Act1.Notes, "Act1.Notes");
      Assert.AreEqual(Act2Name, Act2.Name, "Act2.Name");
      Assert.AreEqual(1, Act1.Sets.Count, "Act1.Sets.Count");
      Assert.AreSame(Act1, Set1.Act, "Set1.Act");
      Assert.AreEqual(Act1.Name, Set1.Act.Name, "Set1.Act.Name");
      Assert.AreSame(DefaultAct, Set2.Act, "Set2.Act");
    }

    [Test]
    public void AllowPersistUnspecifiedName() {
      var noName = new Act {
        QueryHelper = QueryHelper
      };
      using var session = new TestSession(DatabaseFolderPath);
      session.BeginUpdate();
      Assert.DoesNotThrow(() => session.Persist(noName));
      session.Commit();
    }

    [Test]
    public void DisallowChangeNameToDuplicate() {
      using var session = new TestSession(DatabaseFolderPath);
      session.BeginUpdate();
      Act2 =
        QueryHelper.Read<Act>(Act2Name, session);
      Act2.Name = Act2Name;
      Assert.Throws<PropertyConstraintException>(() => Act2.Name = Act1Name);
      session.Commit();
    }

    [Test]
    public void DisallowChangeNameToNull() {
      using var session = new TestSession(DatabaseFolderPath);
      session.BeginUpdate();
      Act1 = QueryHelper.Read<Act>(Act1Name, session);
      Assert.Throws<PropertyConstraintException>(() => Act1.Name = string.Empty);
      session.Commit();
    }

    [Test]
    public void DisallowPersistDuplicate() {
      var duplicate = new Act {
        QueryHelper = QueryHelper,
        Name = Act1Name
      };
      using var session = new TestSession(DatabaseFolderPath);
      session.BeginUpdate();
      Assert.Throws<PropertyConstraintException>(() => session.Persist(duplicate));
      session.Commit();
    }
  }
}