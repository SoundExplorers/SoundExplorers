using System;
using System.Data;
using System.Data.Linq;
using NUnit.Framework;
using SoundExplorersDatabase.Data;
using VelocityDb.Exceptions;

namespace SoundExplorersDatabase.Tests.Data {
  [TestFixture]
  public class EventTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
      Location1 = new Location {
        QueryHelper = QueryHelper,
        Name = Location1Name
      };
      Location2 = new Location {
        QueryHelper = QueryHelper,
        Name = Location2Name
      };
      Newsletter1 = new Newsletter {
        QueryHelper = QueryHelper,
        Date = Newsletter1Date,
        Path = Newsletter1Path
      };
      Newsletter2 = new Newsletter {
        QueryHelper = QueryHelper,
        Date = Newsletter2Date,
        Path = Newsletter2Path
      };
      Series1 = new Series {
        QueryHelper = QueryHelper,
        Name = Series1Name
      };
      Series2 = new Series {
        QueryHelper = QueryHelper,
        Name = Series2Name
      };
      Event1 = new Event {
        QueryHelper = QueryHelper,
        Date = Event1Date,
        Notes = Event1Notes
      };
      Event1AtLocation2 = new Event {
        QueryHelper = QueryHelper,
        Date = Event1Date
      };
      Event2 = new Event {
        QueryHelper = QueryHelper,
        Date = Event2Date
      };
      Set1 = new Set {
        QueryHelper = QueryHelper,
        SetNo = Set1SetNo
      };
      Set2 = new Set {
        QueryHelper = QueryHelper,
        SetNo = Set2SetNo
      };
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        session.Persist(Location1);
        session.Persist(Location2);
        session.Persist(Newsletter1);
        session.Persist(Newsletter2);
        session.Persist(Series1);
        session.Persist(Series2);
        Event2.Location = Location1;
        Event1.Location = Location1;
        Event1AtLocation2.Location = Location2;
        session.Persist(Event1);
        session.Persist(Event1AtLocation2);
        session.Persist(Event2);
        Event1.Newsletter = Newsletter1;
        Event1AtLocation2.Newsletter = Newsletter2;
        Event1AtLocation2.Series = Series2;
        Event1.Sets.Add(Set1);
        Event1.Sets.Add(Set2);
        session.Persist(Set1);
        session.Persist(Set2);
        session.Commit();
      }
    }

    [TearDown]
    public void Teardown() {
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }

    private const string Event1SimpleKey = "2013/04/11";
    private const string Event1Notes = "My notes.";
    private const string Event2SimpleKey = "2016/07/14";
    private const string Location1Name = "Fred's";
    private const string Location2Name = "Pyramid Club";
    private const string Newsletter1Key = "2013/04/05";
    private const string Newsletter1Path = "Path One";
    private const string Newsletter2Key = "2016/07/08";
    private const string Newsletter2Path = "Path Two";
    private const string Series1Name = "Jazz Festival 2014";
    private const string Series2Name = "Field Recordings";
    private const int Set1SetNo = 1;
    private const int Set2SetNo = 2;
    private string DatabaseFolderPath { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private Event Event1 { get; set; }
    private static DateTime Event1Date => DateTime.Parse(Event1SimpleKey);
    private Event Event1AtLocation2 { get; set; }
    private Event Event2 { get; set; }
    private static DateTime Event2Date => DateTime.Parse(Event2SimpleKey);
    private Location Location1 { get; set; }
    private Location Location2 { get; set; }
    private Newsletter Newsletter1 { get; set; }

    private static readonly DateTime Newsletter1Date =
      DateTime.Parse(Newsletter1Key);

    private Newsletter Newsletter2 { get; set; }

    private static readonly DateTime Newsletter2Date =
      DateTime.Parse(Newsletter2Key);

    private Series Series1 { get; set; }
    private Series Series2 { get; set; }
    private Set Set1 { get; set; }
    private Set Set2 { get; set; }

    [Test]
    public void T010_Initial() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        Event1 = QueryHelper.Read<Event>(Event1SimpleKey, Location1, session);
        Event1AtLocation2 =
          QueryHelper.Read<Event>(Event1SimpleKey, Location2, session);
        Event2 = QueryHelper.Read<Event>(Event2SimpleKey, Location1, session);
        Location1 = QueryHelper.Read<Location>(Location1Name, session);
        Location2 = QueryHelper.Read<Location>(Location2Name, session);
        Newsletter1 = QueryHelper.Read<Newsletter>(Newsletter1Key, session);
        Series1 = QueryHelper.Read<Series>(Series1Name, session);
        Set1 = QueryHelper.Read<Set>(Set1.SimpleKey, Event1, session);
        Set2 = QueryHelper.Read<Set>(Set2.SimpleKey, Event1, session);
        session.Commit();
      }
      Assert.AreEqual(Event1Date, Event1.Date, "Event1.Date");
      Assert.AreEqual(Event1Notes, Event1.Notes, "Event1.Notes");
      Assert.AreSame(Location1, Event1.Location, "Event1.Location");
      Assert.AreEqual(2, Location1.Events.Count, "Location1.Events.Count");
      Assert.AreEqual(2, Location1.References.Count,
        "Location1.References.Count");
      Assert.AreSame(Event1, Location1.Events[0], "Location1.Events[0]");
      Assert.AreSame(Event2, Location1.Events[1], "Location1.Events[1]");
      Assert.AreSame(Newsletter1, Event1.Newsletter, "Event1.Newsletter");
      Assert.AreEqual(1, Newsletter1.Events.Count, "Newsletter1.Events.Count");
      Assert.AreEqual(1, Newsletter1.References.Count,
        "Newsletter1.References.Count");
      Assert.AreSame(Event1, Newsletter1.Events[0], "Newsletter1.Events[0]");
      Assert.AreEqual(1, Newsletter2.Events.Count, "Newsletter2.Events.Count");
      Assert.AreEqual(1, Newsletter2.References.Count,
        "Newsletter2.References.Count");
      Assert.IsNull(Event1.Series, "Event1.Series");
      Assert.AreEqual(0, Series1.Events.Count, "Series1.Events.Count");
      Assert.AreEqual(0, Series1.References.Count, "Series1.Events.Count");
      Assert.AreEqual(1, Series2.Events.Count, "Series2.Events.Count");
      Assert.AreEqual(1, Series2.References.Count, "Series2.References.Count");
      Assert.AreEqual(Event2Date, Event2.Date, "Event1.Date");
      Assert.AreSame(Location1, Event2.Location, "Event2.Location");
      Assert.AreEqual(1, Location2.Events.Count, "Location2.Events.Count");
      Assert.AreEqual(1, Location2.References.Count,
        "Location2.References.Count");
      Assert.AreSame(Event1AtLocation2, Location2.Events[0],
        "Location2.Events[1]");
      Assert.AreEqual(2, Event1.Sets.Count, "Event1.Sets.Count");
      Assert.AreEqual(2, Event1.References.Count, "Event1.References.Count");
      Assert.AreSame(Event1, Set1.Event, "Set1.Event");
      Assert.AreEqual(Event1.Date, Set1.Event?.Date,
        "Set1.Event.Date initially");
      Assert.AreSame(Event1, Set2.Event, "Set2.Event");
      Assert.AreEqual(Event1.Date, Set2.Event.Date, "Set2.Event.Date");
    }

    [Test]
    public void T020_DisallowDuplicate() {
      var duplicate = new Event {
        QueryHelper = QueryHelper,
        Date = Event1Date
      };
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Location1 = QueryHelper.Read<Location>(Location1Name, session);
        Assert.Throws<DuplicateKeyException>(
          () => duplicate.Location = Location1, "Duplicate not allowed");
        session.Commit();
      }
    }

    [Test]
    public void T030_DisallowSetLocationToNull() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Location1 = QueryHelper.Read<Location>(Location1Name, session);
        Event1 = QueryHelper.Read<Event>(Event1SimpleKey, Location1, session);
        Assert.Throws<NoNullAllowedException>(() =>
            // ReSharper disable once AssignNullToNotNullAttribute
            Event1.Location = null,
          "Disallow remove Event from mandatory link to Location.");
        session.Commit();
      }
    }

    /// <summary>
    ///   Adds an Event to a Series that already includes an Event
    ///   on the same Date at a different Location.
    /// </summary>
    [Test]
    public void T040_AddToSeries() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Series2 = QueryHelper.Read<Series>(Series2Name, session);
        Event1 = QueryHelper.Read<Event>(Event1SimpleKey, Location1, session);
        Event1.Series = Series2;
        session.Commit();
        Assert.AreSame(Series2, Event1.Series,
          "Event1.Series after Event1 added to Series");
        Assert.AreEqual(2, Series2.Events.Count,
          "Series2.Events.Count after Event1 added to Series");
        Assert.AreSame(Event1, Series2.Events[0],
          "Series2 1st Event after Event1 added to Series");
      }
    }

    [Test]
    public void T050_ChangeLocation() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Location1 = QueryHelper.Read<Location>(Location1Name, session);
        Location2 = QueryHelper.Read<Location>(Location2Name, session);
        Event1 = Location1.Events[0];
        Event1AtLocation2 = Location2.Events[0];
        Event2 = Location1.Events[1];
        Event2.Location = Location2;
        session.Commit();
        Assert.AreSame(Location2, Event2.Location,
          "Event2.Location after Event2 changes Location");
        Assert.AreEqual(1, Location1.Events.Count,
          "Location1.Events.Count after Event2 changes Location");
        Assert.AreEqual(2, Location2.Events.Count,
          "Location2.Events.Count after Event2 changes Location");
        Assert.AreSame(Event1AtLocation2, Location2.Events[0],
          "Location2 1st Event after Event2 changes Location");
        Assert.AreSame(Event2, Location2.Events[1],
          "Location2 2nd Event after Event1 changes Location");
      }
    }

    /// <summary>
    ///   Changes an Event's NewsLetter to one that already covers an Event
    ///   on the same Date at a different Location.
    /// </summary>
    [Test]
    public void T060_ChangeNewsletter() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Newsletter1 = QueryHelper.Read<Newsletter>(Newsletter1Key, session);
        Newsletter2 = QueryHelper.Read<Newsletter>(Newsletter2Key, session);
        Event1 = Newsletter1.Events[0];
        Event1.Newsletter = Newsletter2;
        session.Commit();
        Assert.AreSame(Newsletter2, Event1.Newsletter,
          "Event1.Newsletter after Event1 changes Newsletter");
        Assert.AreEqual(0, Newsletter1.Events.Count,
          "Newsletter1.Events.Count after Event1 changes Newsletter");
        Assert.AreEqual(2, Newsletter2.Events.Count,
          "Newsletter2.Events.Count after Event1 changes Newsletter");
        Assert.AreSame(Event1, Newsletter2.Events[0],
          "Newsletter2 1st Event after Event1 changes Newsletter");
      }
    }

    [Test]
    public void T070_SetNewsletterToNull() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Newsletter1 =
          QueryHelper.Read<Newsletter>(Newsletter1.SimpleKey, session);
        Event1 = Newsletter1.Events[0];
        Event1.Newsletter = null;
        session.Commit();
        Assert.IsNull(Event1.Newsletter, "Event1.Newsletter");
        Assert.AreEqual(0, Newsletter1.Events.Count,
          "Newsletter1.Events.Count");
      }
    }

    [Test]
    public void T080_DisallowUnpersistEventWithSets() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Event1 = QueryHelper.Read<Event>(Event1SimpleKey, Location1, session);
        Assert.Throws<ReferentialIntegrityException>(() =>
          Event1.Unpersist(session));
        session.Commit();
      }
    }

    [Test]
    public void T090_DisallowRemoveSet() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Event1 = QueryHelper.Read<Event>(Event1SimpleKey, Location1, session);
        Set1 = QueryHelper.Read<Set>(Set1.SimpleKey, Event1, session);
        Assert.Throws<ConstraintException>(() =>
            Event1.Sets.Remove(Set1),
          "Disallow remove Set from mandatory link to Event.");
        session.Commit();
      }
    }

    [Test]
    public void T100_GetNullLocation() {
      var @event = new Event {
        QueryHelper = QueryHelper,
        Date = Event2Date
      };
      Assert.IsNull(@event.Location);
    }
  }
}