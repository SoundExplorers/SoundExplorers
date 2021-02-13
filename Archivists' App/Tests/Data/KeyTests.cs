using System;
using NUnit.Framework;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data {
  [TestFixture]
  public class KeyTests {
    [SetUp]
    public void Setup() {
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
      Data = new TestData(new QueryHelper());
    }

    [TearDown]
    public void TearDown() {
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }

    private string DatabaseFolderPath { get; set; } = null!;
    private TestData Data { get; set; } = null!;

    [Test]
    public void ConvertToString() {
      const string dateString = "1900/12/25";
      const string locationName = "Fred's";
      const int setNo = 1;
      var event1 = new Event();
      Assert.IsEmpty(event1.Key.ToString(),
        "Null SimpleKey and Identifying Parent");
      event1.Date = DateTime.Parse(dateString);
      Assert.AreEqual(event1.Key.ToString(), dateString, "SimpleKey only");
      event1.Location = new Location {Name = locationName};
      Assert.AreEqual(event1.Key.ToString(), "1900/12/25 | Fred's",
        "SimpleKey and Identifying Parent");
      var set1 = new Set {SetNo = setNo, Event = event1};
      Assert.AreEqual(set1.Key.ToString(), "01 | 1900/12/25 | Fred's", "set1");
    }

    [Test]
    public void CaseInsensitive() {
      var location1A = new Location {Name = "Fred's"};
      var location1B = new Location {Name = "fred's"};
      var location2A = new Location {Name = "Pyramid Club"};
      var location2B = new Location {Name = "pyramid club"};
      Assert.IsTrue(location1A.Key == location1B.Key, "location1A.Key == location1B.Key");
      Assert.IsTrue(location1B.Key < location2A.Key, "location1B.Key < location2A.Key");
      Assert.IsTrue(location2B.Key > location1A.Key, "location2B.Key > location1A.Key");
    }

    [Test]
    public void Difference() {
      var location1 = new Location {Name = "Fred's"};
      var location2 = new Location {Name = "Pyramid Club"};
      Event event1;
      Event event2;
      Event event3;
      Event event4;
      Event event5;
      Event event6;
      Event event7;
      Set set1;
      Set set2;
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        session.Persist(location1);
        session.Persist(location2);
        Data.AddActsPersisted(1, session);
        Data.AddEventTypesPersisted(1, session);
        Data.AddNewslettersPersisted(1, session);
        Data.AddSeriesPersisted(1, session);
        event1 = new Event {
          Date = DateTime.Today,
          Location = location1,
          EventType = Data.EventTypes[0]
        };
        event2 = new Event {
          Date = event1.Date, Location = location2, EventType = event1.EventType
        };
        // Later date but alphabetically prior location compared with event 2
        event3 = new Event {
          Date = event1.Date.AddDays(1), Location = location1,
          EventType = event1.EventType
        };
        event4 = new Event {
          Date = event1.Date.AddDays(2), Location = location1,
          EventType = event1.EventType
        };
        event5 = new Event {Date = event2.Date.AddDays(1)};
        event6 = new Event {
          Date = event5.Date, Location = location2, EventType = event1.EventType
        };
        event7 = new Event {Date = event5.Date};
        Data.AddGenresPersisted(1, session);
        set1 = new Set {
          SetNo = 1,
          Event = event1,
          Genre = Data.Genres[0]
        };
        set2 = new Set {SetNo = 1, Event = event2, Genre = set1.Genre};
        session.Commit();
      }
      Assert.IsTrue(event1.Key != event2.Key, "event1.Key != event2.Key");
      Assert.IsTrue(event1.Key < event2.Key, "event1.Key < event2.Key");
      Assert.IsTrue(event2.Key > event1.Key, "event2.Key > event1.Key");
      Assert.IsTrue(event3.Key != event4.Key, "event3.Key != event4.Key");
      Assert.IsTrue(event3.Key < event4.Key, "event3.Key < event4.Key");
      Assert.IsTrue(event4.Key > event3.Key, "event4.Key > event3.Key");
      Key? nullKey = null;
      Assert.IsTrue(event3.Key > nullKey, "event3.Key > nullKey");
      Assert.IsTrue(nullKey < event3.Key, "event3.Key < nullKey");
      Assert.IsTrue(event5.Key < event6.Key, "event5.Key < event6.Key");
      Assert.IsFalse(event5.Key < event7.Key, "event5.Key < event7.Key");
      Assert.IsTrue(set1.Key < set2.Key, "set1.Key < set2.Key");
      Assert.IsTrue(event2.Key < event3.Key, "event2.Key < event3.Key");
    }

    [Test]
    public void Equality() {
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
      string blankSimpleKey = string.Empty;
      var key5 = new Key(blankSimpleKey, location1);
      var key6 = new Key(blankSimpleKey, location1);
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
  }
}