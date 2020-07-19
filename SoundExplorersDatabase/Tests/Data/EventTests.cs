using System;
using System.Linq;
using NUnit.Framework;
using SoundExplorersDatabase.Data;
using VelocityDb.Exceptions;

namespace SoundExplorersDatabase.Tests.Data {
  [TestFixture]
  public class EventTests {
    [Test]
    public void Persist() {
      var date1 = DateTime.Today.AddDays(-1);
      var date2 = DateTime.Today;
      const string location1Name = "Fred's";
      const string location2Name = "Pyramid Club";
      const string event1Notes = "My notes.";
      using (var session = TestSession.Create()) {
        var location1 = new Location(location1Name);
        var event1 = new Event(date1, location1) {Notes = event1Notes};
        session.BeginUpdate();
        session.Persist(location1);
        session.Persist(event1);
        session.Commit();
        session.BeginRead();
        event1 = Event.Read(date1, location1, session);
        Assert.AreSame(location1, event1.Location, "location1 same as event1.Location");
        Assert.IsNotNull(event1.Location.Events, "event1.Location.Events exist");
        Assert.AreEqual(1, event1.Location.Events.Count, "event1.Location.Events.Count");
        var location1Child1 = event1.Location.Events.First();
        session.Commit();
        Assert.AreEqual(date1, event1.Date, "event1.Date");
        Assert.AreEqual(event1Notes, event1.Notes, "event1.Notes");
        Assert.AreEqual(location1Name, event1.Location.Name, "event1.Location.Name");
        Assert.AreSame(event1, location1Child1, "event1 same as location1Child1");
        var event2 = new Event(date2, location1);
        session.BeginUpdate();
        session.Persist(event2);
        Assert.AreSame(location1, event2.Location, "location1 same as event2.Location");
        Assert.IsNotNull(event2.Location.Events, "event2.Location.Events exist");
        Assert.AreEqual(2, event2.Location.Events.Count, "event2.Location.Events.Count");
        var location1Child2 = event2.Location.Events.ToArray()[1];
        session.Commit();
        Assert.AreEqual(date2, event2.Date, "event2.Date");
        Assert.AreSame(event2, location1Child2, "event2 same as location1Child2");
        var duplicate = new Event(date1, location1);
        session.BeginUpdate();
        Assert.Throws<UniqueConstraintException>(
          () => session.Persist(duplicate), "Duplicate not allowed");
        session.Commit();
        var location2 = new Location(location2Name);
        session.BeginUpdate();
        location2.Events.Add(event2);
        session.Persist(location2);
        Assert.AreSame(location2, event2.Location, "location2 same as event2.Location");
        Assert.IsNotNull(location2.Events, "location2.Events exist");
        Assert.AreEqual(1, location2.Events.Count, "location2.Events.Count");
        Assert.AreEqual(1, location1.Events.Count, "location1.Events.Count");
        var location2Child1 = location2.Events.First();
        session.Commit();
        Assert.AreSame(event2, location2Child1, "event1 same as location2Child1");
      }
    }
  }
}