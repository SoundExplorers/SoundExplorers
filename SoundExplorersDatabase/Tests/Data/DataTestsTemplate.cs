using NUnit.Framework;
using SoundExplorersDatabase.Data;

namespace SoundExplorersDatabase.Tests.Data {
  [TestFixture]
  public class DataTestsTemplate {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
      TestDataFactory = new TestDataFactory(QueryHelper);
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
    private TestDataFactory TestDataFactory { get; set; }

    [Test]
    public void A010_Initial() {
      Assert.IsNotNull(QueryHelper, "Dummy test");
      Assert.IsNotNull(TestDataFactory, "Another dummy test");
    }
  }
}