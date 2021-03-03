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
      EventList = new EventList {Session = Session};
    }

    [TearDown]
    public void TearDown() {
      Session.DeleteDatabaseFolderIfExists();
    }

    [Test]
    public void Locations() {
      var column = EventList.Columns["Location"];
      column.FetchReferenceableItems();
      var items = column.ReferenceableItems!;
      Assert.AreEqual(3, items.Count, "Count");
      var item = items[0];
      Assert.IsInstanceOf<KeyValuePair<object, object>>(item, "item");
      var (key, value) = (KeyValuePair<object, object>)item;
      Assert.IsInstanceOf<string>(key, "Key");
      Assert.IsInstanceOf<Location>(value, "Value");
    }

    [Test]
    public void Newsletters() {
      var column = EventList.Columns["Newsletter"];
      column.FetchReferenceableItems();
      var items = column.ReferenceableItems!;
      Assert.AreEqual(4, items.Count, "Count");
      Assert.IsTrue(items.ContainsKey("1900/01/01"), "Contains dummy Newsletter");
      Assert.IsTrue(items.ContainsKey("2020/01/06"), "Contains real Newsletter");
      var item1 = items[0];
      Assert.IsInstanceOf<KeyValuePair<object, object>>(item1, "item1");
      var (key1, value1) = (KeyValuePair<object, object>)item1;
      Assert.IsInstanceOf<string>(key1, "Key 1");
      Assert.IsInstanceOf<Newsletter>(value1, "Value 1");
      string formattedDate1 = key1.ToString()!;
      Assert.AreEqual("01 Jan 1900", formattedDate1, "formattedDate1");
      var item2 = items[1];
      Assert.IsInstanceOf<KeyValuePair<object, object>>(item2, "item2");
      var (key2, value2) = (KeyValuePair<object, object>)item2;
      Assert.IsInstanceOf<string>(key2, "Key 2");
      Assert.IsInstanceOf<Newsletter>(value2, "Value 2");
      string formattedDate2 = key2.ToString()!;
      Assert.AreEqual("06 Jan 2020", formattedDate2, "formattedDate2");
    }

    private TestData Data { get; set; } = null!;
    private EventList EventList { get; set; } = null!;
    private QueryHelper QueryHelper { get; set; } = null!;
    private TestSession Session { get; set; } = null!;
  }
}