using System;
using System.Data;
using NUnit.Framework;
using SoundExplorers.Data;
using VelocityDb;

namespace SoundExplorers.Tests.Data; 

[TestFixture]
public class QueryHelperTests : TestFixtureBase {
  [SetUp]
  public override void Setup() {
    base.Setup();
    DefaultNewsletter = Newsletter.CreateDefault();
    DefaultSeries = Series.CreateDefault();
    Event1 = new Event {
      QueryHelper = QueryHelper,
      Date = Event1Date,
      Notes = Event1Notes
    };
    Session.BeginUpdate();
    Session.Persist(DefaultNewsletter);
    Session.Persist(DefaultSeries);
    Data.AddEventTypesPersisted(1, Session);
    Data.AddLocationsPersisted(2, Session);
    Event1.EventType = Data.EventTypes[0];
    Event1.Location = Data.Locations[0];
    Session.Persist(Event1);
    Session.Commit();
  }

  private const string Event1Notes = "My event notes.";
  private const string Event1SimpleKey = "2013/04/11";
  private Newsletter DefaultNewsletter { get; set; } = null!;
  private Series DefaultSeries { get; set; } = null!;
  private Event Event1 { get; set; } = null!;
  private static DateTime Event1Date => DateTime.Parse(Event1SimpleKey);

  [Test]
  public void Find() {
    Session.BeginRead();
    var location1A =
      QueryHelper.Find<Location>(
        location => location.Name == Data.Locations[0].Name,
        Session);
    var location1B = QueryHelper.Find<Location>(Data.Locations[0].Name, Session);
    var event1A =
      QueryHelper.Find<Event>(
        @event => @event.Date == Event1Date &&
                  @event.Location.Name == Data.Locations[0].Name, Session);
    var event1B =
      QueryHelper.Find<Event>(Event1SimpleKey, Data.Locations[0], Session);
    Session.Commit();
    Assert.IsNotNull(location1A, "location1A after Find by Name");
    Assert.IsNotNull(event1A,
      "event1A after Find by Date and Location.Name.");
    Assert.IsNotNull(location1B, "location1B after Find by SimpleKey");
    Assert.IsNotNull(event1B,
      "event1B after Find by SimpleKey and IdentifyingParent");
  }

  [Test]
  public void FindDuplicateSimpleKey() {
    Session.BeginRead();
    Location found = (QueryHelper.FindDuplicateSimpleKey(typeof(Location), new Oid(),
        Data.Locations[1].Name, Session) as
      Location)!;
    Session.Commit();
    Assert.IsNotNull(found);
    Assert.AreEqual(Data.Locations[1].Name, found.Name);
  }

  [Test]
  public void Read() {
    Session.BeginRead();
    var location1A =
      QueryHelper.Read<Location>(
        location => location.Name == Data.Locations[0].Name,
        Session);
    var location1B = QueryHelper.Read<Location>(Data.Locations[0].Name, Session);
    var event1A =
      QueryHelper.Read<Event>(
        @event => @event.Date == Event1Date &&
                  @event.Location.Name == Data.Locations[0].Name, Session);
    var event1B =
      QueryHelper.Read<Event>(Event1SimpleKey, Data.Locations[0], Session);
    Session.Commit();
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
    Session.BeginRead();
    Data.EventTypes[0] =
      QueryHelper.Read<EventType>(Data.EventTypes[0].Name, Session);
    Assert.Throws<ConstraintException>(() =>
      QueryHelper.Read<Event>(Event1SimpleKey, Data.EventTypes[0], Session));
    Session.Commit();
  }
}