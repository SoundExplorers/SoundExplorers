using System;
using System.Data;
using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests.Model {
  [TestFixture]
  public class EventListTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      Data = new TestData(QueryHelper);
      Session = new TestSession();
      Session.BeginUpdate();
      Data.AddEventTypesPersisted(2, Session);
      Data.AddLocationsPersisted(2, Session);
      Data.AddActsPersisted(1, Session);
      Data.AddNewslettersPersisted(3, Session);
      Data.AddSeriesPersisted(2, Session);
      Data.AddEventsPersisted(4, Session);
      Session.Commit();
      List = new EventList {QueryHelper = QueryHelper, Session = Session};
    }

    [TearDown]
    public void TearDown() {
      Session.DeleteDatabaseFolderIfExists();
    }

    private TestData Data { get; set; } = null!;
    private EventList List { get; set; } = null!;
    private QueryHelper QueryHelper { get; set; } = null!;
    private TestSession Session { get; set; } = null!;

    [Test]
    public void A010_Initial() {
      Assert.AreEqual("Event", List.EntityTypeName, "EntityName");
      Assert.IsNull(List.ParentListType, "ParentListType");
      Assert.AreEqual(6, List.Columns.Count, "Columns.Count");
      Assert.AreEqual("Date", List.Columns[0].PropertyName, "Columns[0].Name");
      Assert.AreEqual("Location", List.Columns[1].PropertyName, "Columns[1].Name");
      Assert.AreEqual(typeof(LocationList), List.Columns[1].ReferencedEntityListType,
        "Columns[1].ReferencedEntityListType");
      Assert.AreEqual("Name", List.Columns[1].ReferencedPropertyName,
        "Columns[1].ReferencedColumnName");
      Assert.AreEqual("Location", List.Columns[1].ReferencedTableName,
        "Columns[1].ReferencedTableName");
      Assert.AreEqual("Newsletter", List.Columns[2].PropertyName, "Columns[2].Name");
      Assert.AreEqual(typeof(NewsletterList), List.Columns[2].ReferencedEntityListType,
        "Columns[2].ReferencedEntityListType");
      Assert.AreEqual("Date", List.Columns[2].ReferencedPropertyName,
        "Columns[2].ReferencedColumnName");
      Assert.AreEqual("EventType", List.Columns[3].PropertyName, "Columns[3].Name");
      Assert.AreEqual(typeof(EventTypeList), List.Columns[3].ReferencedEntityListType,
        "Columns[3].ReferencedEntityListType");
      Assert.AreEqual("Name", List.Columns[3].ReferencedPropertyName,
        "Columns[3].ReferencedColumnName");
      Assert.AreEqual("Series", List.Columns[4].PropertyName, "Columns[4].Name");
      Assert.AreEqual(typeof(SeriesList), List.Columns[4].ReferencedEntityListType,
        "Columns[4].ReferencedEntityListType");
      Assert.AreEqual("Name", List.Columns[4].ReferencedPropertyName,
        "Columns[4].ReferencedColumnName");
      Assert.AreEqual("Notes", List.Columns[5].PropertyName, "Columns[5].Name");
    }

    [Test]
    public void ChangeNewsletterToDefault() {
      var defaultNewsletter = Data.Newsletters[0];
      var nonDefaultNewsletter = Data.Newsletters[1];
      Session.BeginUpdate();
      Data.Events[0].Newsletter = nonDefaultNewsletter;
      Session.Commit();
      List.Populate();
      List.OnRowEnter(0);
      Assert.AreEqual(nonDefaultNewsletter.Date, List.BindingList[0].Newsletter,
        "Binding Newsletter after populate");
      List.BindingList[0].Newsletter = EntityBase.DefaultDate;
      Assert.AreSame(defaultNewsletter, Data.Events[0].Newsletter,
        "List Newsletter after change to default");
    }

    [Test]
    public void ChangeSeriesToDefault() {
      var defaultSeries = Data.Series[0];
      var nonDefaultSeries = Data.Series[1];
      Session.BeginUpdate();
      Data.Events[0].Series = nonDefaultSeries;
      Session.Commit();
      List.Populate();
      List.OnRowEnter(0);
      Assert.AreEqual(nonDefaultSeries.Name, List.BindingList[0].Series,
        "Binding Series after populate");
      List.BindingList[0].Series = "    ";
      Assert.AreSame(defaultSeries, Data.Events[0].Series,
        "List Series after change to default");
    }

    [Test]
    public void DefaultEventType() {
      var defaultEventType = Data.EventTypes[0];
      Assert.AreEqual(Event.DefaultEventTypeName, defaultEventType.Name,
        "Default EventType added in Setup");
      Session.BeginUpdate();
      // First, delete the events that reference the default event type,
      // so that the default event type can then be deleted.
      for (int i = Data.Events.Count - 1; i >= 0; i--) {
        Session.Unpersist(Data.Events[i]);
      }
      Session.Unpersist(Data.EventTypes[0]);
      defaultEventType = QueryHelper.Find<EventType>(
        Event.DefaultEventTypeName, Session);
      Session.Commit();
      Assert.IsNull(defaultEventType, "Default EventType removed");
      List.Populate(); // This should re-add the now missing default event type.
      Session.BeginRead();
      defaultEventType = QueryHelper.Find<EventType>(
        Event.DefaultEventTypeName, Session);
      Session.Commit();
      Assert.IsNotNull(defaultEventType, "Default EventType after populate");
    }

    [Test]
    public void DisallowDuplicateKey() {
      List.Populate();
      List.OnRowEnter(2);
      var bindingList = List.BindingList;
      Exception exception = Assert.Catch<DuplicateNameException>(
        () => bindingList[2].Date = Data.Events[0].Date,
        "Changing Date to duplicate for Location disallowed");
      Assert.AreEqual("Another Event with key '2020/01/09 | Athens' already exists.",
        exception.Message,
        "Error message on trying to change Date to duplicate for Location");
      bindingList.AddNew();
      List.OnRowEnter(4);
      bindingList[4].Date = Data.Events[1].Date;
      bindingList[4].Location = Data.Events[1].Location.Name;
      exception = Assert.Catch<DatabaseUpdateErrorException>(() => List.OnRowValidated(4),
        "Adding Event with duplicate key disallowed");
      Assert.AreEqual("Another Event with key '2020/01/16 | Athens' already exists.",
        exception.Message,
        "Error message on trying to add Event with duplicate key");
    }

    [Test]
    public void Edit() {
      var date = DateTime.Parse("2015/04/03");
      var defaultNewsletter = Data.Newsletters[0];
      var defaultSeries = Data.Series[0];
      var location = Data.Locations[1];
      var newsletter = Data.Newsletters[2];
      var eventType = Data.EventTypes[1];
      var series = Data.Series[1];
      const string notes = "My notes";
      List.Populate();
      var bindingList = List.BindingList;
      List.OnRowEnter(1);
      bindingList[1].Date = date;
      bindingList[1].Location = location.Name!;
      bindingList[1].Newsletter = newsletter.Date;
      bindingList[1].EventType = eventType.Name!;
      bindingList[1].Series = series.Name;
      bindingList[1].Notes = notes;
      var @event = List[1];
      Assert.AreEqual(date, @event.Date, "Date");
      Assert.AreSame(location, @event.Location, "Location");
      Assert.AreSame(newsletter, @event.Newsletter, "Newsletter");
      Assert.AreSame(eventType, @event.EventType, "EventType");
      Assert.AreSame(series, @event.Series, "Series");
      Assert.AreEqual(notes, @event.Notes, "Notes");
      bindingList[1].Series = null;
      Assert.AreSame(defaultSeries, @event.Series, "Series reset to default");
      bindingList[1].Newsletter = EntityBase.DefaultDate;
      Assert.AreSame(defaultNewsletter, @event.Newsletter,
        "Newsletter reset to default");
    }

    [Test]
    public void GetIdentifyingParentChildrenForMainList() {
      Session.BeginUpdate();
      Data.AddGenresPersisted(1, Session);
      Data.AddSetsPersisted(3, Session);
      Session.Commit();
      List.Populate();
      var identifyingParentChildren = List.GetIdentifyingParentChildrenForMainList(0);
      Assert.AreSame(Data.Events[0], identifyingParentChildren.IdentifyingParent,
        "IdentifyingParent");
      Assert.AreEqual(3, identifyingParentChildren.Children.Count, "Count");
      Assert.IsInstanceOf<Set>(identifyingParentChildren.Children[0], "Child type");
    }
  }
}