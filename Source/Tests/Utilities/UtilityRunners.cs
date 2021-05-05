using System.Diagnostics.CodeAnalysis;
using System.IO;
using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;
using VelocityDb.Session;

namespace SoundExplorers.Tests.Utilities {
  /// <summary>
  ///   A runner for the utility that generates an initialised database and/or a test
  ///   database.
  /// </summary>
  /// <remarks>
  ///   The <see cref="ExplicitAttribute" /> instructs NUnit to ignore the generator
  ///   test(s) unless explicitly selected for running. We don't want to generate the
  ///   main test database, which is used for GUI tests, every time all tests are run.
  /// </remarks>
  [TestFixture]
  [Explicit]
  [ExcludeFromCodeCoverage]
  public class UtilityRunners {
    [SetUp]
    public void Setup() {
      DatabaseGenerator = new DatabaseGenerator {UseLicenceRemovalWorkaround = false};
    }

    private DatabaseGenerator DatabaseGenerator { get; set; } = null!;

    [Test]
    public void GenerateData() {
      DatabaseGenerator.GenerateTestDatabase(
        112, 2019, false); // 112
    }

    [Test]
    public void GenerateInitialisedDatabase() {
      DatabaseGenerator.GenerateInitialisedDatabase();
      var testDatabaseFolder =
        new DirectoryInfo(TestSession.GenerateDatabaseFolderPath());
      testDatabaseFolder.Create();
      try {
        DatabaseConnection.InitialiseDatabase(
          DatabaseGenerator.InitialisedDatabaseFolderPath!, 
          testDatabaseFolder.FullName);
        var data = new TestData(new QueryHelper());
        var session = new SessionNoServer(testDatabaseFolder.FullName);
        session.BeginUpdate();
        data.AddSchemaPersisted(1, session);
        AddOneOfEachEntityTypePersisted(data, session);
        session.Commit();
      } finally {
        try {
          testDatabaseFolder.Delete(true);
        } catch {
          // Don't obscure the original exception, if any.
        }
      }
    }

    public static void AddOneOfEachEntityTypePersisted(
      TestData data, SessionBase session) {
      data.AddActsPersisted(1, session);
      data.AddArtistsPersisted(1, session);
      data.AddEventTypesPersisted(1, session);
      data.AddGenresPersisted(1, session);
      data.AddLocationsPersisted(1, session);
      data.AddNewslettersPersisted(1, session);
      data.AddRolesPersisted(1, session);
      data.AddSeriesPersisted(1, session);
      data.AddUserOptionsPersisted(1, session);
      data.AddEventsPersisted(1, session);
      data.AddSetsPersisted(1, session);
      data.AddPiecesPersisted(1, session);
      data.AddCreditsPersisted(1, session);
    }
  }
}