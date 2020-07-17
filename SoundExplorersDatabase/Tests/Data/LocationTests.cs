﻿using NUnit.Framework;
using SoundExplorersDatabase.Data;
using VelocityDb.Exceptions;

namespace SoundExplorersDatabase.Tests.Data {
  [TestFixture]
  public class LocationTests {
    [Test]
    public void Persist() {
      const string name = "Pyramid Club";
      const string notes = "My notes.";
      using (var session = TestSession.Create()) {
        try {
          var location = new Location { Name = name, Notes = notes };
          session.BeginUpdate();
          session.Persist(location);
          session.Commit();
          session.BeginRead();
          location = Location.Read(name, session);
          session.Commit();
          Assert.AreEqual(name, location.Name, "Name");
          Assert.AreEqual(notes, location.Notes, "Notes");
          var duplicate = new Location { Name = name };
          session.BeginUpdate();
          Assert.Throws<UniqueConstraintException>(
            ()=>session.Persist(duplicate),"Duplicate not allowed");
        } catch {
          session.Abort();
          throw;
        }
      }
    }
  }
}