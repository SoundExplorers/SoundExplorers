using NUnit.Framework;
using SoundExplorersDatabase.Data;
using System.IO;

namespace SoundExplorersDatabase.Tests.Data {
  [TestFixture]
  internal class SchemaVersionTests {

    [Test]
    public void Upgrade() {
      string databaseFolderPath;
      using (var session = TestSession.Create()) {
        databaseFolderPath = session.DatabaseFolderPath;
        Assert.IsTrue(
          Directory.Exists(databaseFolderPath),
          "Database folder exists after session creation.");
        var version = SchemaVersion.Read(expectedNumber: 1, session);
        Assert.AreEqual(0, version.Number, "Number before upgrade 1.");
        Assert.IsFalse(version.IsUpToDate, "SchemaVersion up to date before upgrade 1.");
        version.Upgrade();
        Assert.AreEqual(1, version.Number, "Number after upgrade 1.");
        Assert.IsTrue(version.IsUpToDate, "SchemaVersion up to date after upgrade 1.");
        version = SchemaVersion.Read(expectedNumber: 2, session);
        version.Upgrade();
        Assert.AreEqual(2, version.Number, "Number after upgrade 2.");
        Assert.IsTrue(version.IsUpToDate, "SchemaVersion up to date after upgrade 2.");
        Assert.IsTrue(
          Directory.Exists(databaseFolderPath),
          "Database folder exists before disposal.");
      }
      Assert.IsFalse(Directory.Exists(databaseFolderPath),
        "Database folder exists after disposal.");
    }
  }
}
