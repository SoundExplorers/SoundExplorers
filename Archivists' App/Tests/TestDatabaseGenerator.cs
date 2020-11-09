using System.Diagnostics.CodeAnalysis;
using System.IO;
using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;
using VelocityDb.Session;

namespace SoundExplorers.Tests {
  [TestFixture]
  public class TestDatabaseGenerator {
    private TestData Data { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private SessionBase Session { get; set; }

    // /// <summary>
    // ///   Comment out unless the main test database needs to be regenerated.
    // /// </summary>
    // /// <remarks>
    // ///   If the main test database folder already exists,
    // ///   it will be deleted and recreated from scratch.
    // /// </remarks>
    // [Test]
    // public void GenerateData() {
    //   Generate();
    // }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private void Generate() {
      QueryHelper = new QueryHelper();
      Data = new TestData(QueryHelper);
      TestSession.DeleteFolderIfExists(DatabaseConfig.DefaultDatabaseFolderPath);
      Directory.CreateDirectory(DatabaseConfig.DefaultDatabaseFolderPath);
      Session = new TestSession(DatabaseConfig.DefaultDatabaseFolderPath);
      Session.BeginUpdate();
      Data.AddActsPersisted(10, Session);
      Data.AddEventTypesPersisted(5, Session);
      Data.AddGenresPersisted(10, Session);
      Data.AddLocationsPersisted(8, Session);
      Data.AddNewslettersPersisted(64, Session);
      Data.AddSeriesPersisted(8, Session);
      Session.Commit();
    }
  }
}