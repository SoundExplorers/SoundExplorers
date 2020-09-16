using System.Linq.Expressions;
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
      Session = new TestSession();
      // DatabaseFolderPath = TestSession.CreateDatabaseFolder();
      // using (var session = new TestSession(DatabaseFolderPath)) {
      //   session.BeginUpdate();
      //   new Schema().RegisterEntityTypes(session);
      //   session.Commit();
      // }
      // Session = new TestSession(DatabaseFolderPath);
      // Session.BeginRead();
      // Session.Commit();
    }

    [TearDown]
    public void TearDown() {
      Session.DeleteDatabaseFolderIfExists();
    }

    private TestData Data { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private TestSession Session { get; set; }

    [Test]
    public void A010_Initial() {
      A010_Initial<EventTypeList>("EventType");
    }

    private void A010_Initial<TEntityList>([NotNull] string tableName)
      where TEntityList : IEntityList, new() {
      var list = new TEntityList();
      Assert.AreEqual(1, list.Columns.Count, "Columns.Count");
      Assert.IsFalse(list.IsParentList, "IsParentList");
      Assert.IsNull(list.ParentListType, "ParentListType");
      Assert.AreEqual(tableName, list.Table.TableName, "TableName");
      Assert.AreEqual("Name", list.Table.Columns[0].ColumnName, "[0].ColumnName");
      Assert.AreEqual(typeof(string), list.Table.Columns[0].DataType, "[0].DataType");
      list.Session = Session;
      list.Populate();
      Assert.IsEmpty(list.Table.Rows, "Rows initially");
    }

    [Test]
    public void EditTable()  {
      EditTable<EventTypeList>();
    }

    private void EditTable<TEntityList>()
      where TEntityList : IEntityList, new() {
      const string name1 = "Performance";
      const string name2 = "Interview";
      var list = new TEntityList();
      list.Table.Rows.Add(name1);
      list.Session = Session;
      list.AddOrUpdateEntityIfRequired(0);
      Assert.AreEqual(1, list.Count, "Count after 1st add");
      Assert.AreEqual(name1, ((INamed)list[0]).Name, "Name after 1st add");
      list.Table.Rows.Add(name2);
      list.AddOrUpdateEntityIfRequired(1);
      Assert.AreEqual(2, list.Count, "Count after 2nd add");
      Assert.AreEqual(name2, ((INamed)list[1]).Name, "2nd Name after 2nd add");
      list.Populate();
      Assert.AreEqual(2, list.Table.Rows.Count, "Row count after Populate");
      // After being refreshed by Populate, the table should now be sorted into Name order.
      Assert.AreEqual(name2, list.Table.Rows[0]["Name"], "Ist row Name after Populate");
      Assert.AreEqual(name1, list.Table.Rows[1]["Name"], "2nd row Name after Populate");
    }
  }
}