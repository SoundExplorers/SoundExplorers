using System;
using System.Linq;
using NUnit.Framework;
using SoundExplorersDatabase.Data;
using VelocityDb.Exceptions;
using VelocityDb.TypeInfo;

namespace SoundExplorersDatabase.Tests.Data {
  [TestFixture]
  public class EventTests {
    [SetUp]
    public void Setup() {
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
      Date1 = DateTime.Today.AddDays(-1);
      Date2 = DateTime.Today;
      Location1 = new Location(Location1Name);
      Location2 = new Location(Location2Name);
      Location3 = new Location(Location3Name);
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        session.Persist(Location1);
        session.Persist(Location2);
        session.Persist(Location3);
        Event1 = new Event(Date1, Location1) {Notes = Event1Notes};
        session.Persist(Event1);
        session.Commit();
      }
    }

    [TearDown]
    public void Teardown() {
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }

    private const string Event1Notes = "My notes.";
    private const string Location1Name = "Fred's";
    private const string Location2Name = "Pyramid Club";
    private const string Location3Name = "Michael Fowler Centre";

    private string DatabaseFolderPath { get; set; }
    private DateTime Date1 { get; set; }
    private DateTime Date2 { get; set; }
    private Event Event1 { get; set; }
    private Event Event2 { get; set; }
    private Location Location1 { get; set; }
    private Location Location2 { get; set; }
    private Location Location3 { get; set; }

    private void MovePersistedEvent2ToUnpersistedLocation3() {
      Location3 = new Location(Location3Name);
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        Event2 = Event.Read(Date2, Location2, session);
        session.Commit();
        session.BeginUpdate();
        Location3.Events.Add(Event2);
        Assert.IsFalse(Location3.IsPersistent,
          "Location3.IsPersistent after adding Event2 to its events");
        session.Persist(Event2);
        // I don't know why this happens but I hope it's OK.
        // Anyway, it's unlikely to happen in the application.
        Assert.IsTrue(Location3.IsPersistent,
          "Location3.IsPersistent after persisting Event2");
        session.Commit();
        Assert.AreSame(Location3, Event2.Location, "Event2.Location is Location3");
      }
    }

    private void AddEvent2ToLocation1() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Location1 = Location.Read(Location1Name, session);
        Event2 = new Event(Date2, Location1);
        session.Persist(Event2);
        session.Commit();
      }
    }

    [Test]
    public void DeleteLocation1Event2() {
      Reference reference;
      Event referencedEvent2;
      AddEvent2ToLocation1();
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Location1 = Location.Read(Location1Name, session);
        Event2 = Location1.Events.ToArray()[1];
        Assert.AreEqual(2, Location1.References.Count, "Location1.References.Count #1");
        reference = Location1.References.First(r => r.To.Equals(Event2));
        referencedEvent2 = (Event)reference.To;
        session.Unpersist(Event2);
        Location1.References.Remove(reference);
        Location1.Events.Remove(Event2);
        session.Commit();
      }
      Assert.AreSame(Event2, referencedEvent2, "Event2 and referencedEvent2 are same");
      Assert.AreEqual(1, Location1.References.Count, "Location1.References.Count");
      Assert.AreEqual(1, Location1.Events.Count, "Location1.Events.Count");
      Assert.IsFalse(Event2.IsPersistent, "Event2.IsPersistent");
    }

    [Test]
    public void DisallowDuplicate() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        var duplicate = new Event(Date1, Location1);
        session.BeginUpdate();
        Assert.Throws<UniqueConstraintException>(
          () => session.Persist(duplicate), "Duplicate not allowed");
        session.Commit();
      }
    }

    [Test]
    public void DisallowUnpersistLocationWithEvent() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Assert.Throws<ReferentialIntegrityException>(() => Location1.Unpersist(session));
        session.Commit();
      }
    }

    [Test]
    public void Location1Event1() {
      Event location1Child1;
      Assert.AreSame(Location1, Event1.Location, "Location1 same as Event1.Location");
      Assert.IsNotNull(Event1.Location.Events, "Event1.Location.Events exist");
      Assert.AreEqual(1, Event1.Location.Events.Count, "Event1.Location.Events.Count");
      Assert.AreEqual(Date1, Event1.Date, "Event1.Date");
      Assert.AreEqual(Event1Notes, Event1.Notes, "Event1.Notes");
      Assert.AreEqual(Location1Name, Event1.Location.Name, "Event1.Location.Name");
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        Event1 = Event.Read(Date1, Location1, session);
        location1Child1 = Event1.Location.Events.First();
        session.Commit();
      }
      Assert.AreSame(Event1, location1Child1, "Event1 same as location1Child1");
    }

    [Test]
    public void Location1Event2() {
      Event location1Child2;
      AddEvent2ToLocation1();
      Assert.AreSame(Location1, Event2.Location, "Location1 same as Event2.Location");
      Assert.IsNotNull(Event2.Location.Events, "Event2.Location.Events exist");
      Assert.AreEqual(2, Event2.Location.Events.Count, "Event2.Location.Events.Count");
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        Event2 = Event.Read(Date2, Location1, session);
        location1Child2 = Event2.Location.Events.ToArray()[1];
        session.Commit();
      }
      Assert.AreSame(Event2, location1Child2, "Event2 same as location1Child2");
    }

    [Test]
    public void MoveEvent2ToLocation2() {
      Event location2Child1;
      AddEvent2ToLocation1();
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Location1 = Location.Read(Location1Name, session);
        Location2 = Location.Read(Location2Name, session);
        Event2 = Event.Read(Date2, Location1, session);
        Location2.Events.Add(Event2);
        session.Commit();
      }
      Assert.AreSame(Location2, Event2.Location, "Location2 same as Event2.Location");
      Assert.IsNotNull(Location2.Events, "Location2.Events exist");
      Assert.AreEqual(1, Location2.Events.Count, "Location2.Events.Count");
      Assert.AreEqual(1, Location1.Events.Count, "Location1.Events.Count");
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        location2Child1 = Location2.Events.First();
        session.Commit();
      }
      Assert.AreSame(Event2, location2Child1, "Event2 same as location2Child1");
    }
  }
}