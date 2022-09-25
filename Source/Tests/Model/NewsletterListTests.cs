using System;
using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests.Model; 

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
    Assert.AreEqual("Date", List.Columns[0].PropertyName, "Columns[0].Name");
    Assert.AreEqual("Url", List.Columns[1].PropertyName, "Columns[1].Name");
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
    var bindingList = List.BindingList;
    List.OnRowEnter(0);
    bindingList[0].Date = date;
    bindingList[0].Url = url;
    var location = List[0];
    Assert.AreEqual(date, location.Date, "Date");
    Assert.AreEqual(url, location.Url, "Url");
  }

  [Test]
  public void ValidateUrl() {
    Session.BeginUpdate();
    Data.AddNewslettersPersisted(3, Session);
    Session.Commit();
    List.Populate();
    List.OnRowEnter(2);
    var bindingList = List.BindingList;
    Key otherKey = bindingList[0].Key;
    string otherUrl = bindingList[0].Url = TestData.GenerateUniqueUrl();
    string uniqueUrl = TestData.GenerateUniqueUrl();
    Assert.DoesNotThrow(
      () => bindingList[2].Url = uniqueUrl,
      "Changing Url to unique allowed");
    var exception = Assert.Catch<DatabaseUpdateErrorException>(
      () => bindingList[2].Url = otherUrl,
      "Changing Url to duplicate disallowed");
    Assert.AreEqual(
      $"URL cannot be set to '{otherUrl}'. " +
      $"Newsletter {otherKey} already exists with that URL.",
      exception!.Message,
      "Error message on trying to change Url to duplicate");
    bindingList.AddNew();
    List.OnRowEnter(3);
    bindingList[3].EntityList = List;
    Key newKey = bindingList[3].Key;
    bindingList[3].Url = otherUrl;
    exception = Assert.Catch<DatabaseUpdateErrorException>(() => List.OnRowValidated(3),
      "Adding Newsletter with duplicate Url disallowed");
    Assert.AreEqual(
      $"Newsletter {newKey} cannot be added because Newsletter {otherKey} " +
      $"already exists with the same URL '{otherUrl}'.",
      exception!.Message,
      "Error message on trying to add Newsletter with duplicate Url");
    uniqueUrl = TestData.GenerateUniqueUrl();
    bindingList[3].Url = uniqueUrl;
    Assert.DoesNotThrow(
      () => List.OnRowValidated(3),
      "Adding Newsletter with unique Url allowed");
  }
}