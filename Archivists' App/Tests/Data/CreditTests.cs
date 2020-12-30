using System;
using System.Data;
using NUnit.Framework;
using SoundExplorers.Data;
using PropertyConstraintException = SoundExplorers.Data.PropertyConstraintException;

namespace SoundExplorers.Tests.Data {
  [TestFixture]
  public class CreditTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
      Data = new TestData(QueryHelper);
      Baker = new Artist {
        QueryHelper = QueryHelper,
        Surname = BakerName
      };
      Clarissa = new Artist {
        QueryHelper = QueryHelper,
        Forename = ClarissaName
      };
      Drums = new Role {
        QueryHelper = QueryHelper,
        Name = DrumsName
      };
      ElectricGuitar = new Role {
        QueryHelper = QueryHelper,
        Name = ElectricGuitarName
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
      Piece1 = new Piece {
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
      Credit3 = new Credit {
        QueryHelper = QueryHelper,
        CreditNo = Credit3CreditNo
      };
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        session.Persist(Baker);
        session.Persist(Clarissa);
        session.Persist(Drums);
        session.Persist(ElectricGuitar);
        session.Persist(Location1);
        Data.AddEventTypesPersisted(1, session);
        Event1.EventType = Data.EventTypes[0];
        Event1.Location = Location1;
        session.Persist(Event1);
        Set1.Event = Event1;
        Data.AddGenresPersisted(1, session);
        Set1.Genre = Data.Genres[0];
        session.Persist(Set1);
        Piece1.Set = Set1;
        Piece2.Set = Set1;
        session.Persist(Piece1);
        session.Persist(Piece2);
        Credit1.Piece = Piece1;
        Credit1.Artist = Baker;
        Credit1.Role = Drums;
        Credit2.Piece = Piece1;
        Credit2.Artist = Baker;
        Credit2.Role = Drums;
        Credit3.Piece = Piece2;
        Credit3.Artist = Baker;
        Credit3.Role = Drums;
        session.Persist(Credit1);
        session.Persist(Credit2);
        session.Persist(Credit3);
        session.Commit();
      }
      Session = new TestSession(DatabaseFolderPath);
      Session.BeginRead();
      Baker = QueryHelper.Read<Artist>(BakerName, Session);
      Clarissa = QueryHelper.Read<Artist>(ClarissaName, Session);
      Drums = QueryHelper.Read<Role>(DrumsName, Session);
      ElectricGuitar = QueryHelper.Read<Role>(ElectricGuitarName, Session);
      Set1 = QueryHelper.Read<Set>(Set1.SimpleKey, Event1, Session);
      Piece1 = QueryHelper.Read<Piece>(Piece1.SimpleKey, Set1, Session);
      Piece2 = QueryHelper.Read<Piece>(Piece2.SimpleKey, Set1, Session);
      Credit1 = QueryHelper.Read<Credit>(Credit1SimpleKey, Piece1, Session);
      Credit2 = QueryHelper.Read<Credit>(Credit2SimpleKey, Piece1, Session);
      Credit3 = QueryHelper.Read<Credit>(Credit3SimpleKey, Piece2, Session);
      Session.Commit();
    }

    [TearDown]
    public void TearDown() {
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }

    private const string Location1Name = "Pyramid Club";
    private const int Set1SetNo = 1;
    private const string BakerName = "Baker";
    private const string ClarissaName = "Clarissa";
    private const string DrumsName = "Drums";
    private const string ElectricGuitarName = "Electric guitar";
    private const int Piece1PieceNo = 1;
    private const int Piece2PieceNo = 2;
    private const int Credit1CreditNo = 1;
    private const string Credit1SimpleKey = "01";
    private const int Credit2CreditNo = 2;
    private const string Credit2SimpleKey = "02";
    private const int Credit3CreditNo = 3;
    private const string Credit3SimpleKey = "03";
    private string DatabaseFolderPath { get; set; } = null!;
    private QueryHelper QueryHelper { get; set; } = null!;
    private TestSession Session { get; set; } = null!;
    private TestData Data { get; set; } = null!;
    private Artist Clarissa { get; set; } = null!;
    private Artist Baker { get; set; } = null!;
    private Role Drums { get; set; } = null!;
    private Role ElectricGuitar { get; set; } = null!;
    private Location Location1 { get; set; } = null!;
    private Event Event1 { get; set; } = null!;
    private static DateTime Event1Date => DateTime.Today.AddDays(-1);
    private Set Set1 { get; set; } = null!;
    private Piece Piece1 { get; set; } = null!;
    private Piece Piece2 { get; set; } = null!;
    private Credit Credit1 { get; set; } = null!;
    private Credit Credit2 { get; set; } = null!;
    private Credit Credit3 { get; set; } = null!;

