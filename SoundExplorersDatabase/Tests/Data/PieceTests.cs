using System;
using System.Data;
using System.Data.Linq;
using NUnit.Framework;
using SoundExplorersDatabase.Data;

namespace SoundExplorersDatabase.Tests.Data {
  [TestFixture]
  public class PieceTests {
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
      Set1 = new Set {
        QueryHelper = QueryHelper,
        SetNo = Set1SetNo
      };
      Set2 = new Set {
        QueryHelper = QueryHelper,
        SetNo = Set2SetNo
      };
      Piece1 = new Piece {
        QueryHelper = QueryHelper,
        PieceNo = Piece1PieceNo,
        AudioUrl = Piece1AudioUrl,
        Notes = Piece1Notes,
        Title = Piece1Title,
        VideoUrl = Piece1VideoUrl
      };
      Piece1AtSet2 = new Piece {
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
        session.Persist(Event1);
        Set1.Event = Event1;
        Set2.Event = Event1;
        session.Persist(Set1);
        session.Persist(Set2);
        Piece1.Set = Set1;
        Piece1AtSet2.Set = Set2;
        Piece2.Set = Set1;
        session.Persist(Piece1);
        session.Persist(Piece1AtSet2);
        session.Persist(Piece2);
        session.Commit();
      }
    }

    [TearDown]
    public void TearDown() {
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }

