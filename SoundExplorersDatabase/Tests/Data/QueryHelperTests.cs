using System;
using NUnit.Framework;
using SoundExplorersDatabase.Data;

namespace SoundExplorersDatabase.Tests.Data {
  [TestFixture]
  public class QueryHelperTests {
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
        Date = Event1Date,
        Notes = Event1Notes
      };
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        session.Persist(Location1);
        session.Persist(Location2);
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
    private const string Location1Name = "Fred's";
    private const string Location1Notes = "My location notes.";
    private const string Location2Name = "Pyramid Club";
    private string DatabaseFolderPath { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private Event Event1 { get; set; }
    private static DateTime Event1Date => DateTime.Parse(Event1SimpleKey);
    private Location Location1 { get; set; }
    private Location Location2 { get; set; }

    [Test]
    public void T010_Find() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        var location1A =
          QueryHelper.Find<Location>(location => location.Name == Location1Name,
            session);
        var location1B = QueryHelper.Find<Location>(Location1Name, session);
        var event1A =
          QueryHelper.Find<Event>(
            @event => @event.Date == Event1Date &&
                      @event.Location.Name == Location1Name, session);
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
    public void T020_Read() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        var location1A =
          QueryHelper.Read<Location>(location => location.Name == Location1Name,
            session);
        var location1B = QueryHelper.Read<Location>(Location1Name, session);
        var event1A =
          QueryHelper.Read<Event>(
            @event => @event.Date == Event1Date &&
                      @event.Location.Name == Location1Name, session);
        var event1B =
          QueryHelper.Read<Event>(Event1SimpleKey, Location1, session);
        session.Commit();
        Assert.AreEqual(Location1Notes, location1A.Notes,
          "location1A.Notes after Read by Name");
        Assert.AreEqual(Event1Notes, event1A.Notes,
          "event1A.Notes after Read by Date and Location.Name.");
        Assert.AreEqual(Location1Notes, location1B.Notes,
          "location1B.Notes after Read by SimpleKey");
        Assert.AreEqual(Event1Notes, event1B.Notes,
          "event1B.Notes after Read by SimpleKey and IdentifyingParent");
      }
    }

    [Test]
    public void T030_FindWithSameSimpleKey() {
      var duplicate = new Location {
        QueryHelper = QueryHelper,
        Name = Location2Name
      };
      Location found;
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        found =
          QueryHelper.FindWithSameSimpleKey(duplicate, session) as Location;
        session.Commit();
      }
      Assert.IsNotNull(found);
      Assert.AreEqual(Location2Name, found.Name);
    }
  }
}