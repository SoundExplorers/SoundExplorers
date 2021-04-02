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
      Data = new TestData(QueryHelper);
      Session = new TestSession(DatabaseFolderPath);
      Session.BeginUpdate();
      Data.AddRootsPersistedIfRequired(Session);
      Session.Commit();
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
      DefaultNewsletter = Newsletter.CreateDefault();
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
      DefaultSeries = Series.CreateDefault();
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
      DefaultAct = Act.CreateDefault();
      Session.BeginUpdate();
      Session.Persist(EventType1);
      Session.Persist(Genre1);
      Session.Persist(Location1);
      Session.Persist(Location2);
      Session.Persist(DefaultNewsletter);
      Session.Persist(Newsletter1);
      Session.Persist(Newsletter2);
      Session.Persist(DefaultSeries);
      Session.Persist(Series1);
      Session.Persist(Series2);
      Session.Persist(DefaultAct);
      Event1.Location = Location1;
      Event1AtLocation2.Location = Location2;
      Event2.Location = Location1;
      Event2.Series = Series1;
      Event1.EventType = EventType1;
      Event1AtLocation2.EventType = EventType1;
      Event2.EventType = EventType1;
      Session.Persist(Event1);
      Session.Persist(Event1AtLocation2);
      Session.Persist(Event2);
      Event1.Newsletter = Newsletter1;
      Event1AtLocation2.Newsletter = Newsletter2;
      Event1AtLocation2.Series = Series2;
      Set1.Event = Event1;
      Set1.Genre = Genre1;
      Set2.Event = Event1;
      Set2.Genre = Genre1;
      Session.Persist(Set1);
      Session.Persist(Set2);
      Session.Commit();
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
    private TestData Data { get; set; } = null!;
    private TestSession Session { get; set; } = null!;
    private Act DefaultAct { get; set; } = null!;
    private Newsletter DefaultNewsletter { get; set; } = null!;
    private Series DefaultSeries { get; set; } = null!;
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
      Session.BeginRead();
      Event1 = QueryHelper.Read<Event>(Event1SimpleKey, Location1, Session);
      Event1AtLocation2 =
        QueryHelper.Read<Event>(Event1SimpleKey, Location2, Session);
      Event2 = QueryHelper.Read<Event>(Event2SimpleKey, Location1, Session);
      EventType1 = QueryHelper.Read<EventType>(EventType1Name, Session);
      Location1 = QueryHelper.Read<Location>(Location1Name, Session);
      Location2 = QueryHelper.Read<Location>(Location2Name, Session);
      Newsletter1 =
        QueryHelper.Read<Newsletter>(Newsletter1SimpleKey, Session);
      DefaultSeries = QueryHelper.Read<Series>(DefaultSeries.Name, Session);
      Series1 = QueryHelper.Read<Series>(Series1Name, Session);
      Set1 = QueryHelper.Read<Set>(Set1.SimpleKey, Event1, Session);
      Set2 = QueryHelper.Read<Set>(Set2.SimpleKey, Event1, Session);
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
      Assert.AreSame(DefaultSeries, Event1.Series, "Event1.Series");
      Assert.AreEqual(1, Series1.Events.Count, "Series1.Events.Count");
      Assert.AreEqual(1, Series1.References.Count, "Series1.References.Count");
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
      Session.Commit();
    }

    /// <summary>
    ///   Adds an Event to a Series that already includes an Event
    ///   on the same Date at a different Location.
    /// </summary>
    [Test]
    public void AddToSeries() {
      Session.BeginUpdate();
      Series2 = QueryHelper.Read<Series>(Series2Name, Session);
      Event1 = QueryHelper.Read<Event>(Event1SimpleKey, Location1, Session);
      Event1.Series = Series2;
      Assert.AreSame(Series2, Event1.Series, "Event1.Series");
      Assert.AreEqual(2, Series2.Events.Count, "Series2.Events.Count");
      Assert.AreSame(Event1, Series2.Events[0], "Series2 1st Event");
      Assert.AreSame(Series2, Event1.Series, "Event1.Series in new Session");
      Assert.AreEqual(2, Series2.Events.Count, "Series2.Events.Count in new Session");
      Assert.AreSame(Event1, Series2.Events[0], "Series2 1st Event in new Session");
      Session.Commit();
    }

    [Test]
    public void ChangeLocation() {
      Session.BeginUpdate();
      Location1 = QueryHelper.Read<Location>(Location1Name, Session);
      Location2 = QueryHelper.Read<Location>(Location2Name, Session);
      Event1 = Location1.Events[0];
      Event1AtLocation2 = Location2.Events[0];
      Event2 = Location1.Events[1];
      Event2.Location = Location2;
      Assert.AreSame(Location2, Event2.Location, "Event2.Location");
      Assert.AreEqual(1, Location1.Events.Count, "Location1.Events.Count");
      Assert.AreEqual(2, Location2.Events.Count, "Location2.Events.Count");
      Assert.AreSame(Event1AtLocation2, Location2.Events[0],
        "Location2 1st Event");
      Assert.AreSame(Event2, Location2.Events[1], "Location2 2nd Event");
      Assert.AreSame(Location2, Event2.Location, "Event2.Location in new Session");
      Assert.AreEqual(1, Location1.Events.Count, "Location1.Events.Count in new Session");
      Assert.AreEqual(2, Location2.Events.Count, "Location2.Events.Count in new Session");
      Assert.AreSame(Event1AtLocation2, Location2.Events[0],
        "Location2 1st Event in new Session");
      Assert.AreSame(Event2, Location2.Events[1], "Location2 2nd Event in new Session");
      Session.Commit();
    }

    [Test]
    public void ChangeLocationToSame() {
      Session.BeginUpdate();
      Location1 = QueryHelper.Read<Location>(Location1Name, Session);
      Event1 = Location1.Events[0];
      Assert.DoesNotThrow(() => Event1.Location = Location1);
      Session.Commit();
    }

    /// <summary>
    ///   Changes an Event's NewsLetter to one that already covers an Event
    ///   on the same Date at a different Location.
    /// </summary>
    [Test]
    public void ChangeNewsletter() {
      const string newNotes = "My new notes";
      Session.BeginUpdate();
      Newsletter1 =
        QueryHelper.Read<Newsletter>(Newsletter1SimpleKey, Session);
      Newsletter2 =
        QueryHelper.Read<Newsletter>(Newsletter2SimpleKey, Session);
      Event1 = Newsletter1.Events[0];
      Event1.Newsletter = Newsletter2;
      Event1.Notes = newNotes;
      Assert.AreSame(Newsletter2, Event1.Newsletter, "Event1.Newsletter");
      Assert.AreEqual(0, Newsletter1.Events.Count,
        "Newsletter1.Events.Count");
      Assert.AreEqual(2, Newsletter2.Events.Count,
        "Newsletter2.Events.Count");
      Assert.AreSame(Event1, Newsletter2.Events[0], "Newsletter2 1st Event");
      Assert.AreEqual(newNotes, Event1.Notes, "Event1.Notes in new Session");
      Assert.AreSame(Newsletter2, Event1.Newsletter, "Event1.Newsletter in new Session");
      Assert.AreEqual(0, Newsletter1.Events.Count,
        "Newsletter1.Events.Count in new Session");
      Assert.AreEqual(2, Newsletter2.Events.Count,
        "Newsletter2.Events.Count in new Session");
      Session.Commit();
    }

    [Test]
    public void ChangeNewsletterToDefault() {
      Assert.AreSame(Newsletter1, Event1.Newsletter, "Event1.Newsletter initially");
      Assert.AreEqual(1, DefaultNewsletter.Events.Count,
        "DefaultNewsletter.Events.Count initially");
      Assert.AreEqual(1, Newsletter1.Events.Count,
        "Newsletter1.Events.Count initially");
      Session.BeginUpdate();
      DefaultNewsletter =
        QueryHelper.Read<Newsletter>(DefaultNewsletter.SimpleKey, Session);
      Newsletter1 =
        QueryHelper.Read<Newsletter>(Newsletter1SimpleKey, Session);
      Event1 = QueryHelper.Read<Event>(Event1SimpleKey, Location1, Session);
      Event1.Newsletter = DefaultNewsletter;
      Session.Commit();
      Assert.AreSame(DefaultNewsletter, Event1.Newsletter, "Event1.Newsletter");
      Assert.AreEqual(2, DefaultNewsletter.Events.Count,
        "DefaultNewsletter.Events.Count after update");
      Assert.AreEqual(0, Newsletter1.Events.Count,
        "Newsletter1.Events.Count after update");
      Session.BeginRead();
      DefaultNewsletter =
        QueryHelper.Read<Newsletter>(DefaultNewsletter.SimpleKey, Session);
      Newsletter1 =
        QueryHelper.Read<Newsletter>(Newsletter1SimpleKey, Session);
      Event1 = QueryHelper.Read<Event>(Event1SimpleKey, Location1, Session);
      Session.Commit();
      Assert.AreSame(DefaultNewsletter, Event1.Newsletter,
        "Event1.Newsletter in new Session");
      Assert.AreEqual(2, DefaultNewsletter.Events.Count,
        "DefaultNewsletter.Events.Count in new Session");
      Assert.AreEqual(0, Newsletter1.Events.Count,
        "Newsletter1.Events.Count in new Session");
    }

    [Test]
    public void ChangeSeriesToDefault() {
      Assert.AreSame(Series1, Event2.Series, "Event2.Series initially");
      Assert.AreEqual(1, DefaultSeries.Events.Count,
        "DefaultSeries.Events.Count initially");
      Assert.AreEqual(1, Series1.Events.Count,
        "Series1.Events.Count initially");
      Session.BeginUpdate();
      DefaultSeries =
        QueryHelper.Read<Series>(DefaultSeries.SimpleKey, Session);
      Series1 =
        QueryHelper.Read<Series>(Series1.SimpleKey, Session);
      Event2 = QueryHelper.Read<Event>(Event2SimpleKey, Location1, Session);
      Event2.Series = DefaultSeries;
      Session.Commit();
      Assert.AreSame(DefaultSeries, Event2.Series, "Event2.Series");
      Assert.AreEqual(2, DefaultSeries.Events.Count,
        "DefaultSeries.Events.Count after update");
      Assert.AreEqual(0, Series1.Events.Count,
        "Series1.Events.Count after update");
      Session.BeginRead();
      DefaultSeries =
        QueryHelper.Read<Series>(DefaultSeries.SimpleKey, Session);
      Series1 =
        QueryHelper.Read<Series>(Series1.SimpleKey, Session);
      Event2 = QueryHelper.Read<Event>(Event2SimpleKey, Location1, Session);
      Session.Commit();
      Assert.AreSame(DefaultSeries, Event2.Series,
        "Event2.Series in new Session");
      Assert.AreEqual(2, DefaultSeries.Events.Count,
        "DefaultSeries.Events.Count in new Session");
      Assert.AreEqual(0, Series1.Events.Count,
        "Series1.Events.Count in new Session");
    }

    [Test]
    public void DisallowChangeDateToDuplicate() {
      Session.BeginUpdate();
      Event2 =
        QueryHelper.Read<Event>(Event2SimpleKey, Location1, Session);
      Event2.Date = Event2Date;
      Assert.Throws<ConstraintException>(() => Event2.Date = Event1Date);
      Session.Commit();
    }

    [Test]
    public void DisallowChangeDateToMinimum() {
      Session.BeginUpdate();
      Event2 = QueryHelper.Read<Event>(Event2SimpleKey, Location1, Session);
      Assert.Throws<PropertyConstraintException>(() =>
        Event2.Date = DateTime.MinValue);
      Session.Commit();
    }

    [Test]
    public void DisallowChangeEventTypeToNull() {
      Session.BeginUpdate();
      EventType1 = QueryHelper.Read<EventType>(EventType1Name, Session);
      Event1 = QueryHelper.Read<Event>(Event1SimpleKey, Location1, Session);
      Assert.Throws<ConstraintException>(() => Event1.EventType = null!);
      Session.Commit();
    }

    [Test]
    public void DisallowChangeLocationToNull() {
      Session.BeginUpdate();
      Location1 = QueryHelper.Read<Location>(Location1Name, Session);
      Event1 = QueryHelper.Read<Event>(Event1SimpleKey, Location1, Session);
      Assert.Throws<PropertyConstraintException>(() => Event1.Location = null!);
      Session.Commit();
    }

    [Test]
    public void DisallowPersistUnspecifiedDate() {
      var noDate = new Event {
        QueryHelper = QueryHelper
      };
      Session.BeginUpdate();
      Location1 = QueryHelper.Read<Location>(Location1.SimpleKey, Session);
      noDate.Location = Location1;
      Assert.Throws<PropertyConstraintException>(() => Session.Persist(noDate));
      Session.Abort();
    }

    [Test]
    public void DisallowSetKeyToDuplicate() {
      var duplicate = new Event {
        QueryHelper = QueryHelper,
        Date = Event1Date
      };
      Session.BeginUpdate();
      Location1 = QueryHelper.Read<Location>(Location1Name, Session);
      Assert.Throws<ConstraintException>(() =>
        duplicate.Location = Location1);
      Session.Commit();
    }

    [Test]
    public void DisallowUnpersistEventWithSets() {
      Session.BeginUpdate();
      Event1 = QueryHelper.Read<Event>(Event1SimpleKey, Location1, Session);
      Assert.Throws<ConstraintException>(() =>
        Event1.Unpersist(Session));
      Session.Commit();
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
    public void OrderOfSets() {
      var set3 = new Set {
        QueryHelper = QueryHelper,
        SetNo = 3
      };
      var set4 = new Set {
        QueryHelper = QueryHelper,
        SetNo = 4
      };
      Session.BeginUpdate();
      Event1 = QueryHelper.Read<Event>(Event1SimpleKey, Location1, Session);
      Genre1 = QueryHelper.Read<Genre>(Genre1Name, Session);
      set4.Event = Event1;
      set4.Genre = Genre1;
      Session.Persist(set4);
      set3.Event = Event1;
      set3.Genre = Genre1;
      Session.Persist(set3);
      Assert.AreEqual(4, Event1.Sets.Count, "Sets.Count");
      Assert.AreEqual(1, Event1.Sets[0].SetNo, "Sets[0].SetNo");
      Assert.AreEqual(2, Event1.Sets[1].SetNo, "Sets[1].SetNo");
      Assert.AreEqual(3, Event1.Sets[2].SetNo, "Sets[2].SetNo");
      Assert.AreEqual(4, Event1.Sets[3].SetNo, "Sets[3].SetNo");
      Session.Commit();
    }

    [Test]
    public void Unpersist() {
      Session.BeginUpdate();
      Event1 = QueryHelper.Read<Event>(Event1SimpleKey, Location1, Session);
      Set1 = QueryHelper.Read<Set>(Set1.SimpleKey, Event1, Session);
      Set2 = QueryHelper.Read<Set>(Set2.SimpleKey, Event1, Session);
      ConstraintException exception =
        Assert.Catch<ConstraintException>(() => Session.Unpersist(Event1),
          "Unpersist Event with Sets");
      Session.Unpersist(Set1);
      Session.Unpersist(Set2);
      Session.Commit();
      Assert.IsTrue(
        exception.Message.Contains("' cannot be deleted because it is referenced by "),
        "ConstraintException message");
      Session.BeginUpdate();
      Event1 = QueryHelper.Read<Event>(Event1SimpleKey, Location1, Session);
      EventType1 = QueryHelper.Read<EventType>(EventType1Name, Session);
      Location1 = QueryHelper.Read<Location>(Location1Name, Session);
      Newsletter1 =
        QueryHelper.Read<Newsletter>(Newsletter1SimpleKey, Session);
      Session.Unpersist(Event1);
      Session.Commit();
      Assert.AreEqual(2, EventType1.Events.Count,
        "EventType1.Events.Count after deleting Event1");
      Assert.AreEqual(1, Location1.Events.Count,
        "Location1.Events.Count after deleting Event1");
      Assert.AreEqual(0, Newsletter1.Events.Count,
        "Newsletter1.Events.Count after deleting Event1");
      Session.BeginUpdate();
      Event1 = QueryHelper.Find<Event>(Event1SimpleKey, Location1, Session)!;
      EventType1 = QueryHelper.Read<EventType>(EventType1Name, Session);
      Location1 = QueryHelper.Read<Location>(Location1Name, Session);
      Newsletter1 =
        QueryHelper.Read<Newsletter>(Newsletter1SimpleKey, Session);
      Session.Unpersist(Event1);
      Session.Commit();
      Assert.IsNull(Event1, "Event1 in new Session");
      Assert.AreEqual(2, EventType1.Events.Count,
        "EventType1.Events.Count in new Session");
      Assert.AreEqual(1, Location1.Events.Count,
        "Location1.Events.Count in new Session");
      Assert.AreEqual(0, Newsletter1.Events.Count,
        "Newsletter1.Events.Count in new Session");
    }
  }
}