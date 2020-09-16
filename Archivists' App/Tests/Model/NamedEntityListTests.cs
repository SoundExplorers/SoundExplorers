using JetBrains.Annotations;
using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests.Model {
  [TestFixture]
  public class NamedEntityListTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      Data = new TestData(QueryHelper);
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
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

    private TestData Data { get; set; }
    private string DatabaseFolderPath { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private TestSession Session { get; set; }

    [Test]
    public void A010_Initial() {
      Initial<EventTypeList>("EventType");
    }

    private static void Initial<TEntityList>([NotNull] string tableName)
      where TEntityList : IEntityList, new() {
      var list = new TEntityList();
      Assert.AreEqual(1, list.Columns.Count, "Columns.Count");
      Assert.IsFalse(list.IsParentList, "IsParentList");
      Assert.IsNull(list.ParentListType, "ParentListType");
      Assert.AreEqual(tableName, list.Table.TableName, "TableName");
    }
  }
}