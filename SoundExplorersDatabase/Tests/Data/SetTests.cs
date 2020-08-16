using System;
using System.Data.Linq;
using NUnit.Framework;
using SoundExplorersDatabase.Data;

namespace SoundExplorersDatabase.Tests.Data {
  [TestFixture]
  public class SetTests {
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
      Event2 = new Event {
        QueryHelper = QueryHelper,
        Date = Event2Date
      };
      Act1 = new Act {
        QueryHelper = QueryHelper,
        Name = Act1Name
      };
      Act2 = new Act {
        QueryHelper = QueryHelper,
        Name = Act2Name
      };
      Set1 = new Set {
        QueryHelper = QueryHelper,
        SetNo = Set1SetNo,
        Notes = Set1Notes
      };
      Set1AtEvent2 = new Set {
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
        Event2.Location = Location1;
        session.Persist(Event1);
        session.Persist(Event2);
        session.Persist(Act1);
        session.Persist(Act2);
        Set1.Event = Event1;
        Set1AtEvent2.Event = Event2;
        Set2.Event = Event1;
        session.Persist(Set1);
        session.Persist(Set2);
        Set1.Act = Act1;
        Set1AtEvent2.Act = Act2;
        Set2.Act = Act1;
        session.Commit();
      }
    }

    [TearDown]
    public void TearDown() {
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }

    private const string Act1Name = "Ewan Husami";
    private const string Act2Name = "Ivanhoe Britches";
    private const string Location1Name = "Pyramid Club";
    private const string Set1Key = "01";
    private const string Set1Notes = "My notes.";
    private const int Set1SetNo = 1;
    private const string Set2Key = "02";
    private const int Set2SetNo = 2;

    private string DatabaseFolderPath { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private Act Act1 { get; set; }
    private Act Act2 { get; set; }
    private Event Event1 { get; set; }
    private static DateTime Event1Date => DateTime.Today.AddDays(-1);
    private Event Event2 { get; set; }
    private static DateTime Event2Date => DateTime.Today;
    private Location Location1 { get; set; }
    private Set Set1 { get; set; }
    private Set Set1AtEvent2 { get; set; }
    private Set Set2 { get; set; }

    [Test]
    public void T010_Initial() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        Location1 = QueryHelper.Read<Location>(Location1Name, session);
        Event1 = QueryHelper.Read<Event>(Event1.SimpleKey, Location1, session);
        Event2 = QueryHelper.Read<Event>(Event2.SimpleKey, Location1, session);
        Act1 = QueryHelper.Read<Act>(Act1Name, session);
        Act2 = QueryHelper.Read<Act>(Act2Name, session);
        Set1 = QueryHelper.Read<Set>(Set1Key, Event1, session);
        Set1AtEvent2 = QueryHelper.Read<Set>(Set1Key, Event2, session);
        Set2 = QueryHelper.Read<Set>(Set2Key, Event1, session);
        session.Commit();
      }
      Assert.AreEqual(Set1SetNo, Set1.SetNo, "Set1.SetNo");
      Assert.AreEqual(Set1Notes, Set1.Notes, "Set1.Notes");
      Assert.AreEqual(Set1SetNo, Set1AtEvent2.SetNo, "Set1_2.SetNo");
      Assert.AreEqual(Set2SetNo, Set2.SetNo, "Set2.SetNo");
      Assert.AreEqual(2, Event1.Sets.Count, "Event1.Sets.Count");
      Assert.AreEqual(1, Event2.Sets.Count, "Event1.Sets.Count");
      Assert.AreEqual(2, Act1.Sets.Count, "Act1.Sets.Count");
      Assert.AreEqual(1, Act2.Sets.Count, "Act2.Sets.Count");
      Assert.AreSame(Set1, Event1.Sets[0], "Event1.Sets[0]");
      Assert.AreSame(Set1AtEvent2, Event2.Sets[0], "Event2.Sets[0]");
      Assert.AreSame(Set2, Event1.Sets[1], "Event1.Sets[1]");
      Assert.AreSame(Set1, Act1.Sets[0], "Act1.Sets[0]");
      Assert.AreSame(Set1AtEvent2, Act2.Sets[0], "Act2.Sets[0]");
    }

    [Test]
    public void T020_DisallowDuplicate() {
      var duplicate = new Set {
        QueryHelper = QueryHelper,
        SetNo = Set1SetNo
      };
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Location1 = QueryHelper.Read<Location>(Location1Name, session);
        Event1 = Location1.Events[Event1.Key];
        Assert.Throws<DuplicateKeyException>(
          () => duplicate.Event = Event1, "Duplicate not allowed");
        session.Commit();
      }
    }

    /// <summary>
    ///   Changes a Set's Act to one that already played a Set with the same
    ///   SetNo at a different Event.
    /// </summary>
    [Test]
    public void T040_ChangeAct() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Act1 = QueryHelper.Read<Act>(Act1.Name, session);
        Act2 = QueryHelper.Read<Act>(Act2.Name, session);
        Set1 = Act1.Sets[0];
        Set1AtEvent2 = Act2.Sets[0];
        Set2 = Act1.Sets[1];
        Assert.AreSame(Set1.Event, Set2.Event,
          "Set2.Event");
        Assert.AreNotSame(Set1.Event, Set1AtEvent2.Event,
          "Set1AtEvent2.Event");
        Set1.Act = Act2;
        session.Commit();
        Assert.AreSame(Act2, Set1.Act,
          "Set1.Act after Set1 changes Act");
        Assert.AreEqual(1, Act1.Sets.Count,
          "Act1.Sets.Count after Set1 changes Act");
        Assert.AreEqual(2, Act2.Sets.Count,
          "Act2.Sets.Count after Set1 changes Act");
        Assert.AreSame(Set1, Act2.Sets[0],
          "Act2 1st Set after Set1 changes Act");
        Assert.AreSame(Set1AtEvent2, Act2.Sets[1],
          "Act2 2nd Set after Set1 changes Act");
      }
    }
  }
}