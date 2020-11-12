using System;
using System.Data;
using NUnit.Framework;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data {
  [TestFixture]
  public class PieceTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
      Data = new TestData(QueryHelper);
      Baker = new Artist {
        QueryHelper = QueryHelper,
        Surname = BakerName
      };
      Drums = new Role {
        QueryHelper = QueryHelper,
        Name = DrumsName
      };
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
      Credit1 = new Credit {
        QueryHelper = QueryHelper,
        CreditNo = Credit1CreditNo
      };
      Credit2 = new Credit {
        QueryHelper = QueryHelper,
        CreditNo = Credit2CreditNo
      };
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        session.Persist(Baker);
        session.Persist(Drums);
        session.Persist(Location1);
        Event1.Location = Location1;
        Data.AddEventTypesPersisted(1, session);
        Event1.EventType = Data.EventTypes[0];
        session.Persist(Event1);
        Set1.Event = Event1;
        Set2.Event = Event1;
        Data.AddGenresPersisted(1, session);
        Set1.Genre = Data.Genres[0];
        Set2.Genre = Set1.Genre;
        session.Persist(Set1);
        session.Persist(Set2);
        Piece1.Set = Set1;
        Piece1AtSet2.Set = Set2;
        Piece2.Set = Set1;
        session.Persist(Piece1);
        session.Persist(Piece1AtSet2);
        session.Persist(Piece2);
        Credit1.Artist = Baker; 
        Credit1.Piece = Piece1; 
        Credit1.Role = Drums; 
        Credit2.Artist = Baker; 
        Credit2.Piece = Piece1; 
        Credit2.Role = Drums; 
        session.Persist(Credit1);
        session.Persist(Credit2);
        session.Commit();
      }
      Session = new TestSession(DatabaseFolderPath);
      Session.BeginRead();
      Location1 = QueryHelper.Read<Location>(Location1Name, Session);
      Event1 = QueryHelper.Read<Event>(Event1.SimpleKey, Location1, Session);
      Set1 = QueryHelper.Read<Set>(Set1.SimpleKey, Event1, Session);
      Set2 = QueryHelper.Read<Set>(Set2.SimpleKey, Event1, Session);
      Piece1 = QueryHelper.Read<Piece>(Piece1SimpleKey, Set1, Session);
      Piece1AtSet2 = QueryHelper.Read<Piece>(Piece1SimpleKey, Set2, Session);
      Piece2 = QueryHelper.Read<Piece>(Piece2SimpleKey, Set1, Session);
      Credit1 = QueryHelper.Read<Credit>(Credit1.SimpleKey, Piece1, Session);
      Credit2 = QueryHelper.Read<Credit>(Credit2.SimpleKey, Piece1, Session);
      Session.Commit();
    }

    [TearDown]
    public void TearDown() {
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }

    private const string BakerName = "Baker";
    private const int Credit1CreditNo = 1;
    private const int Credit2CreditNo = 2;
    private const string DrumsName = "Drums";
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
    private TestSession Session { get; set; }
    private TestData Data { get; set; }
    private Artist Baker { get; set; }
    private Credit Credit1 { get; set; }
    private Credit Credit2 { get; set; }
    private Event Event1 { get; set; }
    private static DateTime Event1Date => DateTime.Today.AddDays(-1);
    private Location Location1 { get; set; }
    private Piece Piece1 { get; set; }

    private static string Piece1AudioUrl =>
      "https://archive.org/details/geometry_dash_1.9/Geometry+Dash+OST/BaseAfterBase.mp3";

    private static string Piece1VideoUrl =>
      "https://archive.org/details/nikopivx/niko-pivx-xoxo-hd.mp4";

    private Piece Piece1AtSet2 { get; set; }
    private Piece Piece2 { get; set; }
    private Role Drums { get; set; }
    private Set Set1 { get; set; }
    private Set Set2 { get; set; }

    [Test]
    public void A010_Initial() {
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
      Assert.AreEqual(2, Piece1.Credits.Count, "Piece1.Credits.Count");
      Assert.AreEqual(2, Piece1.References.Count, "Piece1.References.Count");
      Assert.AreSame(Credit1, Piece1.Credits[0], "Piece1.Credits[0]");
      Assert.AreSame(Credit2, Piece1.Credits[1], "Piece1.Credits[1]");
      Assert.AreSame(Piece1, Credit1.Piece, "Credit1.Piece");
      Assert.AreEqual(Piece1PieceNo, Credit1.Piece.PieceNo,
        "Credit1.Piece.PieceNo");
      Assert.AreSame(Piece1, Credit2.Piece, "Credit2.Piece");
      Assert.AreEqual(Piece1PieceNo, Credit2.Piece.PieceNo,
        "Credit2.Piece.PieceNo");
      Assert.AreSame(Set1, Credit1.Piece.Set, "Credit1.Piece.Set");
      Assert.AreSame(Set1, Credit2.Piece.Set, "Credit2.Piece.Set");
    }

    [Test]
    public void ChangeAudioUrl() {
      const string newAudioUrl =
        // ReSharper disable once StringLiteralTypo
        "https://soundcloud.com/simonor/peter-daly-simon-ororke-tippabo?in=simonor/sets/peter-daly-and-simon-ororke";
      Session.BeginUpdate();
      Assert.DoesNotThrow(() => Piece1.AudioUrl = newAudioUrl);
      Session.Commit();
    }

    [Test]
    public void ChangeSet() {
      Session.BeginUpdate();
      Piece2.Set = Set2;
      Session.Commit();
      Assert.AreSame(Set2, Piece2.Set, "Piece2.Set");
      Assert.AreEqual(1, Set1.Pieces.Count, "Set1.Pieces.Count");
      Assert.AreEqual(2, Set2.Pieces.Count, "Set2.Pieces.Count");
      Assert.AreSame(Piece1AtSet2, Set2.Pieces[0], "Set2 1st Piece");
      Assert.AreSame(Piece2, Set2.Pieces[1], "Set2 2nd Piece");
    }

    [Test]
    public void ChangeVideoUrl() {
      const string newVideoUrl = "https://www.youtube.com/watch?v=dPIaEWd8zf4";
      Session.BeginUpdate();
      Assert.DoesNotThrow(() => Piece1.VideoUrl = newVideoUrl);
      Session.Commit();
    }

    [Test]
    public void DisallowChangeAudioUrlToDuplicate() {
      Session.BeginUpdate();
      Assert.Throws<PropertyConstraintException>(() =>
        Piece2.AudioUrl = Piece1AudioUrl);
      Session.Commit();
    }

    [Test]
    public void DisallowChangePieceNoToDuplicate() {
      Session.BeginUpdate();
      Piece2.PieceNo = Piece2PieceNo;
      Assert.Throws<ConstraintException>(() =>
        Piece2.PieceNo = Piece1PieceNo);
      Session.Commit();
    }

    [Test]
    public void DisallowChangePieceNoToZero() {
      Session.BeginUpdate();
      Assert.Throws<PropertyConstraintException>(() =>
        Piece2.PieceNo = 0);
      Session.Commit();
    }

    [Test]
    public void DisallowChangeVideoUrlToDuplicate() {
      Session.BeginUpdate();
      Assert.Throws<PropertyConstraintException>(() =>
        Piece2.VideoUrl = Piece1VideoUrl);
      Session.Commit();
    }

    [Test]
    public void DisallowInvalidAudioUrl() {
      var piece = new Piece();
      var exception = Assert.Catch<PropertyConstraintException>(
        () => piece.AudioUrl = "blah",
        "A PropertyConstraintException should have been thrown.");
      Assert.AreEqual("Invalid AudioUrl format: 'blah'.", exception.Message,
        "Message");
      Assert.AreEqual("AudioUrl", exception.PropertyName,
        "PropertyName");
    }

    [Test]
    public void DisallowInvalidVideoUrl() {
      var piece = new Piece();
      var exception = Assert.Catch<PropertyConstraintException>(
        () => piece.VideoUrl = "blah",
        "A PropertyConstraintException should have been thrown.");
      Assert.AreEqual("Invalid VideoUrl format: 'blah'.", exception.Message,
        "Message");
      Assert.AreEqual("VideoUrl", exception.PropertyName,
        "PropertyName");
    }

    [Test]
    public void DisallowPersistDuplicateAudioUrl() {
      var duplicate = new Piece {
        QueryHelper = QueryHelper,
        PieceNo = 9,
        AudioUrl = Piece1AudioUrl
      };
      Session.BeginUpdate();
      duplicate.Set = Set1;
      Assert.Throws<PropertyConstraintException>(() => Session.Persist(duplicate));
      Session.Abort();
    }

    [Test]
    public void DisallowPersistDuplicateVideoUrl() {
      var duplicate = new Piece {
        QueryHelper = QueryHelper,
        PieceNo = 9,
        VideoUrl = Piece1VideoUrl
      };
      Session.BeginUpdate();
      duplicate.Set = Set1;
      Assert.Throws<PropertyConstraintException>(() => Session.Persist(duplicate));
      Session.Abort();
    }

    [Test]
    public void DisallowPersistUnspecifiedPieceNo() {
      var noPieceNo = new Piece {
        QueryHelper = QueryHelper
      };
      Session.BeginUpdate();
      noPieceNo.Set = Set1;
      Assert.Throws<PropertyConstraintException>(() => Session.Persist(noPieceNo));
      Session.Abort();
    }

    [Test]
    public void DisallowSetKeyToDuplicate() {
      var duplicate = new Piece {
        QueryHelper = QueryHelper,
        PieceNo = Piece1PieceNo
      };
      Session.BeginUpdate();
      Assert.Throws<ConstraintException>(() => duplicate.Set = Set1);
      Session.Commit();
    }

    [Test]
    public void DisallowUnpersistPieceWithCredits() {
      Session.BeginUpdate();
      Assert.Throws<ConstraintException>(() =>
        Piece1.Unpersist(Session));
      Session.Commit();
    }

    [Test]
    public void SetAudioUrlToNull() {
      Session.BeginUpdate();
      Assert.DoesNotThrow(() => Piece1.AudioUrl = null);
      Session.Commit();
    }

    [Test]
    public void SetVideoUrlToNull() {
      Session.BeginUpdate();
      Assert.DoesNotThrow(() => Piece1.VideoUrl = null);
      Session.Commit();
    }

    [Test]
    public void Unpersist() {
      Session.BeginUpdate();
      Session.Unpersist(Piece2);
      Session.Commit();
      Assert.AreEqual(1, Set1.Pieces.Count, "Set1.Pieces.Count");
    }
  }
}