using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests.Model {
  [TestFixture]
  public class LocationListTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      Data = new TestData(QueryHelper);
      Session = new TestSession();
    }

    [TearDown]
    public void TearDown() {
      Session.DeleteDatabaseFolderIfExists();
    }

    private TestData Data { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private TestSession Session { get; set; }

    [Test]
    public void GetChildren() {
      Session.BeginUpdate();
      Data.AddEventTypesPersisted(1, Session);
      Data.AddLocationsPersisted(1, Session);
      Data.AddEventsPersisted(3, Session);
      Session.Commit();
      var list = new LocationList {Session = Session};
      list.Populate();
      var children = list.GetChildren(0);
      Assert.AreEqual(3, children?.Count, "Count");
      Assert.IsInstanceOf<Event>(children?[0], "Child type");
    }
  }
}