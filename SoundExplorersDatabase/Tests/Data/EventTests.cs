using System;
using System.Linq;
using NUnit.Framework;
using SoundExplorersDatabase.Data;
using VelocityDb.Exceptions;

namespace SoundExplorersDatabase.Tests.Data {
  [TestFixture]
  public class EventTests {
    private const string Event1Notes = "My notes.";
    private const string Location1Name = "Fred's";
    private const string Location2Name = "Pyramid Club";

    private string DatabaseFolderPath { get; set; }
    private DateTime Date1 { get; set; }
    private DateTime Date2 { get; set; }
    private Event Event1 { get; set; }
    private Event Event2 { get; set; }
    private Location Location1 { get; set; }
    private Location Location2 { get; set; }

    private void AddUnpersistedEvent1ToUnpersistedLocation1() {
      Event location1Child1;
      Location1 = new Location(Location1Name);
      Event1 = new Event(Date1, Location1) {Notes = Event1Notes};
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        session.Persist(Location1);
        session.Persist(Event1);
        session.Commit();
        session.BeginRead();
        Event1 = Event.Read(Date1, Location1, session);
        Assert.AreSame(Location1, Event1.Location, "Location1 same as Event1.Location");
        Assert.IsNotNull(Event1.Location.Events, "Event1.Location.Events exist");
        Assert.AreEqual(1, Event1.Location.Events.Count, "Event1.Location.Events.Count");
        location1Child1 = Event1.Location.Events.First();
        session.Commit();
      }
      Assert.AreEqual(Date1, Event1.Date, "Event1.Date");
      Assert.AreEqual(Event1Notes, Event1.Notes, "Event1.Notes");
      Assert.AreEqual(Location1Name, Event1.Location.Name, "Event1.Location.Name");
      Assert.AreSame(Event1, location1Child1, "Event1 same as location1Child1");
    }

    private void DisallowUnpersistLocationWithEvent() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Location1 = Location.Read(Location1Name, session);
        Assert.AreEqual(1, Location1.Events.Count, "Location1.Events.Count");
        Assert.Throws<ReferentialIntegrityException>(() => Location1.Unpersist(session));
        session.Commit();
      }
    }

    private void DisallowDuplicate() {
      var duplicate = new Event(Date1, Location1);
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Assert.Throws<UniqueConstraintException>(
          () => session.Persist(duplicate), "Duplicate not allowed");
        session.Commit();
      }
    }

    private void AddUnpersistedEvent2ToPersistedLocation1() {
      Event location1Child2;
      Event2 = new Event(Date2, Location1);
      using (var session = new TestSession()) {
        session.BeginUpdate();
        session.Persist(Event2);
        Assert.AreSame(Location1, Event2.Location, "Location1 same as Event2.Location");
        Assert.IsNotNull(Event2.Location.Events, "Event2.Location.Events exist");
        Assert.AreEqual(2, Event2.Location.Events.Count, "Event2.Location.Events.Count");
        location1Child2 = Event2.Location.Events.ToArray()[1];
        session.Commit();
      }
      Assert.AreEqual(Date2, Event2.Date, "Event2.Date");
      Assert.AreSame(Event2, location1Child2, "Event2 same as location1Child2");
    }

    private void DisallowMovePersistedEvent2ToUnpersistedLocation2() {
      Location2 = new Location(Location2Name);
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Assert.Throws<UnexpectedException>(() => Location2.Events.Add(Event2));
        session.Commit();
      }
    }

    private void MovePersistedEvent2ToPersistedLocation2() {
      Event location2Child1;
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        session.Persist(Location2);
        Location2.Events.Add(Event2);
        Assert.AreSame(Location2, Event2.Location, "Location2 same as Event2.Location");
        Assert.IsNotNull(Location2.Events, "Location2.Events exist");
        Assert.AreEqual(1, Location2.Events.Count, "Location2.Events.Count");
        Assert.AreEqual(1, Location1.Events.Count, "Location1.Events.Count");
        location2Child1 = Location2.Events.First();
        session.Commit();
      }
      Assert.AreSame(Event2, location2Child1, "Event1 same as location2Child1");
    }

    [Test]
    public void Multiple() {
      Date1 = DateTime.Today.AddDays(-1);
      Date2 = DateTime.Today;
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
      try {
        AddUnpersistedEvent1ToUnpersistedLocation1();
        DisallowUnpersistLocationWithEvent();
        DisallowDuplicate();
        AddUnpersistedEvent2ToPersistedLocation1();
        DisallowMovePersistedEvent2ToUnpersistedLocation2();
        MovePersistedEvent2ToPersistedLocation2();
      }
      finally {
        TestSession.DeleteFolderIfExists(DatabaseFolderPath);
      }
    }
  }
}