    private const string Location1Name = "Pyramid Club";
    private const string Piece1Notes = "My notes.";
    private const string Piece1SimpleKey = "01";
    private const string Piece1Title = "My title.";
    private const int Piece1PieceNo = 1;
    private const int Piece2PieceNo = 2;
    private const string Piece2SimpleKey = "02";
    private const int Set1SetNo = 1;
    private const int Set2SetNo = 2;
    private string DatabaseFolderPath { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private Event Event1 { get; set; }
    private static DateTime Event1Date => DateTime.Today.AddDays(-1);
    private Location Location1 { get; set; }
    private Piece Piece1 { get; set; }

    private static Uri Piece1AudioUrl =>
      new Uri(
        "https://archive.org/details/geometry_dash_1.9/Geometry+Dash+OST/BaseAfterBase.mp3",
        UriKind.Absolute);

    private static Uri Piece1VideoUrl =>
      new Uri("https://archive.org/details/nikopivx/niko-pivx-xoxo-hd.mp4",
        UriKind.Absolute);

    private Piece Piece1AtSet2 { get; set; }
    private Piece Piece2 { get; set; }
    private Set Set1 { get; set; }
    private Set Set2 { get; set; }

    [Test]
    public void A010_Initial() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        Location1 = QueryHelper.Read<Location>(Location1Name, session);
        Event1 = QueryHelper.Read<Event>(Event1.SimpleKey, Location1, session);
        Set1 = QueryHelper.Read<Set>(Set1.SimpleKey, Event1, session);
        Set2 = QueryHelper.Read<Set>(Set2.SimpleKey, Event1, session);
        Piece1 = QueryHelper.Read<Piece>(Piece1SimpleKey, Set1, session);
        Piece1AtSet2 = QueryHelper.Read<Piece>(Piece1SimpleKey, Set2, session);
        Piece2 = QueryHelper.Read<Piece>(Piece2SimpleKey, Set1, session);
        session.Commit();
      }
      Assert.AreEqual(Piece1PieceNo, Piece1.PieceNo, "Piece1.PieceNo");
      Assert.AreEqual(Piece1AudioUrl, Piece1.AudioUrl, "Piece1.AudioUrl");
      Assert.AreEqual(Piece1Notes, Piece1.Notes, "Piece1.Notes");
      Assert.AreEqual(Piece1Title, Piece1.Title, "Piece1.Title");
      Assert.AreEqual(Piece1VideoUrl, Piece1.VideoUrl, "Piece1.VideoUrl");
      Assert.AreEqual(Piece1PieceNo, Piece1AtSet2.PieceNo, "Piece1_2.PieceNo");
      Assert.AreEqual(Piece2PieceNo, Piece2.PieceNo, "Piece2.PieceNo");
      Assert.AreEqual(2, Set1.Pieces.Count, "Set1.Pieces.Count");
      Assert.AreEqual(2, Set1.References.Count, "Set1.References.Count");
      Assert.AreEqual(1, Set2.Pieces.Count, "Set1.Pieces.Count");
      Assert.AreSame(Piece1, Set1.Pieces[0], "Set1.Pieces[0]");
      Assert.AreSame(Piece1AtSet2, Set2.Pieces[0], "Set2.Pieces[0]");
      Assert.AreSame(Piece2, Set1.Pieces[1], "Set1.Pieces[1]");
      Assert.AreSame(Set1, Piece1.Set, "Piece1.Set");
      Assert.AreEqual(Set1.SetNo, Piece1.Set.SetNo, "Piece1.Set.SetNo");
      Assert.AreSame(Set1, Piece2.Set, "Piece2.Set");
      Assert.AreEqual(Set1.SetNo, Piece2.Set.SetNo, "Piece2.Set.SetNo");
      Assert.AreSame(Event1, Piece1.Set.Event, "Piece1.Set.Event");
      Assert.AreSame(Location1, Piece1.Set.Event.Location,
        "Piece1.Set.Event.Location");
      Assert.AreSame(Event1, Piece2.Set.Event, "Piece2.Set.Event");
    }

    [Test]
    public void CannotCheckChangePieceNoToDuplicateOutsideSession() {
      Assert.Throws<InvalidOperationException>(() =>
        Piece2.PieceNo = Piece1PieceNo);
    }

    [Test]
    public void ChangeSet() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Set1 = QueryHelper.Read<Set>(Set1.SimpleKey, Event1, session);
        Set2 = QueryHelper.Read<Set>(Set2.SimpleKey, Event1, session);
        Piece1 = Set1.Pieces[0];
        Piece1AtSet2 = Set2.Pieces[0];
        Piece2 = Set1.Pieces[1];
        Piece2.Set = Set2;
        session.Commit();
      }
      Assert.AreSame(Set2, Piece2.Set,
        "Piece2.Set after Piece2 changes Set");
      Assert.AreEqual(1, Set1.Pieces.Count,
        "Set1.Pieces.Count after Piece2 changes Set");
      Assert.AreEqual(2, Set2.Pieces.Count,
        "Set2.Pieces.Count after Piece2 changes Set");
      Assert.AreSame(Piece1AtSet2, Set2.Pieces[0],
        "Set2 1st Piece after Piece2 changes Set");
      Assert.AreSame(Piece2, Set2.Pieces[1],
        "Set2 2nd Piece after Piece1 changes Set");
    }

    [Test]
    public void DisallowChangePieceNoToDuplicate() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Piece2 =
          QueryHelper.Read<Piece>(Piece2SimpleKey, Set1, session);
        Piece2.PieceNo = Piece2PieceNo;
        Assert.Throws<DuplicateKeyException>(() =>
          Piece2.PieceNo = Piece1PieceNo);
        session.Commit();
      }
    }

    [Test]
    public void DisallowChangePieceNoToZero() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Piece2 = QueryHelper.Read<Piece>(Piece2SimpleKey, Set1, session);
        Assert.Throws<NoNullAllowedException>(() =>
          Piece2.PieceNo = 0);
        session.Commit();
      }
    }

    [Test]
    public void DisallowPersistUnspecifiedPieceNo() {
      var noPieceNo = new Piece {
        QueryHelper = QueryHelper
      };
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Set1 = QueryHelper.Read<Set>(Set1.SimpleKey, Event1, session);
        noPieceNo.Set = Set1;
        Assert.Throws<NoNullAllowedException>(() => session.Persist(noPieceNo));
        session.Abort();
      }
    }

    [Test]
    public void DisallowSetKeyToDuplicate() {
      var duplicate = new Piece {
        QueryHelper = QueryHelper,
        PieceNo = Piece1PieceNo
      };
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Set1 = QueryHelper.Read<Set>(Set1.SimpleKey, Event1, session);
        Piece1 = QueryHelper.Read<Piece>(Piece1SimpleKey, Set1, session);
        Assert.Throws<DuplicateKeyException>(() => duplicate.Set = Set1);
        session.Commit();
      }
    }
  }
}