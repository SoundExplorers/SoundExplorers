using System;
using System.Data;
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
      Piece1 = new Piece {
        QueryHelper = QueryHelper,
        PieceNo = Piece1PieceNo
      };
      Piece2 = new Piece {
        QueryHelper = QueryHelper,
        PieceNo = Piece2PieceNo
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
        Set1.Pieces.Add(Piece1);
        Set1.Pieces.Add(Piece2);
        session.Persist(Piece1);
        session.Persist(Piece2);
        session.Commit();
      }
      Session = new TestSession(DatabaseFolderPath);
      Session.BeginRead();
      Event1 = QueryHelper.Read<Event>(Event1.SimpleKey, Location1, Session);
      Event2 = QueryHelper.Read<Event>(Event2.SimpleKey, Location1, Session);
      Act1 = QueryHelper.Read<Act>(Act1Name, Session);
      Act2 = QueryHelper.Read<Act>(Act2Name, Session);
      Set1 = QueryHelper.Read<Set>(Set1SimpleKey, Event1, Session);
      Set1AtEvent2 = QueryHelper.Read<Set>(Set1SimpleKey, Event2, Session);
      Set2 = QueryHelper.Read<Set>(Set2SimpleKey, Event1, Session);
      Piece1 = QueryHelper.Read<Piece>(Piece1.SimpleKey, Set1, Session);
      Piece2 = QueryHelper.Read<Piece>(Piece2.SimpleKey, Set1, Session);
      Session.Commit();
    }

    [TearDown]
    public void TearDown() {
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }

    private const string Act1Name = "Ewan Husami";
    private const string Act2Name = "Ivanhoe Britches";
    private const string Location1Name = "Pyramid Club";
    private const int Piece1PieceNo = 1;
    private const int Piece2PieceNo = 2;
    private const string Set1Notes = "My notes.";
    private const string Set1SimpleKey = "01";
    private const int Set1SetNo = 1;
    private const int Set2SetNo = 2;
    private const string Set2SimpleKey = "02";
    private string DatabaseFolderPath { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private TestSession Session { get; set; }
    private Act Act1 { get; set; }
    private Act Act2 { get; set; }
    private Event Event1 { get; set; }
    private static DateTime Event1Date => DateTime.Today.AddDays(-1);
    private Event Event2 { get; set; }
    private static DateTime Event2Date => DateTime.Today;
    private Location Location1 { get; set; }
    private Piece Piece1 { get; set; }
    private Piece Piece2 { get; set; }
    private Set Set1 { get; set; }
    private Set Set1AtEvent2 { get; set; }
    private Set Set2 { get; set; }

    [Test]
    public void A010_Initial() {
      Assert.AreEqual(Set1SetNo, Set1.SetNo, "Set1.SetNo");
      Assert.AreEqual(Set1Notes, Set1.Notes, "Set1.Notes");
      Assert.AreEqual(Set1SetNo, Set1AtEvent2.SetNo, "Set1_2.SetNo");
      Assert.AreEqual(Set2SetNo, Set2.SetNo, "Set2.SetNo");
      Assert.AreEqual(2, Event1.Sets.Count, "Event1.Sets.Count");
      Assert.AreEqual(1, Event2.Sets.Count, "Event1.Sets.Count");
      Assert.AreEqual(2, Act1.Sets.Count, "Act1.Sets.Count");
      Assert.AreEqual(1, Act2.Sets.Count, "Act2.Sets.Count");
      Assert.AreSame(Set1, Event1.Sets[0], "Event1.Sets[0]");
      Assert.AreSame(Set1, Event1.Sets[Set1.Key], "Event1.Sets[Set1.Key]");
      Assert.AreSame(Set1, Event1.Sets[Set1SimpleKey, Event1],
        "Event1.Sets[Set1SimpleKey, Event1]");
      Assert.AreSame(Set1AtEvent2, Event2.Sets[0], "Event2.Sets[0]");
      Assert.AreSame(Set2, Event1.Sets[1], "Event1.Sets[1]");
      Assert.AreSame(Set1, Act1.Sets[0], "Act1.Sets[0]");
      Assert.AreSame(Set1AtEvent2, Act2.Sets[0], "Act2.Sets[0]");
      Assert.AreEqual(2, Set1.Pieces.Count, "Set1.Pieces.Count");
      Assert.AreEqual(2, Set1.References.Count, "Set1.References.Count");
      Assert.AreSame(Set1, Piece1.Set, "Piece1.Set");
      Assert.AreEqual(Set1.SetNo, Piece1.Set.SetNo, "Piece1.Set.SetNo");
      Assert.AreSame(Set1, Piece2.Set, "Piece2.Set");
      Assert.AreEqual(Set1.SetNo, Piece2.Set.SetNo, "Piece2.Set.SetNo");
      Assert.AreSame(Event1, Piece1.Set.Event, "Piece1.Set.Event");
      Assert.AreSame(Event1, Piece2.Set.Event, "Piece2.Set.Event");
    }

    /// <summary>
    ///   Changes a Set's Act to one that already played a Set with the same
    ///   SetNo at a different Event.
    /// </summary>
    [Test]
    public void ChangeAct() {
      Session.BeginUpdate();
      Set2 = Act1.Sets[1];
      Assert.AreSame(Set1.Event, Set2.Event,
        "Set2.Event");
      Assert.AreNotSame(Set1.Event, Set1AtEvent2.Event,
        "Set1AtEvent2.Event");
      Set1.Act = Act2;
      Session.Commit();
      Assert.AreSame(Act2, Set1.Act, "Set1.Act");
      Assert.AreEqual(1, Act1.Sets.Count, "Act1.Sets.Count");
      Assert.AreEqual(2, Act2.Sets.Count, "Act2.Sets.Count");
      Assert.AreSame(Set1, Act2.Sets[0], "Act2 1st Set");
      Assert.AreSame(Set1AtEvent2, Act2.Sets[1], "Act2 2nd Set");
    }

    [Test]
    public void DisallowChangeSetNoToDuplicate() {
      Session.BeginUpdate();
      Set2.SetNo = Set2SetNo;
      Assert.Throws<DuplicateKeyException>(() => Set2.SetNo = Set1SetNo);
      Session.Commit();
    }

    [Test]
    public void DisallowChangeSetNoToZero() {
      Session.BeginUpdate();
      Assert.Throws<NoNullAllowedException>(() => Set2.SetNo = 0);
      Session.Commit();
    }

    [Test]
    public void DisallowPersistUnspecifiedSetNo() {
      var noSetNo = new Set {
        QueryHelper = QueryHelper
      };
      Session.BeginUpdate();
      noSetNo.Event = Event1;
      Assert.Throws<NoNullAllowedException>(() => Session.Persist(noSetNo));
      Session.Abort();
    }

    [Test]
    public void DisallowRemovePiece() {
      Session.BeginUpdate();
      Assert.Throws<ConstraintException>(() => Set1.Pieces.Remove(Piece1));
      Session.Commit();
    }

    [Test]
    public void DisallowSetKeyToDuplicate() {
      var duplicate = new Set {
        QueryHelper = QueryHelper,
        SetNo = Set1SetNo
      };
      Session.BeginUpdate();
      Assert.Throws<DuplicateKeyException>(() => duplicate.Event = Event1);
      Session.Commit();
    }

    [Test]
    public void DisallowUnpersistSetWithPieces() {
      Session.BeginUpdate();
      Assert.Throws<ConstraintException>(() =>
        Set1.Unpersist(Session));
      Session.Commit();
    }

    [Test]
    public void Unpersist() {
      var set3 = new Set {
        QueryHelper = QueryHelper,
        SetNo = 3
      };
      Session.BeginUpdate();
      set3.Act = Act2;
      set3.Event = Event1;
      Session.Persist(set3);
      Session.Commit();
      Assert.AreEqual(2, Act2.Sets.Count, "Act2.Sets.Count after Persist");
      Assert.AreEqual(3, Event1.Sets.Count, "Event1.Sets.Count after Persist");
      Assert.AreSame(set3, Act2.Sets[set3.Key],
        "Act2.Sets[set3.Key] after Persist");
      Assert.AreSame(set3, Event1.Sets[set3.Key],
        "Event1.Sets[set3.Key] after Persist");
      Assert.AreSame(Act2, set3.Act, "set3.Act after Persist");
      Assert.AreSame(Event1, set3.Event, "set3.Event after Persist");
      Session.BeginUpdate();
      Session.Unpersist(set3);
      Session.Commit();
      Assert.AreEqual(1, Act2.Sets.Count, "Act2.Sets.Count after Unpersist");
      Assert.AreEqual(2, Event1.Sets.Count,
        "Event1.Sets.Count after Unpersist");
      Assert.IsNull(set3.Act, "set3.Act after Unpersist");
      Assert.IsNull(set3.Event, "set3.Event after Unpersist");
    }
  }
}