using NUnit.Framework;
using SoundExplorersDatabase.Data;
using System.IO;

namespace SoundExplorersDatabase.Tests.Data {
  [TestFixture]
  internal class DatabaseSchemaTest {

    [Test]
    public void NewSchema() {
      string databaseFolderPath;
      using (var session = TestSession.Create()) {
        databaseFolderPath = session.DatabaseFolderPath;
        Assert.IsTrue(
          Directory.Exists(databaseFolderPath),
          "Database folder exists after session creation.");
        var schema = new DatabaseSchema(expectedVersionNo: 1, session);
        Assert.AreEqual(schema.VersionNo, -1, "VersionNo before update 1.");
        Assert.IsFalse(schema.IsUpToDate, "Schema up to date before update 1.");
        schema.Update();
        Assert.AreEqual(schema.VersionNo, 1, "VersionNo after update 1.");
        Assert.IsTrue(schema.IsUpToDate, "Schema up to date after update 1.");
        schema = new DatabaseSchema(expectedVersionNo: 2, session);
        Assert.AreEqual(schema.VersionNo, -1, "VersionNo before update 2.");
        Assert.IsFalse(schema.IsUpToDate, "Schema up to date before update 2.");
        schema.Update();
        Assert.AreEqual(schema.VersionNo, 2, "VersionNo after update 2.");
        Assert.IsTrue(schema.IsUpToDate, "Schema up to date after update 2.");
        Assert.IsTrue(
          Directory.Exists(databaseFolderPath),
          "Database folder exists before disposal.");
      }
      Assert.IsFalse(Directory.Exists(databaseFolderPath),
        "Database folder exists after disposal.");
    }
  }
}
