using System;
using NUnit.Framework;
using SoundExplorersDatabase.Data;

namespace SoundExplorersDatabase.Tests.Data {
  [TestFixture]
  public class KeyTests {
    // private class MockLocation : MockRelativeBase {
    //   private string _name;
    //
    //   public string Name {
    //     get => _name;
    //     set {
    //       _name = value;
    //       SimpleKey = value;
    //     }
    //   }
    // }
    //
    // private class MockEvent : MockRelativeBase {
    //   private DateTime _date;
    //   private MockLocation _location;
    //
    //   public DateTime Date {
    //     get => _date;
    //     set {
    //       _date = value;
    //       SimpleKey = $"{value:yyyy/MM/dd}";
    //     }
    //   }
    //
    //   public MockLocation Location {
    //     get => _location;
    //     set {
    //       _location = value;
    //       IdentifyingParent = value;
    //     }
    //   }
    // }
    //
    // private class MockSet : MockRelativeBase {
    //   private MockEvent _event;
    //   private int _setNo;
    //
    //   public int SetNo {
    //     get => _setNo;
    //     set {
    //       _setNo = value;
    //       SimpleKey = value.ToString().PadLeft(2, '0');
    //     }
    //   }
    //
    //   public MockEvent Event {
    //     get => _event;
    //     set {
    //       _event = value;
    //       IdentifyingParent = value;
    //     }
    //   }
    // }

    [Test]
    public void T010_Equality() {
      var event1 = new Event {Date = DateTime.Parse("2013/01/02")};
      var event2 = new Event {Date = event1.Date};
      var location1 = new Location {Name = "Fred's"};
      var key1 = new Key(event1.SimpleKey, location1);
      var key2 = new Key(event2.SimpleKey, location1);
      Assert.AreEqual(key1.GetHashCode(), key2.GetHashCode(),
        "key2.GetHashCode()");
      Assert.AreEqual(key1, key2, "key2");
      Assert.IsTrue(key1 == key2, "key1 == key2");
      var key3 = new Key(event1.SimpleKey, null);
      var key4 = new Key(event1.SimpleKey, null);
      Assert.AreEqual(key3.GetHashCode(), key4.GetHashCode(),
        "key4.GetHashCode()");
      Assert.AreEqual(key3, key4, "key4");
      Assert.IsTrue(key3 == key4, "key3 == key4");
      var key5 = new Key(null, location1);
      var key6 = new Key(null, location1);
      Assert.AreEqual(key5.GetHashCode(), key6.GetHashCode(),
        "key6.GetHashCode()");
      Assert.AreEqual(key5, key6, "key6");
      Assert.IsTrue(key5 == key6, "key5 == key6");
      Assert.AreNotEqual(key3.GetHashCode(), key5.GetHashCode(),
        "key6.GetHashCode()");
      Assert.AreNotEqual(key3, key5, "key5");
      Assert.IsFalse(key3 == key5, "key5 == key6");
      Assert.IsTrue(key3 != key5, "key5 == key6");
      key2 = null;
      Assert.IsTrue(key1 != key2, "null != !null");
      key1 = null;
      Assert.IsTrue(key1 == key2, "null == null");
    }

    [Test]
    public void T020_Difference() {
      var location1 = new Location {Name = "Fred's"};
      var location2 = new Location {Name = "Pyramid Club"};
      Assert.IsTrue(location1.Key != location2.Key,
        "location1.Key != location2.Key");
      Assert.IsTrue(location1.Key < location2.Key,
        "location1.Key < location2.Key");
      Assert.IsTrue(location2.Key > location1.Key,
        "location2.Key > location1.Key");
      var event1 = new Event {Date = DateTime.Today, Location = location1};
      var event2 = new Event {Date = event1.Date, Location = location2};
      Assert.IsTrue(event1.Key != event2.Key, "event1.Key != event2.Key");
      Assert.IsTrue(event1.Key < event2.Key, "event1.Key < event2.Key");
      Assert.IsTrue(event2.Key > event1.Key, "event2.Key > event1.Key");
      var event3 = new Event {Date = DateTime.Today, Location = location1};
      var event4 = new Event
        {Date = event1.Date.AddDays(1), Location = location1};
      Assert.IsTrue(event3.Key != event4.Key, "event3.Key != event4.Key");
      Assert.IsTrue(event3.Key < event4.Key, "event3.Key < event4.Key");
      Assert.IsTrue(event4.Key > event3.Key, "event4.Key > event3.Key");
      Key nullKey = null;
      Assert.IsTrue(event3.Key > nullKey, "event3.Key > nullKey");
      Assert.IsTrue(nullKey < event3.Key, "event3.Key < nullKey");
      var event5 = new Event {Date = DateTime.Today};
      var event6 = new Event {Date = event1.Date, Location = location2};
      Assert.IsTrue(event5.Key < event6.Key, "event5.Key < event6.Key");
      var set1 = new Set {SetNo = 1, Event = event1};
      var set2 = new Set {SetNo = 1, Event = event2};
      Assert.IsTrue(set1.Key < set2.Key, "set1.Key < set2.Key");
    }

    [Test]
    public void T030_ToString() {
      const string dateString = "1800/12/25";
      const string locationName = "Fred's";
      const int setNo = 1;
      var event1 = new Event();
      Assert.IsEmpty(event1.Key.ToString(),
        "Null SimpleKey and Identifying Parent");
      event1.Date = DateTime.Parse(dateString);
      Assert.AreEqual(event1.Key.ToString(), dateString, "SimpleKey only");
      event1.Location = new Location {Name = locationName};
      Assert.AreEqual(event1.Key.ToString(), "1800/12/25 Fred's",
        "SimpleKey and Identifying Parent");
      var set1 = new Set {SetNo = setNo, Event = event1};
      Assert.AreEqual(set1.Key.ToString(), "01 1800/12/25 Fred's", "set1");
    }
  }
}