    [Test]
    public void A010_Initial() {
      Assert.AreEqual(Credit1CreditNo, Credit1.CreditNo, "Credit1.CreditNo");
      Assert.AreSame(Piece1, Credit1.Piece, "Credit1.Piece");
      Assert.AreEqual(Piece1.PieceNo, Credit1.Piece.PieceNo,
        "Credit1.Piece.PieceNo");
      Assert.AreSame(Baker, Credit1.Artist, "Credit1.Artist");
      Assert.AreSame(Drums, Credit1.Role, "Credit1.Role");
      Assert.AreEqual(Credit2CreditNo, Credit2.CreditNo, "Credit2.CreditNo");
      Assert.AreSame(Piece1, Credit2.Piece, "Credit2.Piece");
      Assert.AreEqual(Piece1.PieceNo, Credit2.Piece.PieceNo,
        "Credit2.Piece.PieceNo");
      Assert.AreSame(Baker, Credit2.Artist, "Credit2.Artist");
      Assert.AreSame(Drums, Credit2.Role, "Credit2.Role");
      Assert.AreSame(Set1, Credit1.Piece.Set, "Credit1.Piece.Set");
      Assert.AreEqual(Credit3CreditNo, Credit3.CreditNo, "Credit3.CreditNo");
      Assert.AreSame(Piece2, Credit3.Piece, "Credit3.Piece");
      Assert.AreSame(Baker, Credit3.Artist, "Credit3.Artist");
      Assert.AreSame(Drums, Credit3.Role, "Credit3.Role");
      Assert.AreEqual(3, Baker.Credits.Count, "Baker.Credits.Count");
      Assert.AreSame(Credit1, Baker.Credits[0], "Baker.Credits[0]");
      Assert.AreSame(Credit2, Baker.Credits[1], "Baker.Credits[2]");
      Assert.AreSame(Credit3, Baker.Credits[2], "Baker.Credits[1]");
      Assert.AreEqual(3, Drums.Credits.Count, "Drums.Credits.Count");
      Assert.AreSame(Credit1, Drums.Credits[0], "Drums.Credits[0]");
      Assert.AreSame(Credit2, Drums.Credits[1], "Drums.Credits[1]");
      Assert.AreSame(Credit3, Drums.Credits[2], "Drums.Credits[2]");
      Assert.AreEqual(2, Piece1.Credits.Count, "Piece1.Credits.Count");
      Assert.AreEqual(1, Piece2.Credits.Count, "Piece1.Credits.Count");
      Assert.AreSame(Credit1, Piece1.Credits[0], "Piece1.Credits[0]");
      Assert.AreSame(Credit2, Piece1.Credits[1], "Piece1.Credits[1]");
      Assert.AreSame(Credit3, Piece2.Credits[0], "Piece2.Credits[0]");
    }

    [Test]
    public void ChangeArtist() {
      Session.BeginUpdate();
      Credit2.Artist = Clarissa;
      Session.Commit();
      Assert.AreSame(Clarissa, Credit2.Artist, "Credit2.Artist");
      Assert.AreEqual(2, Baker.Credits.Count, "Baker.Credits.Count");
      Assert.AreEqual(1, Clarissa.Credits.Count, "Clarissa.Credits.Count");
      Assert.AreSame(Credit2, Clarissa.Credits[0], "Clarissa 2nd Credit");
    }

    [Test]
    public void ChangePiece() {
      Session.BeginUpdate();
      Credit2.Piece = Piece2;
      Session.Commit();
      Assert.AreSame(Piece2, Credit2.Piece, "Credit2.Piece");
      Assert.AreEqual(1, Piece1.Credits.Count, "Piece1.Credits.Count");
      Assert.AreEqual(2, Piece2.Credits.Count, "Piece2.Credits.Count");
      Assert.AreSame(Credit2, Piece2.Credits[0], "Piece2 1st Credit");
      Assert.AreSame(Credit3, Piece2.Credits[1], "Piece2 2nd Credit");
      Assert.AreSame(Credit2, Piece2.Credits[Credit2.Key],
        "Piece2.Credits[Credit2.Key]");
      Assert.AreSame(Credit2, Baker.Credits[Credit2.Key],
        "Baker.Credits[Credit2.Key]");
      Assert.AreSame(Credit2, Drums.Credits[Credit2.Key],
        "Drums.Credits[Credit2.Key]");
    }

    [Test]
    public void ChangeRole() {
      Session.BeginUpdate();
      Credit2.Role = ElectricGuitar;
      Session.Commit();
      Assert.AreSame(ElectricGuitar, Credit2.Role, "Credit2.Role");
      Assert.AreEqual(2, Drums.Credits.Count, "Drums.Credits.Count");
      Assert.AreEqual(1, ElectricGuitar.Credits.Count,
        "ElectricGuitar.Credits.Count");
      Assert.AreSame(Credit2, ElectricGuitar.Credits[0],
        "ElectricGuitar 2nd Credit");
    }

    [Test]
    public void DisallowChangeCreditNoToDuplicate() {
      Session.BeginUpdate();
      Credit2.CreditNo = Credit2CreditNo;
      Assert.Throws<ConstraintException>(() =>
        Credit2.CreditNo = Credit1CreditNo);
      Session.Commit();
    }

    [Test]
    public void DisallowChangeCreditNoToZero() {
      Session.BeginUpdate();
      Assert.Throws<PropertyConstraintException>(() =>
        Credit2.CreditNo = 0);
      Session.Commit();
    }

    [Test]
    public void DisallowPersistUnspecifiedCreditNo() {
      var noCreditNo = new Credit {
        QueryHelper = QueryHelper
      };
      Session.BeginUpdate();
      noCreditNo.Piece = Piece1;
      Assert.Throws<PropertyConstraintException>(() => Session.Persist(noCreditNo));
      Session.Abort();
    }

    [Test]
    public void DisallowSetKeyToDuplicate() {
      var duplicate = new Credit {
        QueryHelper = QueryHelper,
        CreditNo = Credit1CreditNo
      };
      Session.BeginUpdate();
      Assert.Throws<ConstraintException>(() => duplicate.Piece = Piece1);
      Session.Commit();
    }

    [Test]
    public void Unpersist() {
      Session.BeginUpdate();
      Assert.DoesNotThrow(() => Session.Unpersist(Credit1));
      Session.Commit();
    }
  }
}