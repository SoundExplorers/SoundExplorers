﻿using System;
using NUnit.Framework;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data; 

[TestFixture]
public class ActTests : TestFixtureBase {
  [SetUp]
  public override void Setup() {
    base.Setup();
    DefaultAct = Act.CreateDefault();
    DefaultNewsletter = Newsletter.CreateDefault();
    DefaultSeries = Series.CreateDefault();
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
    Session.BeginUpdate();
    Session.Persist(DefaultAct);
    Session.Persist(DefaultNewsletter);
    Session.Persist(DefaultSeries);
    Session.Persist(Location1);
    Data.AddEventTypesPersisted(1, Session);
    Event1.Location = Location1;
    Event1.EventType = Data.EventTypes[0];
    Session.Persist(Event1);
    Session.Persist(Act1);
    Session.Persist(Act2);
    Set1.Event = Event1;
    Set2.Event = Event1;
    Data.AddGenresPersisted(1, Session);
    Set1.Genre = Data.Genres[0];
    Set2.Genre = Set1.Genre;
    Session.Persist(Set1);
    Session.Persist(Set2);
    Set1.Act = Act1;
    Session.Commit();
  }

  private const string Act1Name = "Ewan Husami";
  private const string Act1Notes = "My notes.";
  private const string Act2Name = "Ivanhoe Britches";
  private const string Event1Key = "2012/03/04";
  private const string Location1Name = "Pyramid Club";
  private const int Set1SetNo = 1;
  private const int Set2SetNo = 2;
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
    Session.BeginRead();
    DefaultAct = QueryHelper.Read<Act>(DefaultAct.Name, Session);
    Act1 = QueryHelper.Read<Act>(Act1Name, Session);
    Act2 = QueryHelper.Read<Act>(Act2Name, Session);
    Set1 = QueryHelper.Read<Set>(Set1.SimpleKey, Event1, Session);
    Set2 = QueryHelper.Read<Set>(Set2.SimpleKey, Event1, Session);
    Session.Commit();
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
    Session.BeginUpdate();
    Assert.DoesNotThrow(() => Session.Persist(noName));
    Session.Commit();
  }

  [Test]
  public void DisallowChangeNameToDuplicate() {
    Session.BeginUpdate();
    Act2 =
      QueryHelper.Read<Act>(Act2Name, Session);
    Act2.Name = Act2Name;
    Assert.Throws<PropertyConstraintException>(() => Act2.Name = Act1Name);
    Session.Commit();
  }

  [Test]
  public void DisallowChangeNameToNull() {
    Session.BeginUpdate();
    Act1 = QueryHelper.Read<Act>(Act1Name, Session);
    Assert.Throws<PropertyConstraintException>(() => Act1.Name = string.Empty);
    Session.Commit();
  }

  [Test]
  public void DisallowPersistDuplicate() {
    var duplicate = new Act {
      QueryHelper = QueryHelper,
      Name = Act1Name
    };
    Session.BeginUpdate();
    Assert.Throws<PropertyConstraintException>(() => Session.Persist(duplicate));
    Session.Commit();
  }
}