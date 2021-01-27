using System;
using System.Data;
using NUnit.Framework;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data {
  [TestFixture]
  public class EventTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
      EventType1 = new EventType {
        QueryHelper = QueryHelper,
        Name = EventType1Name
      };
      Genre1 = new Genre {
        QueryHelper = QueryHelper,
        Name = Genre1Name
      };
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
        Url = Newsletter1Url
      };
      Newsletter2 = new Newsletter {
        QueryHelper = QueryHelper,
        Date = Newsletter2Date,
        Url = Newsletter2Url
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
      using var session = new TestSession(DatabaseFolderPath);
      session.BeginUpdate();
      session.Persist(EventType1);
      session.Persist(Genre1);
      session.Persist(Location1);
      session.Persist(Location2);
      session.Persist(Newsletter1);
      session.Persist(Newsletter2);
      session.Persist(Series1);
      session.Persist(Series2);
      Event1.Location = Location1;
      Event1AtLocation2.Location = Location2;
      Event2.Location = Location1;
      Event1.EventType = EventType1;
      Event1AtLocation2.EventType = EventType1;
      Event2.EventType = EventType1;
      session.Persist(Event1);
      session.Persist(Event1AtLocation2);
      session.Persist(Event2);
      Event1.Newsletter = Newsletter1;
      Event1AtLocation2.Newsletter = Newsletter2;
      Event1AtLocation2.Series = Series2;
      Set1.Event = Event1;
      Set1.Genre = Genre1;
      Set2.Event = Event1;
      Set2.Genre = Genre1;
      session.Persist(Set1);
      session.Persist(Set2);
      session.Commit();
    }

    [TearDown]
    public void Teardown() {
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }

    private const string Event1SimpleKey = "2013/05/01";
    private const string Event1Notes = "My notes.";
    private const string Event2SimpleKey = "2013/05/02";
    private const string EventType1Name = "Performance";
    private const string Genre1Name = "Jazz";
    private const string Location1Name = "Fred's";
    private const string Location2Name = "Pyramid Club";
    private const string Newsletter1SimpleKey = "2013/04/11";
    private const string Newsletter2SimpleKey = "2013/04/22";
    private const string Series1Name = "Jazz Festival 2013";
    private const string Series2Name = "Field Recordings";
    private const int Set1SetNo = 1;
    private const int Set2SetNo = 2;
    private string DatabaseFolderPath { get; set; } = null!;
    private QueryHelper QueryHelper { get; set; } = null!;
    private Event Event1 { get; set; } = null!;
    private static DateTime Event1Date => DateTime.Parse(Event1SimpleKey);
    private Event Event1AtLocation2 { get; set; } = null!;
    private Event Event2 { get; set; } = null!;
    private static DateTime Event2Date => DateTime.Parse(Event2SimpleKey);
    private EventType EventType1 { get; set; } = null!;
    private Genre Genre1 { get; set; } = null!;
    private Location Location1 { get; set; } = null!;
    private Location Location2 { get; set; } = null!;
    private Newsletter Newsletter1 { get; set; } = null!;

    private static DateTime Newsletter1Date =>
      DateTime.Parse(Newsletter1SimpleKey);

    private static string Newsletter1Url =>
      "https://archive.org/details/simpsons-lat.375923";

    private Newsletter Newsletter2 { get; set; } = null!;

    private static DateTime Newsletter2Date =>
      DateTime.Parse(Newsletter2SimpleKey);

    private static string Newsletter2Url =>
      "https://archive.org/details/BDChaurasiasHumanAnatomyVolume1MedicosTimes";

    private Series Series1 { get; set; } = null!;
    private Series Series2 { get; set; } = null!;
    private Set Set1 { get; set; } = null!;
    private Set Set2 { get; set; } = null!;

    [Test]
    public void A010_Initial() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        Event1 = QueryHelper.Read<Event>(Event1SimpleKey, Location1, session);
        Event1AtLocation2 =
          QueryHelper.Read<Event>(Event1SimpleKey, Location2, session);
        Event2 = QueryHelper.Read<Event>(Event2SimpleKey, Location1, session);
        EventType1 = QueryHelper.Read<EventType>(EventType1Name, session);
        Location1 = QueryHelper.Read<Location>(Location1Name, session);
        Location2 = QueryHelper.Read<Location>(Location2Name, session);
        Newsletter1 =
          QueryHelper.Read<Newsletter>(Newsletter1SimpleKey, session);
        Series1 = QueryHelper.Read<Series>(Series1Name, session);
        Set1 = QueryHelper.Read<Set>(Set1.SimpleKey, Event1, session);
        Set2 = QueryHelper.Read<Set>(Set2.SimpleKey, Event1, session);
        session.Commit();
      }
      Assert.AreEqual(3, EventType1.Events.Count, "EventType1.Events.Count");
      Assert.AreEqual(Event1Date, Event1.Date, "Event1.Date");
      Assert.AreEqual(Event1Notes, Event1.Notes, "Event1.Notes");
      Assert.AreEqual(EventType1Name, Event1.EventType.Name,
        "Event1.EventType.Name");
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
      Assert.AreEqual(Event2Date, Event2.Date, "Event2.Date");
      Assert.AreSame(Location1, Event2.Location, "Event2.Location");
      Assert.AreEqual(1, Location2.Events.Count, "Location2.Events.Count");
      Assert.AreEqual(1, Location2.References.Count,
        "Location2.References.Count");
      Assert.AreSame(Event1AtLocation2, Location2.Events[0],
        "Location2.Events[1]");
      Assert.AreEqual(2, Event1.Sets.Count, "Event1.Sets.Count");
      Assert.AreEqual(2, Event1.References.Count, "Event1.References.Count");
      Assert.AreSame(Event1, Set1.Event, "Set1.Event");
      Assert.AreEqual(Event1.Date, Set1.Event.Date, "Set1.Event.Date");
      Assert.AreSame(Event1, Set2.Event, "Set2.Event");
      Assert.AreEqual(Event1.Date, Set2.Event.Date, "Set2.Event.Date");
    }

    /// <summary>
    ///   Adds an Event to a Series that already includes an Event
    ///   on the same Date at a different Location.
    /// </summary>
    [Test]
    public void AddToSeries() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Series2 = QueryHelper.Read<Series>(Series2Name, session);
        Event1 = QueryHelper.Read<Event>(Event1SimpleKey, Location1, session);
        Event1.Series = Series2;
        session.Commit();
      }
      Assert.AreSame(Series2, Event1.Series, "Event1.Series");
      Assert.AreEqual(2, Series2.Events.Count, "Series2.Events.Count");
      Assert.AreSame(Event1, Series2.Events[0], "Series2 1st Event");
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        Series2 = QueryHelper.Read<Series>(Series2Name, session);
        Event1 = QueryHelper.Read<Event>(Event1SimpleKey, Location1, session);
        session.Commit();
      }
      Assert.AreSame(Series2, Event1.Series, "Event1.Series in new session");
      Assert.AreEqual(2, Series2.Events.Count, "Series2.Events.Count in new session");
      Assert.AreSame(Event1, Series2.Events[0], "Series2 1st Event in new session");
    }

    [Test]
    public void ChangeLocation() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Location1 = QueryHelper.Read<Location>(Location1Name, session);
        Location2 = QueryHelper.Read<Location>(Location2Name, session);
        Event1 = Location1.Events[0];
        Event1AtLocation2 = Location2.Events[0];
        Event2 = Location1.Events[1];
        Event2.Location = Location2;
        session.Commit();
      }
      Assert.AreSame(Location2, Event2.Location, "Event2.Location");
      Assert.AreEqual(1, Location1.Events.Count, "Location1.Events.Count");
      Assert.AreEqual(2, Location2.Events.Count, "Location2.Events.Count");
      Assert.AreSame(Event1AtLocation2, Location2.Events[0],
        "Location2 1st Event");
      Assert.AreSame(Event2, Location2.Events[1], "Location2 2nd Event");
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        Location1 = QueryHelper.Read<Location>(Location1Name, session);
        Location2 = QueryHelper.Read<Location>(Location2Name, session);
        Event1AtLocation2 =
          QueryHelper.Read<Event>(Event1SimpleKey, Location2, session);
        Event2 = QueryHelper.Read<Event>(Event2SimpleKey, Location2, session);
        session.Commit();
      }
      Assert.AreSame(Location2, Event2.Location, "Event2.Location in new session");
      Assert.AreEqual(1, Location1.Events.Count, "Location1.Events.Count in new session");
      Assert.AreEqual(2, Location2.Events.Count, "Location2.Events.Count in new session");
      Assert.AreSame(Event1AtLocation2, Location2.Events[0],
        "Location2 1st Event in new session");
      Assert.AreSame(Event2, Location2.Events[1], "Location2 2nd Event in new session");
    }

    [Test]
    public void ChangeLocationToSame() {
      using var session = new TestSession(DatabaseFolderPath);
      session.BeginUpdate();
      Location1 = QueryHelper.Read<Location>(Location1Name, session);
      Event1 = Location1.Events[0];
      Assert.DoesNotThrow(() => Event1.Location = Location1);
      session.Commit();
    }

    /// <summary>
    ///   Changes an Event's NewsLetter to one that already covers an Event
    ///   on the same Date at a different Location.
    /// </summary>
    [Test]
    public void ChangeNewsletter() {
      const string newNotes = "My new notes";
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Newsletter1 =
          QueryHelper.Read<Newsletter>(Newsletter1SimpleKey, session);
        Newsletter2 =
          QueryHelper.Read<Newsletter>(Newsletter2SimpleKey, session);
        Event1 = Newsletter1.Events[0];
        Event1.Newsletter = Newsletter2;
        Event1.Notes = newNotes;
        session.Commit();
      }
      Assert.AreSame(Newsletter2, Event1.Newsletter, "Event1.Newsletter");
      Assert.AreEqual(0, Newsletter1.Events.Count,
        "Newsletter1.Events.Count");
      Assert.AreEqual(2, Newsletter2.Events.Count,
        "Newsletter2.Events.Count");
      Assert.AreSame(Event1, Newsletter2.Events[0], "Newsletter2 1st Event");
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        Location1 = QueryHelper.Read<Location>(Location1Name, session);
        Location2 = QueryHelper.Read<Location>(Location2Name, session);
        Newsletter1 =
          QueryHelper.Read<Newsletter>(Newsletter1SimpleKey, session);
        Newsletter2 =
          QueryHelper.Read<Newsletter>(Newsletter2SimpleKey, session);
        Event1 = QueryHelper.Read<Event>(Event1SimpleKey, Location1, session);
        session.Commit();
      }
      Assert.AreEqual(newNotes, Event1.Notes, "Event1.Notes in new session");
      Assert.AreSame(Newsletter2, Event1.Newsletter, "Event1.Newsletter in new session");
      Assert.AreEqual(0, Newsletter1.Events.Count,
        "Newsletter1.Events.Count in new session");
      Assert.AreEqual(2, Newsletter2.Events.Count,
        "Newsletter2.Events.Count in new session");
    }

    [Test]
    public void DisallowChangeDateToDuplicate() {
      using var session = new TestSession(DatabaseFolderPath);
      session.BeginUpdate();
      Event2 =
        QueryHelper.Read<Event>(Event2SimpleKey, Location1, session);
      Event2.Date = Event2Date;
      Assert.Throws<ConstraintException>(() => Event2.Date = Event1Date);
      session.Commit();
    }

    [Test]
    public void DisallowChangeDateToMinimum() {
      using var session = new TestSession(DatabaseFolderPath);
      session.BeginUpdate();
      Event2 = QueryHelper.Read<Event>(Event2SimpleKey, Location1, session);
      Assert.Throws<PropertyConstraintException>(() =>
        Event2.Date = DateTime.MinValue);
      session.Commit();
    }

    [Test]
    public void DisallowChangeEventTypeToNull() {
      using var session = new TestSession(DatabaseFolderPath);
      session.BeginUpdate();
      EventType1 = QueryHelper.Read<EventType>(EventType1Name, session);
      Event1 = QueryHelper.Read<Event>(Event1SimpleKey, Location1, session);
      Assert.Throws<ConstraintException>(() => Event1.EventType = null!);
      session.Commit();
    }

    [Test]
    public void DisallowChangeLocationToNull() {
      using var session = new TestSession(DatabaseFolderPath);
      session.BeginUpdate();
      Location1 = QueryHelper.Read<Location>(Location1Name, session);
      Event1 = QueryHelper.Read<Event>(Event1SimpleKey, Location1, session);
      Assert.Throws<PropertyConstraintException>(() => Event1.Location = null!);
      session.Commit();
    }

    [Test]
    public void DisallowPersistUnspecifiedDate() {
      var noDate = new Event {
        QueryHelper = QueryHelper
      };
      using var session = new TestSession(DatabaseFolderPath);
      session.BeginUpdate();
      Location1 = QueryHelper.Read<Location>(Location1.SimpleKey, session);
      noDate.Location = Location1;
      Assert.Throws<PropertyConstraintException>(() => session.Persist(noDate));
      session.Abort();
    }

    [Test]
    public void DisallowSetKeyToDuplicate() {
      var duplicate = new Event {
        QueryHelper = QueryHelper,
        Date = Event1Date
      };
      using var session = new TestSession(DatabaseFolderPath);
      session.BeginUpdate();
      Location1 = QueryHelper.Read<Location>(Location1Name, session);
      Assert.Throws<ConstraintException>(() =>
        duplicate.Location = Location1);
      session.Commit();
    }

    [Test]
    public void DisallowUnpersistEventWithSets() {
      using var session = new TestSession(DatabaseFolderPath);
      session.BeginUpdate();
      Event1 = QueryHelper.Read<Event>(Event1SimpleKey, Location1, session);
      Assert.Throws<ConstraintException>(() =>
        Event1.Unpersist(session));
      session.Commit();
    }

    [Test]
    public void GetNullLocation() {
      var @event = new Event {
        QueryHelper = QueryHelper,
        Date = Event2Date
      };
      Assert.IsNull(@event.Location);
    }

    [Test]
    public void SetNewsletterToNull() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Newsletter1 =
          QueryHelper.Read<Newsletter>(Newsletter1.SimpleKey, session);
        Event1 = Newsletter1.Events[0];
        Event1.Newsletter = null;
        session.Commit();
      }
      Assert.IsNull(Event1.Newsletter, "Event1.Newsletter");
      Assert.AreEqual(0, Newsletter1.Events.Count,
        "Newsletter1.Events.Count");
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        Newsletter1 =
          QueryHelper.Read<Newsletter>(Newsletter1.SimpleKey, session);
        Event1 = QueryHelper.Read<Event>(Event1SimpleKey, Location1, session);
        session.Commit();
      }
      Assert.IsNull(Event1.Newsletter, "Event1.Newsletter in new session");
      Assert.AreEqual(0, Newsletter1.Events.Count,
        "Newsletter1.Events.Count in new session");
    }

    [Test]
    public void Unpersist() {
      ConstraintException exception;
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Event1 = QueryHelper.Read<Event>(Event1SimpleKey, Location1, session);
        Set1 = QueryHelper.Read<Set>(Set1.SimpleKey, Event1, session);
        Set2 = QueryHelper.Read<Set>(Set2.SimpleKey, Event1, session);
        exception = Assert.Catch<ConstraintException>(() => session.Unpersist(Event1),
          "Unpersist Event with Sets");
        session.Unpersist(Set1);
        session.Unpersist(Set2);
        session.Commit();
      }
      Assert.IsTrue(
        exception.Message.Contains("' cannot be deleted because it is referenced by "),
        "ConstraintException message");
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Event1 = QueryHelper.Read<Event>(Event1SimpleKey, Location1, session);
        EventType1 = QueryHelper.Read<EventType>(EventType1Name, session);
        Location1 = QueryHelper.Read<Location>(Location1Name, session);
        Newsletter1 =
          QueryHelper.Read<Newsletter>(Newsletter1SimpleKey, session);
        session.Unpersist(Event1);
        session.Commit();
      }
      Assert.AreEqual(2, EventType1.Events.Count,
        "EventType1.Events.Count after deleting Event1");
      Assert.AreEqual(1, Location1.Events.Count,
        "Location1.Events.Count after deleting Event1");
      Assert.AreEqual(0, Newsletter1.Events.Count,
        "Newsletter1.Events.Count after deleting Event1");
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Event1 = QueryHelper.Find<Event>(Event1SimpleKey, Location1, session)!;
        EventType1 = QueryHelper.Read<EventType>(EventType1Name, session);
        Location1 = QueryHelper.Read<Location>(Location1Name, session);
        Newsletter1 =
          QueryHelper.Read<Newsletter>(Newsletter1SimpleKey, session);
        session.Unpersist(Event1);
        session.Commit();
      }
      Assert.IsNull(Event1, "Event1 in new session");
      Assert.AreEqual(2, EventType1.Events.Count,
        "EventType1.Events.Count in new session");
      Assert.AreEqual(1, Location1.Events.Count,
        "Location1.Events.Count in new session");
      Assert.AreEqual(0, Newsletter1.Events.Count,
        "Newsletter1.Events.Count in new session");
    }
  }
}