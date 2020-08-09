using System;
using System.Data;
using System.Data.Linq;
using NUnit.Framework;
using SoundExplorersDatabase.Data;
using VelocityDb.Exceptions;

namespace SoundExplorersDatabase.Tests.Data {
  [TestFixture]
  public class LocationTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
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
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        session.Persist(Location1);
        session.Persist(Location2);
        Location1.Events.Add(Event1);
        Location1.Events.Add(Event2);
        session.Persist(Event1);
        session.Persist(Event2);
        session.Commit();
      }
    }

    [TearDown]
    public void TearDown() {
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }

    private const string Location1Name = "Fred's";
    private const string Location1Notes = "My notes.";
    private const string Location2Name = "Pyramid Club";

    private string DatabaseFolderPath { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private Event Event1 { get; set; }
    private DateTime Event1Date => DateTime.Today.AddDays(-1);
    private Event Event2 { get; set; }
    private DateTime Event2Date => DateTime.Today;
    private Location Location1 { get; set; }
    private Location Location2 { get; set; }

    [Test]
    public void T010_Initial() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        Location1 = QueryHelper.Read<Location>(Location1Name, session);
        Location2 = QueryHelper.Read<Location>(Location2Name, session);
        Event1 = QueryHelper.Read<Event>(Event1.Key, Location1, session);
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
    public void T020_DisallowDuplicate() {
      var duplicate = new Location {
        QueryHelper = QueryHelper,
        Name = Location1Name
      };
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Assert.Throws<DuplicateKeyException>(() =>
          session.Persist(duplicate), "Duplicate");
        session.Commit();
      }
    }

    [Test]
    public void T030_DisallowUnpersistLocationWithEvents() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Assert.Throws<ReferentialIntegrityException>(() =>
          Location1.Unpersist(session));
        session.Commit();
      }
    }

    [Test]
    public void T040_DisallowRemoveEvent() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Location1 = QueryHelper.Read<Location>(Location1Name, session);
        Event1 = QueryHelper.Read<Event>(Event1.Key, Location1, session);
        Assert.Throws<ConstraintException>(() =>
            Location1.Events.Remove(Event1),
          "Disallow remove Event from mandatory link to Location.");
        session.Commit();
      }
    }
  }
}