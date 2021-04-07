using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Tests.Data;
using VelocityDb.Session;

namespace SoundExplorers.Tests.Utilities {
  /// <remarks>
  ///   The <see cref="ExplicitAttribute" /> instructs NUnit to ignored the generator
  ///   test(s) unless explicitly selected for running. We don't want to generate the
  ///   main test database, which is used for GUI tests, every time all tests are run.
  /// </remarks>
  [TestFixture]
  [Explicit]
  [ExcludeFromCodeCoverage]
  public class UtilityRunners {
    [SetUp]
    public void Setup() {
      DatabaseGenerator = new DatabaseGenerator();
    }

    private DatabaseGenerator DatabaseGenerator { get; set; } = null!;

    [Test]
    public void GenerateData() {
      DatabaseGenerator.GenerateTestDatabase(
        112, 2019, false);
    }

    [Test]
    public void GenerateInitialisedDatabase() {
      DatabaseGenerator.GenerateInitialisedDatabase();
      var initialisedDatabaseFolder =
        new DirectoryInfo(DatabaseGenerator.InitialisedDatabaseFolderPath!);
      var testDatabaseFolder =
        new DirectoryInfo(TestSession.GenerateDatabaseFolderPath());
      testDatabaseFolder.Create();
      try {
        foreach (var sourceFile in initialisedDatabaseFolder.GetFiles()) {
          string destinationPath = sourceFile.FullName.Replace(
            initialisedDatabaseFolder.FullName,
            testDatabaseFolder.FullName);
          sourceFile.CopyTo(destinationPath);
        }
        var data = new TestData(new QueryHelper());
        var session = new SessionNoServer(testDatabaseFolder.FullName);
        session.BeginUpdate();
        data.AddSchemaPersisted(1, session);
        DatabaseGenerator.AddOneOfEachEntityTypePersisted(data, session);
        session.Commit();
      } finally {
        testDatabaseFolder.Delete(true);  
      }
    }
  }
}