using System;
using System.Data.Linq;
using NUnit.Framework;
using SoundExplorersDatabase.Data;

namespace SoundExplorersDatabase.Tests.Data {
  [TestFixture]
  public class ActTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
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
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        session.Persist(Location1);
        Event1.Location = Location1;
        session.Persist(Event1);
        session.Persist(Act1);
        session.Persist(Act2);
        Event1.Sets.Add(Set1);
        Event1.Sets.Add(Set2);
        session.Persist(Set1);
        session.Persist(Set2);
        Act1.Sets.Add(Set1);
        session.Commit();
      }
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
    private string DatabaseFolderPath { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private Act Act1 { get; set; }
    private Act Act2 { get; set; }
    private Event Event1 { get; set; }
    private static DateTime Event1Date => DateTime.Parse(Event1Key);
    private Location Location1 { get; set; }
    private Set Set1 { get; set; }
    private Set Set2 { get; set; }

    [Test]
    public void T010_Initial() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        Act1 = QueryHelper.Read<Act>(Act1Name, session);
        Act2 = QueryHelper.Read<Act>(Act2Name, session);
        Set1 = QueryHelper.Read<Set>(Set1.SimpleKey, Event1, session);
        Set2 = QueryHelper.Read<Set>(Set2.SimpleKey, Event1, session);
        session.Commit();
      }
      Assert.AreEqual(Act1Name, Act1.Name, "Act1.Name initially");
      Assert.AreEqual(Act1Notes, Act1.Notes, "Act1.Notes initially");
      Assert.AreEqual(Act2Name, Act2.Name, "Act2.Name initially");
      Assert.AreEqual(1, Act1.Sets.Count,
        "Act1.Sets.Count initially");
      Assert.AreSame(Act1, Set1.Act, "Set1.Act initially");
      Assert.AreEqual(Act1.Name, Set1.Act?.Name,
        "Set1.Act.Name initially");
      Assert.IsNull(Set2.Act, "Set2.Act initially");
    }

    [Test]
    public void T020_RemoveSet() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Act1 = QueryHelper.Read<Act>(Act1Name, session);
        Set1 = QueryHelper.Read<Set>(Set1.SimpleKey, Event1, session);
        Act1.Sets.Remove(Set1);
        session.Commit();
      }
      Assert.AreEqual(0, Act1.Sets.Count,
        "Act1.Sets.Count after remove");
      Assert.IsNull(Set1.Act, "Set1.Act after remove");
    }

    [Test]
    public void T030_DisallowDuplicate() {
      var duplicate = new Act {
        QueryHelper = QueryHelper,
        Name = Act1Name
      };
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Assert.Throws<DuplicateKeyException>(() =>
          session.Persist(duplicate), "Duplicate");
        session.Commit();
      }
    }
  }
}