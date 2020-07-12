using NUnit.Framework;
using SoundExplorersDatabase.Data;
using System.IO;

namespace SoundExplorersDatabase.Tests.Data {
  [TestFixture]
  internal class DatabaseSchemaTest {

    private static readonly string _databaseFolderPath = "C:\\Simon\\Database";

    [Test]
    public void NewSchema() {
      RemoveFolderIfExists(_databaseFolderPath);
      Assert.IsFalse(Directory.Exists(_databaseFolderPath), "Database folder exists after initial removal.");
      Directory.CreateDirectory(_databaseFolderPath);
      Assert.IsTrue(Directory.Exists(_databaseFolderPath), "Database folder exists after creation.");
      using (DatabaseSchema schema = new DatabaseSchema(expectedVersionNo: 1, _databaseFolderPath)) {
        Assert.AreEqual(schema.VersionNo, -1, "VersionNo before update 1.");
        Assert.IsFalse(schema.IsUpToDate, "Schema up to date before update 1.");
        schema.Update();
        Assert.AreEqual(schema.VersionNo, 1, "VersionNo after update 1.");
        Assert.IsTrue(schema.IsUpToDate, "Schema up to date after update 1.");
      }
      using (DatabaseSchema schema = new DatabaseSchema(expectedVersionNo: 2, _databaseFolderPath)) {
        Assert.AreEqual(schema.VersionNo, -1, "VersionNo before update 2.");
        Assert.IsFalse(schema.IsUpToDate, "Schema up to date before update 2.");
        schema.Update();
        Assert.AreEqual(schema.VersionNo, 2, "VersionNo after update 2.");
        Assert.IsTrue(schema.IsUpToDate, "Schema up to date after update 2.");
      }
      Assert.IsTrue(Directory.Exists(_databaseFolderPath), "Database folder exists before final removal.");
      RemoveFolderIfExists(_databaseFolderPath);
      Assert.IsFalse(Directory.Exists(_databaseFolderPath), "Database folder exists after final removal.");
    }

    private void RemoveFolderIfExists(string folderPath) {
      if (Directory.Exists(folderPath)) {
        foreach (string filePath in Directory.GetFiles(folderPath)) {
          File.Delete(filePath);
        }
        Directory.Delete(folderPath);
      }
    }
  }
}
