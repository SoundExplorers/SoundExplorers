using NUnit.Framework;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data {
  [TestFixture]
  public class DataTestsTemplate {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
      TestData = new TestData(QueryHelper);
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        session.Commit();
      }
      Session = new TestSession(DatabaseFolderPath);
      Session.BeginRead();
      Session.Commit();
    }

    [TearDown]
    public void TearDown() {
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }

    private string DatabaseFolderPath { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private TestSession Session { get; set; }
    private TestData TestData { get; set; }

    [Test]
    public void A010_Initial() {
      Assert.IsTrue(QueryHelper != null && TestData != null,
        "Dummy test");
    }
  }
}