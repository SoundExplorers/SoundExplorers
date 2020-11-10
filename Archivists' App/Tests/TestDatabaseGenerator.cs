using System.Diagnostics.CodeAnalysis;
using System.IO;
using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests {
  [TestFixture]
  public class TestDatabaseGenerator {
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
    [ExcludeFromCodeCoverage]
    private static void Generate() {
      var queryHelper = new QueryHelper();
      var data = new TestData(queryHelper);
      TestSession.DeleteFolderIfExists(DatabaseConfig.DefaultDatabaseFolderPath);
      Directory.CreateDirectory(DatabaseConfig.DefaultDatabaseFolderPath);
      var session = new TestSession(DatabaseConfig.DefaultDatabaseFolderPath);
      session.BeginUpdate();
      data.AddActsPersisted(10, session);
      data.AddEventTypesPersisted(5, session);
      data.AddGenresPersisted(10, session);
      data.AddLocationsPersisted(8, session);
      data.AddNewslettersPersisted(64, session);
      data.AddSeriesPersisted(8, session);
      session.Commit();
    }
  }
}