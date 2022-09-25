using NUnit.Framework;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data; 

[TestFixture]
public class SchemaTests : TestFixtureBase {
  [Test]
  public void NewSchema() {
    // Must be update transaction to allow for the existence of
    // the database schema file ('schema database')to be checked.
    Session.BeginUpdate();
    var schema = Schema.Find(QueryHelper, Session);
    Session.Commit();
    Assert.IsNull(schema, "Schema initially");
    schema = new Schema();
    Assert.IsFalse(schema.AllowOtherTypesOnSamePage, "AllowOtherTypesOnSamePage");
    Assert.AreEqual(0, schema.Version, "Version after creating Schema");
    Session.BeginUpdate();
    schema.RegisterPersistableTypes(Session);
    schema.Version = 1;
    Session.Persist(schema);
    Session.Commit();
    Session.BeginRead();
    schema = Schema.Find(QueryHelper, Session);
    Session.Commit();
    Assert.IsNotNull(schema, "Schema after finding persisted occurence");
    Assert.AreEqual(1, schema?.Version, "Version finding persisted occurence");
  }

  [Test]
  public void SetInstance() {
    var schema = new Schema();
    Schema.Instance = schema;
    Assert.AreSame(schema, Schema.Instance);
  }
}