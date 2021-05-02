using NUnit.Framework;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests.Model {
  [TestFixture]
  public class SetComparerTests : TestFixtureBase {
    [SetUp]
    public override void Setup() {
      base.Setup();
      SetComparer = new SetComparer();
      Session.BeginUpdate();
      Data.AddEventTypesPersisted(1, Session);
      Data.AddNewslettersPersisted(1, Session);
      Data.AddSeriesPersisted(1, Session);
      Data.AddActsPersisted(1, Session);
      Data.AddGenresPersisted(1, Session);
      Session.Commit();
    }

    private SetComparer SetComparer { get; set; } = null!;

    [Test]
    public void DifferentDates() {
      Session.BeginUpdate();
      Data.AddLocationsPersisted(1, Session);
      Data.AddEventsPersisted(2, Session);
      Data.AddSetsPersisted(1, Session, Data.Events[0]);
      Data.AddSetsPersisted(1, Session, Data.Events[1]);
      Session.Commit();
      var set1 = Data.Sets[0];
      var set2 = Data.Sets[1];
      Assert.AreEqual(-1, SetComparer.Compare(set1, set2), "set1, set2");
      Assert.AreEqual(1, SetComparer.Compare(set2, set1), "set2, set1");
    }

    [Test]
    public void SameDateDifferentLocations() {
      Session.BeginUpdate();
      Data.AddLocationsPersisted(2, Session);
      var location1 = Data.Locations[0];
      location1.Name = "Location 1";
      var location2 = Data.Locations[1];
      location2.Name = "Location 2";
      Data.AddEventsPersisted(1, Session, location1);
      Data.AddEventsPersisted(1, Session, location2);
      var event1 = Data.Events[0];
      var event2 = Data.Events[1];
      event2.Date = event1.Date;
      Data.AddSetsPersisted(1, Session, event1);
      Data.AddSetsPersisted(1, Session, event2);
      Session.Commit();
      var set1 = Data.Sets[0];
      var set2 = Data.Sets[1];
      Assert.AreEqual(-1, SetComparer.Compare(set1, set2), "set1, set2");
      Assert.AreEqual(1, SetComparer.Compare(set2, set1), "set2, set1");
    }

    [Test]
    public void SameEventDifferentSetNos() {
      Session.BeginUpdate();
      Data.AddLocationsPersisted(1, Session);
      Data.AddEventsPersisted(1, Session);
      Data.AddSetsPersisted(2, Session);
      Session.Commit();
      var set1 = Data.Sets[0];
      var set2 = Data.Sets[1];
      Assert.AreEqual(-1, SetComparer.Compare(set1, set2), "set1, set2");
      Assert.AreEqual(1, SetComparer.Compare(set2, set1), "set2, set1");
    }
  }
}