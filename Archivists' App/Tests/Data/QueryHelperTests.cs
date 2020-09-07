using System;
using System.Collections.Generic;
using NUnit.Framework;
using SoundExplorers.Data;
using VelocityDb;

namespace SoundExplorers.Tests.Data {
  [TestFixture]
  public class QueryHelperTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
      TestDataFactory = new TestDataFactory(QueryHelper);
      Event1 = new Event {
        QueryHelper = QueryHelper,
        Date = Event1Date,
        Notes = Event1Notes
      };
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        EventType1 = TestDataFactory.CreateEventTypePersisted(session);
        Location1 = TestDataFactory.CreateLocationPersisted(session);
        Location2 = TestDataFactory.CreateLocationPersisted(session);
        Event1.EventType = EventType1;
        Event1.Location = Location1;
        session.Persist(Event1);
        session.Commit();
      }
    }

    [TearDown]
    public void TearDown() {
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }

    private const string Event1Notes = "My event notes.";
    private const string Event1SimpleKey = "2013/04/11";
    private string DatabaseFolderPath { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private TestDataFactory TestDataFactory { get; set; }
    private Event Event1 { get; set; }
    private static DateTime Event1Date => DateTime.Parse(Event1SimpleKey);
    private EventType EventType1 { get; set; }
    private Location Location1 { get; set; }
    private Location Location2 { get; set; }

    [Test]
    public void Find() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        var location1A =
          QueryHelper.Find<Location>(
            location => location.Name == Location1.Name,
            session);
        var location1B = QueryHelper.Find<Location>(Location1.Name, session);
        var event1A =
          QueryHelper.Find<Event>(
            @event => @event.Date == Event1Date &&
                      @event.Location.Name == Location1.Name, session);
        var event1B =
          QueryHelper.Find<Event>(Event1SimpleKey, Location1, session);
        session.Commit();
        Assert.IsNotNull(location1A, "location1A after Find by Name");
        Assert.IsNotNull(event1A,
          "event1A after Find by Date and Location.Name.");
        Assert.IsNotNull(location1B, "location1B after Find by SimpleKey");
        Assert.IsNotNull(event1B,
          "event1B after Find by SimpleKey and IdentifyingParent");
      }
    }

    [Test]
    public void FindDuplicateSimpleKey() {
      Location found;
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        found =
          QueryHelper.FindDuplicateSimpleKey(typeof(Location), new Oid(),
              Location2.Name, session) as
            Location;
        session.Commit();
      }
      Assert.IsNotNull(found);
      Assert.AreEqual(Location2.Name, found.Name);
    }

    [Test]
    public void Read() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        var location1A =
          QueryHelper.Read<Location>(
            location => location.Name == Location1.Name,
            session);
        var location1B = QueryHelper.Read<Location>(Location1.Name, session);
        var event1A =
          QueryHelper.Read<Event>(
            @event => @event.Date == Event1Date &&
                      @event.Location.Name == Location1.Name, session);
        var event1B =
          QueryHelper.Read<Event>(Event1SimpleKey, Location1, session);
        session.Commit();
        Assert.AreEqual(Location1.Notes, location1A.Notes,
          "location1A.Notes after Read by Name");
        Assert.AreEqual(Event1Notes, event1A.Notes,
          "event1A.Notes after Read by Date and Location.Name.");
        Assert.AreEqual(Location1.Notes, location1B.Notes,
          "location1B.Notes after Read by SimpleKey");
        Assert.AreEqual(Event1Notes, event1B.Notes,
          "event1B.Notes after Read by SimpleKey and IdentifyingParent");
      }
    }

    [Test]
    public void ReadKeyNotFound() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        EventType1 = QueryHelper.Read<EventType>(EventType1.Name, session);
        Assert.Throws<KeyNotFoundException>(() =>
          QueryHelper.Read<Event>(Event1SimpleKey, EventType1, session));
        session.Commit();
      }
    }
  }
}