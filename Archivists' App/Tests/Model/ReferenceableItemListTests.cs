using System.Collections.Generic;
using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests.Model {
  [TestFixture]
  public class ReferenceableItemListTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      Data = new TestData(QueryHelper);
      Session = new TestSession();
      Session.BeginUpdate();
      Data.AddLocationsPersisted(3, Session);
      Data.AddNewslettersPersisted(2, Session);
      Session.Commit();
      EventList = new EventList() {Session = Session};
    }

    [TearDown]
    public void TearDown() {
      Session.DeleteDatabaseFolderIfExists();
    }

    [Test]
    public void Locations() {
      var items = EventList.Columns["Location"].ReferenceableItems;
      Assert.AreEqual(3,items.Count, "Count");
      var item = items[0];
      Assert.IsInstanceOf<KeyValuePair<object, object>>(item, "item");
      var pair = (KeyValuePair<object, object>)item;
      Assert.IsInstanceOf<string>(pair.Key, "Key");
      Assert.IsInstanceOf<Location>(pair.Value, "Value");
    }

    [Test]
    public void Newsletters() {
      var items = EventList.Columns["Newsletter"].ReferenceableItems;
      Assert.AreEqual(2,items.Count, "Count");
      var item = items[0];
      Assert.IsInstanceOf<KeyValuePair<object, object>>(item, "item");
      var pair = (KeyValuePair<object, object>)item;
      Assert.IsInstanceOf<string>(pair.Key, "Key");
      Assert.IsInstanceOf<Newsletter>(pair.Value, "Value");
      var formattedDate = pair.Key.ToString();
      Assert.AreEqual("06 Jan 2020", formattedDate, "formattedDate");
    }

    private TestData Data { get; set; }
    private EventList EventList { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private TestSession Session { get; set; }
  }
}