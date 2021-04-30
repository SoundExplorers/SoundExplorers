using System;
using System.Data;
using NUnit.Framework;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data {
  [TestFixture]
  public class LocationTests : TestFixtureBase {
    [SetUp]
    public override void Setup() {
      base.Setup();
      DefaultNewsletter = Newsletter.CreateDefault();
      DefaultSeries = Series.CreateDefault();
      Location1 = new Location {
        QueryHelper = QueryHelper,
        Name = Location1Name,
        Notes = Location1Notes
      };
      Location2 = new Location {
        QueryHelper = QueryHelper,
        Name = Location2Name
      };
      Event1 = new Event {
        QueryHelper = QueryHelper,
        Date = Event1Date
      };
      Event2 = new Event {
        QueryHelper = QueryHelper,
        Date = Event2Date
      };
      Session.BeginUpdate();
      Session.Persist(DefaultNewsletter);
      Session.Persist(DefaultSeries);
      Session.Persist(Location1);
      Session.Persist(Location2);
      Event1.Location = Location1;
      Event2.Location = Location1;
      Data.AddEventTypesPersisted(1, Session);
      Event1.EventType = Data.EventTypes[0];
      Event2.EventType = Event1.EventType;
      Session.Persist(Event1);
      Session.Persist(Event2);
      Session.Commit();
    }

    private const string Location1Name = "Fred's";
    private const string Location1Notes = "My notes.";
    private const string Location2Name = "Pyramid Club";
    private Newsletter DefaultNewsletter { get; set; } = null!;
    private Series DefaultSeries { get; set; } = null!;
    private Event Event1 { get; set; } = null!;
    private static DateTime Event1Date => DateTime.Today.AddDays(-1);
    private Event Event2 { get; set; } = null!;
    private static DateTime Event2Date => DateTime.Today;
    private Location Location1 { get; set; } = null!;
    private Location Location2 { get; set; } = null!;

    [Test]
    public void A010_Initial() {
      Session.BeginRead();
      Location1 = QueryHelper.Read<Location>(Location1Name, Session);
      Location2 = QueryHelper.Read<Location>(Location2Name, Session);
      Event1 = QueryHelper.Read<Event>(Event1.SimpleKey, Location1, Session);
      Session.Commit();
      Assert.AreEqual(Location1Name, Location1.Name,
        "Location1.Name initially");
      Assert.AreEqual(Location1Notes, Location1.Notes,
        "Location1.Notes initially");
      Assert.AreEqual(Location2Name, Location2.Name,
        "Location2.Name initially");
      Assert.IsFalse(Location1.Events.AllowOtherTypesOnSamePage,
        "Location1.Events.AllowOtherTypesOnSamePage");
      Assert.AreEqual(2, Location1.Events.Count,
        "Location1.Events.Count initially");
      Assert.AreSame(Location1, Event1.Location, "Event1.Location initially");
      Assert.AreEqual(Location1.Name, Event1.Location.Name,
        "Event1.Location.Name initially");
    }

    [Test]
    public void DisallowChangeNameToDuplicate() {
      Session.BeginUpdate();
      Location2 =
        QueryHelper.Read<Location>(Location2Name, Session);
      Location2.Name = Location2Name;
      Assert.Throws<PropertyConstraintException>(() =>
        Location2.Name = Location1Name);
      Session.Commit();
    }

    [Test]
    public void DisallowPersistDuplicate() {
      var duplicate = new Location {
        QueryHelper = QueryHelper,
        Name = Location1Name
      };
      Session.BeginUpdate();
      Assert.Throws<PropertyConstraintException>(() => Session.Persist(duplicate));
      Session.Commit();
    }

    [Test]
    public void DisallowPersistUnspecifiedName() {
      var noName = new Location {
        QueryHelper = QueryHelper
      };
      Session.BeginUpdate();
      Assert.Throws<PropertyConstraintException>(() => Session.Persist(noName));
      Session.Commit();
    }

    [Test]
    public void DisallowSetNullName() {
      var nullName = new Location {
        QueryHelper = QueryHelper
      };
      Assert.Throws<PropertyConstraintException>(() => nullName.Name = null!);
    }

    [Test]
    public void DisallowUnpersistLocationWithEvents() {
      Session.BeginUpdate();
      Assert.Throws<ConstraintException>(() =>
        Location1.Unpersist(Session));
      Session.Commit();
    }
  }
}