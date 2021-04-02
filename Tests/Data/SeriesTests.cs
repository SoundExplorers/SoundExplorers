using System;
using NUnit.Framework;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data {
  [TestFixture]
  public class SeriesTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      Data = new TestData(QueryHelper);
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
      Session = new TestSession(DatabaseFolderPath);
      Session.BeginUpdate();
      Data.AddRootsPersistedIfRequired(Session);
      Session.Commit();
      DefaultNewsletter = Newsletter.CreateDefault();
      DefaultSeries = Series.CreateDefault();
      Location1 = new Location {
        QueryHelper = QueryHelper,
        Name = Location1Name
      };
      Series1 = new Series {
        QueryHelper = QueryHelper,
        Name = Series1Name,
        Notes = Series1Notes
      };
      Series2 = new Series {
        QueryHelper = QueryHelper,
        Name = Series2Name
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
      Session.Persist(Series1);
      Session.Persist(Series2);
      Event1.Location = Location1;
      Event1.Series = Series1;
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

    private const string Location1Name = "Pyramid Club";
    private const string Series1Name = "Jazz Festival 2014";
    private const string Series1Notes = "My notes.";
    private const string Series2Name = "Field Recordings";
    private QueryHelper QueryHelper { get; set; } = null!;
    private TestData Data { get; set; } = null!;
    private string DatabaseFolderPath { get; set; } = null!;
    private TestSession Session { get; set; } = null!;
    private Newsletter DefaultNewsletter { get; set; } = null!;
    private Series DefaultSeries { get; set; } = null!;
    private Event Event1 { get; set; } = null!;
    private static DateTime Event1Date => DateTime.Today.AddDays(-1);
    private Event Event2 { get; set; } = null!;
    private static DateTime Event2Date => DateTime.Today;
    private Location Location1 { get; set; } = null!;
    private Series Series1 { get; set; } = null!;
    private Series Series2 { get; set; } = null!;

    [Test]
    public void T010_Initial() {
      Session.BeginRead();
      DefaultSeries = QueryHelper.Read<Series>(DefaultSeries.Name, Session);
      Series1 = QueryHelper.Read<Series>(Series1Name, Session);
      Series2 = QueryHelper.Read<Series>(Series2Name, Session);
      Event1 = QueryHelper.Read<Event>(Event1.SimpleKey, Location1, Session);
      Event2 = QueryHelper.Read<Event>(Event2.SimpleKey, Location1, Session);
      Session.Commit();
      Assert.AreEqual(Series1Name, Series1.Name, "Series1.Name initially");
      Assert.AreEqual(Series1Notes, Series1.Notes, "Series1.Notes initially");
      Assert.AreEqual(Series2Name, Series2.Name, "Series2.Name initially");
      Assert.AreEqual(1, Series1.Events.Count,
        "Series1.Events.Count initially");
      Assert.AreSame(Series1, Event1.Series, "Event1.Series initially");
      Assert.AreEqual(Series1.Name, Event1.Series.Name,
        "Event1.Series.Name initially");
      Assert.AreSame(DefaultSeries, Event2.Series, "Event2.Series initially");
    }

    [Test]
    public void T030_DisallowDuplicate() {
      var duplicate = new Series {
        QueryHelper = QueryHelper,
        Name = Series1Name
      };
      Session.BeginUpdate();
      Assert.Throws<PropertyConstraintException>(() =>
        Session.Persist(duplicate), "Duplicate");
      Session.Commit();
    }
  }
}