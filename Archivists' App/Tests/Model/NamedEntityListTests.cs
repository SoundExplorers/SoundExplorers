using NUnit.Framework;

namespace SoundExplorers.Tests.Model {
  [TestFixture]
  public class NamedEntityListTests {
    // [SetUp]
    // public void Setup() {
    //   QueryHelper = new QueryHelper();
    //   Data = new TestData(QueryHelper);
    //   Session = new TestSession();
    //   DatabaseFolderPath = TestSession.CreateDatabaseFolder();
    //   using (var session = new TestSession(DatabaseFolderPath)) {
    //     session.BeginUpdate();
    //     new Schema().RegisterEntityTypes(session);
    //     session.Commit();
    //   }
    //   Session = new TestSession(DatabaseFolderPath);
    //   Session.BeginRead();
    //   Session.Commit();
    // }
    //
    // [TearDown]
    // public void TearDown() {
    //   Session.DeleteDatabaseFolderIfExists();
    // }
    //
    // private TestData Data { get; set; }
    // private QueryHelper QueryHelper { get; set; }
    // private TestSession Session { get; set; }

    // [Test]
    // public void A010_Initial() {
    //   A010_Initial<EventTypeList>("EventType");
    // }
    //
    // private void A010_Initial<TEntityList>([NotNull] string tableName)
    //   where TEntityList : IEntityList, new() {
    //   var list = new TEntityList();
    //   Assert.AreEqual(1, list.Columns.Count, "Columns.Count");
    //   Assert.IsFalse(list.IsParentList, "IsParentList");
    //   Assert.IsNull(list.ParentListType, "ParentListType");
    //   Assert.AreEqual(tableName, list.Table.TableName, "TableName");
    //   Assert.AreEqual("Name", list.Table.Columns[0].ColumnName, "[0].ColumnName");
    //   Assert.AreEqual(typeof(string), list.Table.Columns[0].DataType, "[0].DataType");
    //   list.Session = Session;
    //   list.Populate();
    //   Assert.IsEmpty(list.Table.Rows, "Rows initially");
    // }
    //
    // [Test]
    // public void EditTable() {
    //   EditTable<EventTypeList>();
    // }
    //
    // private void EditTable<TEntityList>()
    //   where TEntityList : IEntityList, new() {
    //   const string name1 = "Performance";
    //   const string name2 = "Interview";
    //   const string name3 = "Rehearsal";
    //   var list = new TEntityList {Session = Session};
    //   list.Table.Rows.Add(name1);
    //   list.OnRowLeft(0);
    //   Assert.AreEqual(1, list.Count, "Entity count after 1st add");
    //   Assert.AreEqual(name1, ((INamedEntity)list[0]).Name, "1st entity Name after 1st add");
    //   list.Table.Rows.Add(name2);
    //   list.OnRowLeft(1);
    //   Assert.AreEqual(2, list.Count, "Entity count after 2nd add");
    //   Assert.AreEqual(name2, ((INamedEntity)list[1]).Name, "2nd entity Name after 2nd add");
    //   list.Populate();
    //   Assert.AreEqual(2, list.Table.Rows.Count, "Row count after populate");
    //   // After being refreshed by Populate, the table should now be sorted into Name order.
    //   Assert.AreEqual(name2, list.Table.Rows[0]["Name"], "1st row Name after populate");
    //   Assert.AreEqual(name1, list.Table.Rows[1]["Name"], "2nd row Name after populate");
    //   list.Table.Rows[0]["Name"] = name3;
    //   list.OnRowLeft(0);
    //   Assert.AreEqual(name3, ((INamedEntity)list[0]).Name, "1st entity Name after update");
    //   list.DeleteEntity(0);
    //   list.Populate();
    //   Assert.AreEqual(1, list.Count, "Entity count after delete and repopulate");
    //   Assert.AreEqual(name1, ((INamedEntity)list[0]).Name,
    //     "1st entity Name after delete and repopulate");
    //   Assert.AreEqual(1, list.Table.Rows.Count, "Row count after delete and repopulate");
    //   Assert.AreEqual(name1, list.Table.Rows[0]["Name"],
    //     "1st row Name after delete and repopulate");
    // }
    //
    // [Test]
    // public void ErrorOnDelete() {
    //   Session.BeginUpdate();
    //   Data.AddEventTypesPersisted(2, Session);
    //   Data.AddEventsPersisted(3, Session, eventType: Data.EventTypes[1]);
    //   Session.Commit();
    //   ErrorOnDelete<EventTypeList>((IList)Data.EventTypes);
    // }
    //
    // private void ErrorOnDelete<TEntityList>([NotNull] IList entities)
    //   where TEntityList : IEntityList, new() {
    //   string name = ((INamedEntity)entities[1]).Name;
    //   var list = new TEntityList {Session = Session};
    //   list.Populate(entities);
    //   list.BackupRow(1);
    //   try {
    //     list.DeleteEntity(1);
    //     Assert.Fail("DeleteEntity should have thrown RowErrorException.");
    //   } catch (DatabaseUpdateErrorException exception) {
    //     Assert.IsTrue(
    //       exception.Message.Contains("cannot be deleted because it is referenced by"),
    //       "Message");
    //     Assert.AreEqual(1, exception.RowIndex, "RowIndex");
    //     Assert.IsNotNull(exception.RejectedValues, "RejectedValues");
    //     Assert.AreEqual(name, exception.RejectedValues[0], "RejectedValues[0]");
    //     Assert.IsInstanceOf(typeof(ConstraintException), exception.InnerException,
    //       "InnerException");
    //   }
    // }
    //
    // [Test]
    // public void ErrorOnUpdate() {
    //   ErrorOnUpdate<EventTypeList>();
    // }
    //
    // private void ErrorOnUpdate<TEntityList>()
    //   where TEntityList : IEntityList, new() {
    //   const string name1 = "Performance";
    //   const string name2 = "Rehearsal";
    //   var list = new TEntityList {Session = Session};
    //   list.Table.Rows.Add(name1);
    //   list.OnRowLeft(0);
    //   list.Table.Rows.Add(name2);
    //   list.OnRowLeft(1);
    //   list.Table.Rows[1]["Name"] = name1;
    //   try {
    //     list.OnRowLeft(1);
    //   } catch (Exception e) {
    //     Console.WriteLine(e);
    //     throw;
    //   }
    // }
  }
}