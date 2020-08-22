﻿using System;
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
      Role1 = new Role {
        QueryHelper = QueryHelper,
        Name = Role1Name
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
        session.Persist(RalphJenkins);
        session.Persist(Clarissa);
        session.Persist(Baker);
        session.Persist(Role1);
        session.Persist(Location1);
        Event1.Location = Location1;
        session.Persist(Event1);
        Set1.Event = Event1;
        session.Persist(Set1);
        Piece1.Set = Set1;
        session.Persist(Piece1);
        RalphJenkins.Credits.Add(Credit1);
        RalphJenkins.Credits.Add(Credit2);
        Role1.Credits.Add(Credit1);
        Role1.Credits.Add(Credit2);
        Piece1.Credits.Add(Credit1);
        Piece1.Credits.Add(Credit2);
        session.Persist(Credit1);
        session.Persist(Credit2);
        session.Commit();
      }
      Session = new TestSession(DatabaseFolderPath);
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
    private const string RalphJenkinsName = "Ralph Jenkins";
    private const string RalphJenkinsSurname = " Jenkins ";
    private const string RalphJenkinsNotes = "My notes.";
    private const int Credit1CreditNo = 1;
    private const int Credit2CreditNo = 2;
    private const string Location1Name = "Pyramid Club";
    private const int Piece1PieceNo = 1;
    private const string Role1Name = "Banjo";
    private const int Set1SetNo = 1;
    
    private string DatabaseFolderPath { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private TestSession Session { get; set; }
    private Artist RalphJenkins { get; set; }
    private Artist Clarissa { get; set; }
    private Artist Baker { get; set; }
    private Credit Credit1 { get; set; }
    private Credit Credit2 { get; set; }
    private Event Event1 { get; set; }
    private static DateTime Event1Date => DateTime.Today.AddDays(-1);
    private Location Location1 { get; set; }
    private Piece Piece1 { get; set; }
    private Role Role1 { get; set; }
    private Set Set1 { get; set; }

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
      Assert.AreSame(Credit1, RalphJenkins.Credits[0],
        "RalphJenkins.Credits[0]");
      Assert.AreSame(Credit2, RalphJenkins.Credits[1],
        "RalphJenkins.Credits[1]");
      Assert.AreSame(RalphJenkins, Credit1.Artist, "Credit1.Artist");
      Assert.AreEqual(RalphJenkinsName, Credit1.Artist.Name,
        "Credit1.Artist.Name");
      Assert.AreSame(RalphJenkins, Credit2.Artist, "Credit2.Artist");
    }

    [Test]
    public void DisallowDuplicate() {
      var duplicate = new Artist {
        QueryHelper = QueryHelper,
        // Clarissa has this as a Forename and has no Surname.
        // So the generated Name / SimpleKey should be the same
        // for both Artists and this one should be a duplicate.
        Surname = ClarissaName
      };
      Session.BeginUpdate();
      Assert.Throws<DuplicateKeyException>(() => Session.Persist(duplicate));
      Session.Commit();
    }

    [Test]
    public void DisallowRemoveCredit() {
      Session.BeginUpdate();
      Assert.Throws<ConstraintException>(() =>
        RalphJenkins.Credits.Remove(Credit1));
      Session.Commit();
    }

    [Test]
    public void DisallowSetNameToNull() {
      var nameless = new Artist();
      Assert.Throws<NoNullAllowedException>(() => nameless.Forename = null);
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