using System;
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
      Data.AddEventsPersisted(4, Session);
      Session.Commit();
      List = new EventList {QueryHelper = QueryHelper, Session = Session};
    }

    [TearDown]
    public void TearDown() {
      Session.DeleteDatabaseFolderIfExists();
    }

    private TestData Data { get; set; }
    private EventList List { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private TestSession Session { get; set; }

    [Test]
    public void A010_Initial() {
      Assert.AreEqual("Event", List.EntityName, "EntityName");
      Assert.IsFalse(List.IsParentList, "IsParentList");
      Assert.IsNull(List.ParentListType, "ParentListType");
      Assert.AreEqual(6, List.Columns.Count, "Columns.Count");
      Assert.AreEqual("Date", List.Columns[0].Name, "Columns[0].Name");
      Assert.AreEqual("Location", List.Columns[1].Name, "Columns[1].Name");
      Assert.AreEqual(typeof(LocationList), List.Columns[1].ReferencedEntityListType,
        "Columns[1].ReferencedEntityListType");
      Assert.AreEqual("Name", List.Columns[1].ReferencedColumnName,
        "Columns[1].ReferencedColumnName");
      Assert.AreEqual("Location", List.Columns[1].ReferencedTableName,
        "Columns[1].ReferencedTableName");
      Assert.AreEqual("Newsletter", List.Columns[2].Name, "Columns[2].Name");
      Assert.AreEqual(typeof(NewsletterList), List.Columns[2].ReferencedEntityListType,
        "Columns[2].ReferencedEntityListType");
      Assert.AreEqual("Date", List.Columns[2].ReferencedColumnName,
        "Columns[2].ReferencedColumnName");
      Assert.AreEqual("EventType", List.Columns[3].Name, "Columns[3].Name");
      Assert.AreEqual(typeof(EventTypeList), List.Columns[3].ReferencedEntityListType,
        "Columns[3].ReferencedEntityListType");
      Assert.AreEqual("Name", List.Columns[3].ReferencedColumnName,
        "Columns[3].ReferencedColumnName");
      Assert.AreEqual("Series", List.Columns[4].Name, "Columns[4].Name");
      Assert.AreEqual(typeof(SeriesList), List.Columns[4].ReferencedEntityListType,
        "Columns[4].ReferencedEntityListType");
      Assert.AreEqual("Name", List.Columns[4].ReferencedColumnName,
        "Columns[4].ReferencedColumnName");
      Assert.AreEqual("Notes", List.Columns[5].Name, "Columns[5].Name");
    }

    [Test]
    public void DefaultEventType() {
      var defaultEventType = Data.EventTypes[0];
      Assert.AreEqual(EventBindingItem.DefaultEventTypeName, defaultEventType.Name,
        "Default EventType added in Setup");
      Session.BeginUpdate();
      // First, delete the events that reference the default event type,
      // so that the default event type can then be deleted.
      for (int i = Data.Events.Count - 1; i >= 0; i--) {
        Session.Unpersist(Data.Events[i]);
      }
      Session.Unpersist(Data.EventTypes[0]);
      defaultEventType = QueryHelper.Find<EventType>(
        EventBindingItem.DefaultEventTypeName, Session);
      Session.Commit();
      Assert.IsNull(defaultEventType, "Default EventType removed");
      List.Populate(); // This should re-add the now missing default event type.
      Session.BeginRead();
      defaultEventType = QueryHelper.Find<EventType>(
        EventBindingItem.DefaultEventTypeName, Session);
      Session.Commit();
      Assert.IsNotNull(defaultEventType, "Default EventType after first populate");
      // This refresh populate does not need to add the default event type.
      List.Populate();  
      Session.BeginRead();
      defaultEventType = QueryHelper.Find<EventType>(
        EventBindingItem.DefaultEventTypeName, Session);
      Session.Commit();
      Assert.IsNotNull(defaultEventType, "Default EventType after refresh populate");
    }

    [Test]
    public void Edit() {
      Session.BeginUpdate();
      Data.AddNewslettersPersisted(1, Session);
      Data.AddSeriesPersisted(1, Session);
      Session.Commit();
      var date = DateTime.Parse("2015/04/03");
      var location = Data.Locations[1];
      var newsletter = Data.Newsletters[0];
      var eventType = Data.EventTypes[1];
      var series = Data.Series[0];
      const string notes = "My notes";
      List.Populate();
      var editor = new TestEditor<Event, EventBindingItem>(
        QueryHelper, Session, List.BindingList);
      editor[1].Date = date;
      editor[1].Location = location.Name;
      editor[1].Newsletter = newsletter.Date;
      editor[1].EventType = eventType.Name;
      editor[1].Series = series.Name;
      editor[1].Notes = notes;
      var @event = List[1];
      Assert.AreEqual(date, @event.Date, "Date");
      Assert.AreSame(location, @event.Location, "Location");
      Assert.AreSame(newsletter, @event.Newsletter, "Newsletter");
      Assert.AreSame(eventType, @event.EventType, "EventType");
      Assert.AreSame(series, @event.Series, "Series");
      Assert.AreEqual(notes, @event.Notes, "Notes");
      editor[1].Series = null;
      Assert.IsNull(@event.Series, "Series reset to null");
      editor[1].Newsletter = EntityBase.InitialDate;
      Assert.IsNull(@event.Newsletter, "Newsletter reset to null");
    }

    [Test]
    public void GetChildrenForMainList() {
      Session.BeginUpdate();
      Data.AddGenresPersisted(1, Session);
      Data.AddSetsPersisted(3, Session);
      Session.Commit();
      List.Populate();
      var children = List.GetChildrenForMainList(0);
      Assert.AreEqual(3, children?.Count, "Count");
      Assert.IsInstanceOf<Set>(children?[0], "Child type");
    }
  }
}