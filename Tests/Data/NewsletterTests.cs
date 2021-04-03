using System;
using NUnit.Framework;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data {
  [TestFixture]
  public class NewsletterTests : TestFixtureBase {
    [SetUp]
    public override void Setup() {
      base.Setup();
      DefaultNewsletter = Newsletter.CreateDefault();
      DefaultSeries = Series.CreateDefault();
      Location1 = new Location {
        QueryHelper = QueryHelper,
        Name = Location1Name
      };
      Newsletter1 = new Newsletter {
        QueryHelper = QueryHelper,
        Date = Newsletter1Date,
        Url = Newsletter1Url
      };
      Newsletter2 = new Newsletter {
        QueryHelper = QueryHelper,
        Date = Newsletter2Date,
        Url = Newsletter2Url
      };
      Event1 = new Event {
        QueryHelper = QueryHelper,
        Date = Event1Date
      };
      Event2 = new Event {
        QueryHelper = QueryHelper,
        Date = Event2Date
      };
      Session.BeginUpdate();
      Session.Persist(DefaultNewsletter);
      Session.Persist(DefaultSeries);
      Session.Persist(Location1);
      Session.Persist(Newsletter1);
      Session.Persist(Newsletter2);
      Event1.Location = Location1;
      Event1.Newsletter = Newsletter1;
      Event2.Location = Location1;
      Data.AddEventTypesPersisted(1, Session);
      Event1.EventType = Data.EventTypes[0];
      Event2.EventType = Event1.EventType;
      Session.Persist(Event1);
      Session.Persist(Event2);
      Session.Commit();
    }

    private const string Location1Name = "Pyramid Club";
    private const string Newsletter1SimpleKey = "2013/04/11";
    private const string Newsletter2SimpleKey = "2013/04/22";
    private Newsletter DefaultNewsletter { get; set; } = null!;
    private Series DefaultSeries { get; set; } = null!;
    private Event Event1 { get; set; } = null!;
    private static DateTime Event1Date => DateTime.Today.AddDays(-1);
    private Event Event2 { get; set; } = null!;
    private static DateTime Event2Date => DateTime.Today;
    private Location Location1 { get; set; } = null!;
    private Newsletter Newsletter1 { get; set; } = null!;
    private static DateTime Newsletter1Date => DateTime.Parse(Newsletter1SimpleKey);

    private static string Newsletter1Url =>
      "https://archive.org/details/simpsons-lat.375923";

    private Newsletter Newsletter2 { get; set; } = null!;
    private static DateTime Newsletter2Date => DateTime.Parse(Newsletter2SimpleKey);

    private static string Newsletter2Url =>
      "https://archive.org/details/BDChaurasiasHumanAnatomyVolume1MedicosTimes";

    [Test]
    public void A010_Initial() {
      Session.BeginRead();
      DefaultNewsletter =
        QueryHelper.Read<Newsletter>(DefaultNewsletter.SimpleKey, Session);
      Newsletter1 =
        QueryHelper.Read<Newsletter>(Newsletter1SimpleKey, Session);
      Newsletter2 =
        QueryHelper.Read<Newsletter>(Newsletter2SimpleKey, Session);
      Event1 = QueryHelper.Read<Event>(Event1.SimpleKey, Location1, Session);
      Event2 = QueryHelper.Read<Event>(Event2.SimpleKey, Location1, Session);
      Session.Commit();
      Assert.AreEqual(Newsletter1Date, Newsletter1.Date, "Newsletter1.Date");
      Assert.AreEqual(Newsletter1SimpleKey, Newsletter1.SimpleKey,
        "Newsletter1.SimpleKey");
      Assert.AreEqual(Newsletter1Url, Newsletter1.Url, "Newsletter1.Url");
      Assert.AreEqual(Newsletter2Date, Newsletter2.Date, "Newsletter2.Date");
      Assert.AreEqual(Newsletter2Url, Newsletter2.Url, "Newsletter2.Url");
      Assert.AreEqual(1, Newsletter1.Events.Count, "Newsletter1.Events.Count");
      Assert.AreSame(Newsletter1, Event1.Newsletter, "Event1.Newsletter");
      Assert.AreEqual(Newsletter1.Date, Event1.Newsletter.Date,
        "Event1.Newsletter.Date");
      Assert.AreSame(DefaultNewsletter, Event2.Newsletter, "Event2.Newsletter");
    }

    [Test]
    public void ChangeUrl() {
      const string newUrl =
        "https://docs.microsoft.com/en-us/dotnet/api/system.tuple?view=netcore-3.1";
      Session.BeginUpdate();
      Newsletter1 =
        QueryHelper.Read<Newsletter>(Newsletter1SimpleKey, Session);
      Assert.DoesNotThrow(() => Newsletter1.Url = newUrl);
      Session.Commit();
    }

    [Test]
    public void DisallowChangeDateToDuplicate() {
      Session.BeginUpdate();
      Newsletter2 =
        QueryHelper.Read<Newsletter>(Newsletter2SimpleKey, Session);
      Newsletter2.Date = Newsletter2Date;
      Assert.Throws<PropertyConstraintException>(() =>
        Newsletter2.Date = Newsletter1Date);
      Session.Commit();
    }

    [Test]
    public void DisallowChangeUrlToDuplicate() {
      Session.BeginUpdate();
      Newsletter2 =
        QueryHelper.Read<Newsletter>(Newsletter2SimpleKey, Session);
      Newsletter2.Url = Newsletter2Url;
      Assert.Throws<PropertyConstraintException>(() =>
        Newsletter2.Url = Newsletter1Url);
      Session.Commit();
    }

    [Test]
    public void DisallowPersistDuplicateDate() {
      var date = DateTime.Parse("2020/08/19");
      const string url1 = "https://archive.org/details/jazzpop";
      const string url2 = "https://archive.org/details/native_201910";
      var original = new Newsletter {
        QueryHelper = QueryHelper,
        Date = date,
        Url = url1
      };
      var duplicate = new Newsletter {
        QueryHelper = QueryHelper,
        Date = date,
        Url = url2
      };
      Session.BeginUpdate();
      Session.Persist(original);
      Assert.Throws<PropertyConstraintException>(() => Session.Persist(duplicate));
      Session.Commit();
    }

    [Test]
    public void DisallowPersistDuplicateUrl() {
      var date1 = DateTime.Parse("2020/08/18");
      var date2 = DateTime.Parse("2020/08/19");
      const string url = "https://archive.org/details/jazzpop";
      var original = new Newsletter {
        QueryHelper = QueryHelper,
        Date = date1,
        Url = url
      };
      var duplicate = new Newsletter {
        QueryHelper = QueryHelper,
        Date = date2,
        Url = url
      };
      Session.BeginUpdate();
      Session.Persist(original);
      Assert.Throws<PropertyConstraintException>(() => Session.Persist(duplicate));
      Session.Commit();
    }

    [Test]
    public void DisallowPersistUnspecifiedDate() {
      const string url = "https://archive.org/details/jazzpop";
      var noDate = new Newsletter {
        QueryHelper = QueryHelper,
        Url = url
      };
      Session.BeginUpdate();
      Assert.Throws<PropertyConstraintException>(() => Session.Persist(noDate));
      Session.Commit();
    }

    [Test]
    public void DisallowPersistUnspecifiedUrl() {
      var date = DateTime.Parse("2020/08/19");
      var noUrl = new Newsletter {
        QueryHelper = QueryHelper,
        Date = date
      };
      Session.BeginUpdate();
      Assert.Throws<PropertyConstraintException>(() => Session.Persist(noUrl));
      Session.Commit();
    }

    [Test]
    public void DisallowSetInvalidUrl() {
      Assert.Throws<PropertyConstraintException>(() => Newsletter2.Url = "Invalid URL");
    }

    [Test]
    public void DisallowSetMinimumDate() {
      Assert.Throws<PropertyConstraintException>(() =>
        Newsletter2.Date = DateTime.MinValue);
    }

    [Test]
    public void DisallowSetNullUrl() {
      Assert.Throws<PropertyConstraintException>(() => Newsletter2.Url = null!);
    }
  }
}