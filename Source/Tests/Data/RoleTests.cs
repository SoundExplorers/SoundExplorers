﻿using System;
using System.Data;
using NUnit.Framework;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data; 

[TestFixture]
public class RoleTests : TestFixtureBase {
  [SetUp]
  public override void Setup() {
    base.Setup();
    DefaultAct = Act.CreateDefault();
    DefaultNewsletter = Newsletter.CreateDefault();
    DefaultSeries = Series.CreateDefault();
    Drums = new Role {
      QueryHelper = QueryHelper,
      Name = DrumsName
    };
    ElectricGuitar = new Role {
      QueryHelper = QueryHelper,
      Name = ElectricGuitarName
    };
    Artist1 = new Artist {
      QueryHelper = QueryHelper,
      Forename = Artist1Forename,
      Surname = Artist1Surname
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
    Piece1 = new TestPiece {
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
    Session.BeginUpdate();
    Session.Persist(DefaultAct);
    Session.Persist(DefaultNewsletter);
    Session.Persist(DefaultSeries);
    Session.Persist(Drums);
    Session.Persist(ElectricGuitar);
    Session.Persist(Artist1);
    Session.Persist(Location1);
    Event1.Location = Location1;
    Data.AddEventTypesPersisted(1, Session);
    Event1.EventType = Data.EventTypes[0];
    Session.Persist(Event1);
    Set1.Event = Event1;
    Data.AddGenresPersisted(1, Session);
    Set1.Genre = Data.Genres[0];
    Session.Persist(Set1);
    Piece1.Set = Set1;
    Session.Persist(Piece1);
    Credit1.Artist = Artist1;
    Credit1.Piece = Piece1;
    Credit1.Role = Drums;
    Credit2.Artist = Artist1;
    Credit2.Piece = Piece1;
    Credit2.Role = Drums;
    Session.Persist(Credit1);
    Session.Persist(Credit2);
    Session.Commit();
    Session.BeginRead();
    Drums = QueryHelper.Read<Role>(DrumsName, Session);
    ElectricGuitar = QueryHelper.Read<Role>(ElectricGuitarName, Session);
    Credit1 = QueryHelper.Read<Credit>(Credit1.SimpleKey, Piece1, Session);
    Credit2 = QueryHelper.Read<Credit>(Credit2.SimpleKey, Piece1, Session);
    Session.Commit();
  }

  private const string DrumsName = "Drums";
  private const string ElectricGuitarName = "Electric guitar";
  private const string Artist1Forename = "Ralph";
  private const string Artist1Surname = "Jenkins";
  private const int Credit1CreditNo = 1;
  private const int Credit2CreditNo = 2;
  private const string Location1Name = "Pyramid Club";
  private const int Piece1PieceNo = 1;
  private const int Set1SetNo = 1;
  private Act DefaultAct { get; set; } = null!;
  private Newsletter DefaultNewsletter { get; set; } = null!;
  private Series DefaultSeries { get; set; } = null!;
  private Role Drums { get; set; } = null!;
  private Role ElectricGuitar { get; set; } = null!;
  private Artist Artist1 { get; set; } = null!;
  private Credit Credit1 { get; set; } = null!;
  private Credit Credit2 { get; set; } = null!;
  private Event Event1 { get; set; } = null!;
  private static DateTime Event1Date => DateTime.Today.AddDays(-1);
  private Location Location1 { get; set; } = null!;
  private Piece Piece1 { get; set; } = null!;
  private Set Set1 { get; set; } = null!;

  [Test]
  public void A010_Initial() {
    Assert.AreEqual(DrumsName, Drums.Name, "Drums.Name initially");
    Assert.AreEqual(ElectricGuitarName, ElectricGuitar.Name,
      "ElectricGuitar.Name initially");
    Assert.AreEqual(2, Drums.Credits.Count, "Drums.Credits.Count");
    Assert.AreEqual(2, Drums.References.Count, "Drums.References.Count");
    Session.BeginRead();
    Drums = QueryHelper.Read<Role>(DrumsName, Session);
    Credit1 = QueryHelper.Read<Credit>(Credit1.SimpleKey, Piece1, Session);
    Credit2 = QueryHelper.Read<Credit>(Credit2.SimpleKey, Piece1, Session);
    Assert.AreSame(Credit1, Drums.Credits[0], "Drums.Credits[0]");
    Assert.AreSame(Credit2, Drums.Credits[1], "Drums.Credits[1]");
    Assert.AreSame(Drums, Credit1.Role, "Credit1.Role");
    Assert.AreEqual(DrumsName, Credit1.Role.Name, "Credit1.Role.Name");
    Assert.AreSame(Drums, Credit2.Role, "Credit2.Role");
    Session.Commit();
  }

  [Test]
  public void DisallowDuplicate() {
    var duplicate = new Role {
      QueryHelper = QueryHelper,
      Name = "drums" // Tests that comparison is case-insensitive.
    };
    Session.BeginUpdate();
    Assert.Throws<PropertyConstraintException>(() => Session.Persist(duplicate));
    Session.Commit();
  }

  [Test]
  public void DisallowUnpersistRoleWithCredits() {
    Session.BeginUpdate();
    Assert.Throws<ConstraintException>(() => Drums.Unpersist(Session));
    Session.Commit();
  }

  [Test]
  public void Unpersist() {
    Session.BeginUpdate();
    Assert.DoesNotThrow(() => Session.Unpersist(ElectricGuitar));
    Session.Commit();
  }
}