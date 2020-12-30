using System;
using System.Data;
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
      Data = new TestData(QueryHelper);
      Event1 = new Event {
        QueryHelper = QueryHelper,
        Date = Event1Date,
        Notes = Event1Notes
      };
      using var session = new TestSession(DatabaseFolderPath);
      session.BeginUpdate();
      Data.AddEventTypesPersisted(1, session);
      Data.AddLocationsPersisted(2, session);
      Event1.EventType = Data.EventTypes[0];
      Event1.Location = Data.Locations[0];
      session.Persist(Event1);
      session.Commit();
    }

    [TearDown]
    public void TearDown() {
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }

    private const string Event1Notes = "My event notes.";
    private const string Event1SimpleKey = "2013/04/11";
    private string DatabaseFolderPath { get; set; } = null!;
    private QueryHelper QueryHelper { get; set; } = null!;
    private TestData Data { get; set; } = null!;
    private Event Event1 { get; set; } = null!;
    private static DateTime Event1Date => DateTime.Parse(Event1SimpleKey);

    [Test]
    public void Find() {
      using var session = new TestSession(DatabaseFolderPath);
      session.BeginRead();
      var location1A =
        QueryHelper.Find<Location>(
          location => location.Name == Data.Locations[0].Name,
          session);
      var location1B = QueryHelper.Find<Location>(Data.Locations[0].Name, session);
      var event1A =
        QueryHelper.Find<Event>(
          @event => @event.Date == Event1Date &&
                    @event.Location.Name == Data.Locations[0].Name, session);
      var event1B =
        QueryHelper.Find<Event>(Event1SimpleKey, Data.Locations[0], session);
      session.Commit();
      Assert.IsNotNull(location1A, "location1A after Find by Name");
      Assert.IsNotNull(event1A,
        "event1A after Find by Date and Location.Name.");
      Assert.IsNotNull(location1B, "location1B after Find by SimpleKey");
      Assert.IsNotNull(event1B,
        "event1B after Find by SimpleKey and IdentifyingParent");
    }

    [Test]
    public void FindDuplicateSimpleKey() {
      Location found;
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        found =
          (QueryHelper.FindDuplicateSimpleKey(typeof(Location), new Oid(),
              Data.Locations[1].Name, session) as
            Location)!;
        session.Commit();
      }
      Assert.IsNotNull(found);
      Assert.AreEqual(Data.Locations[1].Name, found.Name);
    }

    [Test]
    public void Read() {
      using var session = new TestSession(DatabaseFolderPath);
      session.BeginRead();
      var location1A =
        QueryHelper.Read<Location>(
          location => location.Name == Data.Locations[0].Name,
          session);
      var location1B = QueryHelper.Read<Location>(Data.Locations[0].Name, session);
      var event1A =
        QueryHelper.Read<Event>(
          @event => @event.Date == Event1Date &&
                    @event.Location.Name == Data.Locations[0].Name, session);
      var event1B =
        QueryHelper.Read<Event>(Event1SimpleKey, Data.Locations[0], session);
      session.Commit();
      Assert.AreEqual(Data.Locations[0].Notes, location1A.Notes,
        "location1A.Notes after Read by Name");
      Assert.AreEqual(Event1Notes, event1A.Notes,
        "event1A.Notes after Read by Date and Location.Name.");
      Assert.AreEqual(Data.Locations[0].Notes, location1B.Notes,
        "location1B.Notes after Read by SimpleKey");
      Assert.AreEqual(Event1Notes, event1B.Notes,
        "event1B.Notes after Read by SimpleKey and IdentifyingParent");
    }

    [Test]
    public void ReadKeyNotFound() {
      using var session = new TestSession(DatabaseFolderPath);
      session.BeginRead();
      Data.EventTypes[0] =
        QueryHelper.Read<EventType>(Data.EventTypes[0].Name, session);
      Assert.Throws<ConstraintException>(() =>
        QueryHelper.Read<Event>(Event1SimpleKey, Data.EventTypes[0], session));
      session.Commit();
    }
  }
}