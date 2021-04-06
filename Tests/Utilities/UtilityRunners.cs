using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

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
      DatabaseGenerator.GenerateTestDatabase(112, 2019, false);
    }

    [Test]
    public void GenerateInitialisedDatabase() {
      DatabaseGenerator.GenerateInitialisedDatabase();
    }
  }
}