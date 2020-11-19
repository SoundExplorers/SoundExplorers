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
      Data.AddNewslettersPersisted(4, Session);
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
      Assert.AreEqual(5,items.Count, "Count");
      Assert.IsTrue(items.ContainsKey("1900/01/01"), "Contains dummy Newsletter");
      Assert.IsTrue(items.ContainsKey("2020/01/06"), "Contains real Newsletter");
      var item1 = items[0];
      Assert.IsInstanceOf<KeyValuePair<object, object>>(item1, "item1");
      var pair1 = (KeyValuePair<object, object>)item1;
      Assert.IsInstanceOf<string>(pair1.Key, "Key 1");
      Assert.IsNull(pair1.Value, "Value 1");
      var formattedDate1 = pair1.Key.ToString();
      Assert.AreEqual("01 Jan 1900", formattedDate1, "formattedDate1");
      var item2 = items[1];
      Assert.IsInstanceOf<KeyValuePair<object, object>>(item2, "item2");
      var pair2 = (KeyValuePair<object, object>)item2;
      Assert.IsInstanceOf<string>(pair2.Key, "Key 2");
      Assert.IsInstanceOf<Newsletter>(pair2.Value, "Value 2");
      var formattedDate2 = pair2.Key.ToString();
      Assert.AreEqual("06 Jan 2020", formattedDate2, "formattedDate2");
    }

    private TestData Data { get; set; }
    private EventList EventList { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private TestSession Session { get; set; }
  }
}