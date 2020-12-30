using System;
using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests.Model {
  [TestFixture]
  public class NewsletterListTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      Data = new TestData(QueryHelper);
      Session = new TestSession();
      List = new NewsletterList {Session = Session};
    }

    [TearDown]
    public void TearDown() {
      Session.DeleteDatabaseFolderIfExists();
    }

    private TestData Data { get; set; } = null!;
    private NewsletterList List { get; set; } = null!;
    private QueryHelper QueryHelper { get; set; } = null!;
    private TestSession Session { get; set; } = null!;

    [Test]
    public void A010_Initial() {
      Assert.AreEqual("Newsletter", List.EntityTypeName, "EntityName");
      Assert.AreEqual(2, List.Columns.Count, "Columns.Count");
      Assert.AreEqual("Date", List.Columns[0].Name, "Columns[0].Name");
      Assert.AreEqual("Url", List.Columns[1].Name, "Columns[1].Name");
      Assert.AreEqual("URL", List.Columns[1].DisplayName, "Columns[1].DisplayName");
    }

    [Test]
    public void Edit() {
      Session.BeginUpdate();
      Data.AddNewslettersPersisted(1, Session);
      Session.Commit();
      var date = DateTime.Parse("2015/04/03");
      const string url = "http://www.homegrownjuice.co.nz/thank-you.html";
      List.Populate();
      var bindingList = List.TypedBindingList;
      List.OnRowEnter(0);
      bindingList[0].Date = date;
      bindingList[0].Url = url;
      var location = List[0];
      Assert.AreEqual(date, location.Date, "Date");
      Assert.AreEqual(url, location.Url, "Url");
    }
  }
}