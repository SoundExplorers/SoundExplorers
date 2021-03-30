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
      Session = new TestSession(DatabaseFolderPath);
      Session.BeginUpdate();
      Data.AddRootsPersistedIfRequired(Session);
      Session.Commit();
      DefaultNewsletter = Newsletter.CreateDefault(Data.NewsletterRoot);
      DefaultSeries = Series.CreateDefault(Data.SeriesRoot);
      Location1 = new Location(Data.LocationRoot) {
        QueryHelper = QueryHelper,
        Name = Location1Name,
        Notes = Location1Notes
      };
      Location2 = new Location(Data.LocationRoot) {
        QueryHelper = QueryHelper,
        Name = Location2Name
      };
      Event1 = new Event(Data.EventRoot) {
        QueryHelper = QueryHelper,
        Date = Event1Date
      };
      Event2 = new Event(Data.EventRoot) {
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
    private TestSession Session { get; set; } = null!;
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
      var duplicate = new Location(Data.LocationRoot) {
        QueryHelper = QueryHelper,
        Name = Location1Name
      };
      Session.BeginUpdate();
      Assert.Throws<PropertyConstraintException>(() => Session.Persist(duplicate));
      Session.Commit();
    }

    [Test]
    public void DisallowPersistUnspecifiedName() {
      var noName = new Location(Data.LocationRoot) {
        QueryHelper = QueryHelper
      };
      Session.BeginUpdate();
      Assert.Throws<PropertyConstraintException>(() => Session.Persist(noName));
      Session.Commit();
    }

    [Test]
    public void DisallowSetNullName() {
      var nullName = new Location(Data.LocationRoot) {
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