using NUnit.Framework;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data {
  [TestFixture]
  public class SchemaTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
      Session = new TestSession(DatabaseFolderPath);
    }

    [TearDown]
    public void TearDown() {
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }

    private string DatabaseFolderPath { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private TestSession Session { get; set; }

    [Test]
    public void NewSchema() {
      // Must be update transaction to allow for the existence of
      // the database schema file ('schema database')to be checked.
      Session.BeginUpdate();
      var schema = Schema.Find(QueryHelper, Session);
      Session.Commit();
      Assert.IsNull(schema, "Schema initially");
      schema = new Schema();
      Assert.AreEqual(0, schema.Version, "Version after creating Schema");
      Session.BeginUpdate();
      schema.Upgrade(1, Session);
      Session.Persist(schema);
      schema = Schema.Find(QueryHelper, Session);
      Session.Commit();
      Assert.IsNotNull(schema, "Schema after finding persisted occurence");
      Assert.AreEqual(1, schema.Version, "Version finding persisted occurence");
    }
  }
}