using NUnit.Framework;
using SoundExplorersDatabase.Data;
using VelocityDb.Exceptions;

namespace SoundExplorersDatabase.Tests.Data {
  [TestFixture]
  public class LocationTests {
    [Test]
    public void Persist() {
      const string name = "Pyramid Club";
      const string notes = "My notes.";
      var location = new Location(name) {Notes = notes};
      using (var session = TestSession.Create()) {
        session.BeginUpdate();
        session.Persist(location);
        session.Commit();
        session.BeginRead();
        Assert.DoesNotThrow(
          () => location =
            (Location)session.ReadUsingIndex(() =>
              Location.Read(name, session)),
          "Index used");
        session.Commit();
        Assert.AreEqual(name, location.Name, "Name");
        Assert.AreEqual(notes, location.Notes, "Notes");
        var duplicate = new Location(name);
        session.BeginUpdate();
        Assert.Throws<UniqueConstraintException>(
          () => session.Persist(duplicate), "Duplicate not allowed");
        session.Commit();
      }
    }
  }
}