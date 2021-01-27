using System;
using System.Data;
using NUnit.Framework;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data {
  [TestFixture]
  public class LocationTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
      Data = new TestData(QueryHelper);
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
      using var session = new TestSession(DatabaseFolderPath);
      session.BeginUpdate();
      session.Persist(Location1);
      session.Persist(Location2);
      Event1.Location = Location1;
      Event2.Location = Location1;
      Data.AddEventTypesPersisted(1, session);
      Event1.EventType = Data.EventTypes[0];
      Event2.EventType = Event1.EventType;
      session.Persist(Event1);
      session.Persist(Event2);
      session.Commit();
    }

    [TearDown]
    public void TearDown() {
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }

    private const string Location1Name = "Fred's";
    private const string Location1Notes = "My notes.";
    private const string Location2Name = "Pyramid Club";
    private string DatabaseFolderPath { get; set; } = null!;
    private QueryHelper QueryHelper { get; set; } = null!;
    private TestData Data { get; set; } = null!;
    private Event Event1 { get; set; } = null!;
    private static DateTime Event1Date => DateTime.Today.AddDays(-1);
    private Event Event2 { get; set; } = null!;
    private static DateTime Event2Date => DateTime.Today;
    private Location Location1 { get; set; } = null!;
    private Location Location2 { get; set; } = null!;

    [Test]
    public void A010_Initial() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        Location1 = QueryHelper.Read<Location>(Location1Name, session);
        Location2 = QueryHelper.Read<Location>(Location2Name, session);
        Event1 = QueryHelper.Read<Event>(Event1.SimpleKey, Location1, session);
        session.Commit();
      }
      Assert.AreEqual(Location1Name, Location1.Name,
        "Location1.Name initially");
      Assert.AreEqual(Location1Notes, Location1.Notes,
        "Location1.Notes initially");
      Assert.AreEqual(Location2Name, Location2.Name,
        "Location2.Name initially");
      Assert.AreEqual(2, Location1.Events.Count,
        "Location1.Events.Count initially");
      Assert.AreSame(Location1, Event1.Location, "Event1.Location initially");
      Assert.AreEqual(Location1.Name, Event1.Location.Name,
        "Event1.Location.Name initially");
    }

    [Test]
    public void DisallowChangeNameToDuplicate() {
      using var session = new TestSession(DatabaseFolderPath);
      session.BeginUpdate();
      Location2 =
        QueryHelper.Read<Location>(Location2Name, session);
      Location2.Name = Location2Name;
      Assert.Throws<PropertyConstraintException>(() =>
        Location2.Name = Location1Name);
      session.Commit();
    }

    [Test]
    public void DisallowPersistDuplicate() {
      var duplicate = new Location {
        QueryHelper = QueryHelper,
        Name = Location1Name
      };
      using var session = new TestSession(DatabaseFolderPath);
      session.BeginUpdate();
      Assert.Throws<PropertyConstraintException>(() => session.Persist(duplicate));
      session.Commit();
    }

    [Test]
    public void DisallowPersistUnspecifiedName() {
      var noName = new Location {
        QueryHelper = QueryHelper
      };
      using var session = new TestSession(DatabaseFolderPath);
      session.BeginUpdate();
      Assert.Throws<PropertyConstraintException>(() => session.Persist(noName));
      session.Commit();
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
      using var session = new TestSession(DatabaseFolderPath);
      session.BeginUpdate();
      Assert.Throws<ConstraintException>(() =>
        Location1.Unpersist(session));
      session.Commit();
    }
  }
}