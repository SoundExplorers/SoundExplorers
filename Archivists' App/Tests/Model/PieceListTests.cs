using System;
using System.Data;
using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests.Model {
  [TestFixture]
  public class PieceListTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      Data = new TestData(QueryHelper);
      Session = new TestSession();
    }

    [TearDown]
    public void TearDown() {
      Session.DeleteDatabaseFolderIfExists();
    }

    private TestData Data { get; set; } = null!;
    private QueryHelper QueryHelper { get; set; } = null!;
    private TestSession Session { get; set; } = null!;
    private PieceList List { get; set; } = null!;
    private SetList ParentList { get; set; } = null!;

    [Test]
    public void A010_InitialiseAsChildList() {
      List = CreatePieceList(false);
      Assert.AreEqual("Piece", List.EntityTypeName, "EntityName");
      Assert.AreEqual(typeof(SetList), List.ParentListType, "ParentListType");
      Assert.AreEqual(9, List.Columns.Count, "Columns.Count");
      Assert.AreEqual("Date", List.Columns[0].PropertyName, "Columns[0].PropertyName");
      Assert.IsFalse(List.Columns[0].IsVisible, "Columns[0].IsVisible");
      Assert.AreEqual("Location", List.Columns[1].PropertyName,
        "Columns[1].PropertyName");
      Assert.IsFalse(List.Columns[1].IsVisible, "Columns[1].IsVisible");
      Assert.AreEqual("SetNo", List.Columns[2].PropertyName, "Columns[2].PropertyName");
      Assert.IsFalse(List.Columns[2].IsVisible, "Columns[2].IsVisible");
      Assert.AreEqual("PieceNo", List.Columns[3].PropertyName, "Columns[3].PropertyName");
      Assert.AreEqual("Title", List.Columns[4].PropertyName, "Columns[4].PropertyName");
      Assert.AreEqual("Duration", List.Columns[5].PropertyName, "Columns[5].PropertyName");
      Assert.AreEqual("Audio URL", List.Columns[6].DisplayName, "Columns[6].DisplayName");
      Assert.AreEqual("AudioUrl", List.Columns[6].PropertyName,
        "Columns[6].PropertyName");
      Assert.AreEqual("Video URL", List.Columns[7].DisplayName, "Columns[8].DisplayName");
      Assert.AreEqual("VideoUrl", List.Columns[7].PropertyName,
        "Columns[7].PropertyName");
      Assert.AreEqual("Notes", List.Columns[8].PropertyName, "Columns[8].PropertyName");
    }

    [Test]
    public void AddPiece() {
      List = CreatePieceList();
      Assert.IsTrue(List.IsChildList, "IsChildList");
      Populate();
      var set = Data.Sets[0];
      Populate();
      var bindingList = List.BindingList;
      bindingList.AddNew();
      List.OnRowEnter(3);
      bindingList[3].Duration = "3:00";
      List.OnRowValidated(3);
      Assert.AreEqual(4, set.Pieces.Count, "Count");
      Populate();
      Assert.AreEqual("4", bindingList[3].PieceNo, "PieceNo in binding list");
      Assert.AreEqual(4, List[3].PieceNo, "PieceNo in List");
      Assert.AreEqual(4, set.Pieces[3].PieceNo, "PieceNo in Event.Pieces");
    }

    [Test]
    public void ChangePieceNo() {
      List = CreatePieceList();
      Populate();
      List.OnRowEnter(2);
      Assert.AreEqual("3", List.BindingList[2].PieceNo, "Binding PieceNo after populate");
      List.BindingList[2].PieceNo = "9";
      Assert.AreEqual(9, Data.Pieces[2].PieceNo, "List PieceNo after change");
    }

    [Test]
    public void DefaultPieceNoWithExistingPieces() {
      List = CreatePieceList();
      Populate();
      List.BindingList.AddNew();
      Assert.AreEqual("4", List.BindingList[3].PieceNo);
    }

    [Test]
    public void DefaultPieceNoWithNoExistingPieces() {
      List = CreatePieceList(false);
      Populate();
      List.BindingList.AddNew();
      Assert.AreEqual("1", List.BindingList[0].PieceNo);
    }

    [Test]
    public void DisallowAddPieceWithInvalidDuration() {
      List = CreatePieceList(false);
      Populate();
      var bindingList = List.BindingList;
      bindingList.AddNew();
      List.OnRowEnter(0);
      bindingList[0].Duration = "abc";
      var exception = Assert.Catch<DatabaseUpdateErrorException>(
        () => List.OnRowValidated(0),
        "Adding Piece with invalid Duration disallowed.");
      Assert.IsTrue(
        exception.Message.StartsWith(
          "Duration must be between 1 second and 9 hours, 59 minutes, 59 seconds." + 
          "\r\nAccepted formats\r\n"), 
        "Message");
    }

    [Test]
    public void DisallowAddPieceWithoutDuration() {
      List = CreatePieceList(false);
      Populate();
      var bindingList = List.BindingList;
      bindingList.AddNew();
      List.OnRowEnter(0);
      bindingList[0].Title = "This is my song";
      var exception = Assert.Catch<DatabaseUpdateErrorException>(
        () => List.OnRowValidated(0),
        "Adding Piece without Duration disallowed.");
      Assert.AreEqual(
        $"Piece '{bindingList[0].Key}' cannot be added because its Duration has not been specified.",
        exception.Message, "Error message");
    }

    [Test]
    public void DisallowChangeDurationToInvalid() {
      List = CreatePieceList();
      Populate();
      List.OnRowEnter(2);
      var bindingList = List.BindingList;
      Assert.DoesNotThrow(() => bindingList[2].Duration = "10:23");
      var exception = Assert.Catch<DatabaseUpdateErrorException>(
        () => bindingList[2].Duration = "abc",
        "Changing Duration to invalid value disallowed");
      Assert.IsTrue(
        exception.Message.StartsWith(
          "Duration must be between 1 second and 9 hours, 59 minutes, 59 seconds." + 
          "\r\nAccepted formats\r\n"), 
        "Message");
    }

    [Test]
    public void DisallowChangeKeyToDuplicate() {
      List = CreatePieceList();
      Populate();
      List.OnRowEnter(2);
      var bindingList = List.BindingList;
      Assert.DoesNotThrow(() => bindingList[2].PieceNo = "3");
      Exception exception = Assert.Catch<DuplicateNameException>(
        () => bindingList[2].PieceNo = "1",
        "Changing PieceNo to duplicate for Set disallowed");
      Assert.AreEqual(
        $"Another Piece with key '{bindingList[2].Key}' already exists.",
        exception.Message,
        "Error message on trying to change PieceNo to duplicate for Set");
      bindingList.AddNew();
      List.OnRowEnter(3);
      bindingList[3].PieceNo = "2";
      exception = Assert.Catch<DatabaseUpdateErrorException>(() => List.OnRowValidated(3),
        "Adding Piece with PieceNo duplicate for Set disallowed");
      Assert.AreEqual(
        $"Another Piece with key '{bindingList[3].Key}' already exists.",
        exception.Message,
        "Error message on trying to add Piece with duplicate PieceNo for Set");
    }

    [Test]
    public void DisallowChangePieceNoToInvalidFormat() {
      List = CreatePieceList();
      Populate();
      List.OnRowEnter(2);
      var bindingList = List.BindingList;
      Exception exception = Assert.Catch<DatabaseUpdateErrorException>(
        () => bindingList[2].PieceNo = "ABC",
        "Changing PieceNo to invalid format disallowed");
      Assert.AreEqual("PieceNo must be an integer between 1 and 99.",
        exception.Message,
        "Error message on trying to change PieceNo to invalid format");
    }

    [Test]
    public void GetIdentifyingParentChildrenForMainList() {
      List = CreatePieceList();
      Session.BeginUpdate();
      Data.AddCreditsPersisted(5, Session);
      Session.Commit();
      Populate();
      var identifyingParentChildren = List.GetIdentifyingParentChildrenForMainList(0);
      Assert.AreSame(Data.Pieces[0], identifyingParentChildren.IdentifyingParent,
        "IdentifyingParent");
      Assert.AreEqual(5, identifyingParentChildren.Children.Count, "Count");
      Assert.IsInstanceOf<Credit>(identifyingParentChildren.Children[0], "Child type");
    }

    [Test]
    public void ReadAsChildList() {
      var newDuration2 = TimeSpan.FromMinutes(59);
      var newDuration3 = TimeSpan.FromHours(1);
      List = CreatePieceList(true, false);
      Assert.IsFalse(List.IsChildList, "IsChildList");
      Session.BeginUpdate();
      var piece2 = Data.Pieces[1];
      piece2.Duration = newDuration2;
      var piece3 = Data.Pieces[2];
      piece3.Duration = newDuration3;
      Session.Commit();
      Populate();
      var bindingList = List.BindingList;
      Assert.AreEqual("59:00", bindingList[1].Duration, "Duration string 2");
      Assert.AreEqual(newDuration2,
        PieceBindingItem.ToDurationTimeSpan(bindingList[1].Duration), 
        "Duration TimeSpan 2");
      Assert.AreEqual(piece3.Set.Event.Date, bindingList[2].Date, "Date");
      Assert.AreEqual(piece3.Set.Event.Location.Name, bindingList[2].Location, 
        "Location");
      Assert.AreEqual(piece3.Set.SetNo, int.Parse(bindingList[2].SetNo), "SetNo");
      Assert.AreEqual(piece3.PieceNo, int.Parse(bindingList[2].PieceNo), "PieceNo");
      Assert.AreEqual(piece3.Title, bindingList[2].Title, "Title");
      Assert.AreEqual("1:00:00", bindingList[2].Duration, "Duration string 3");
      Assert.AreEqual(newDuration3,
        PieceBindingItem.ToDurationTimeSpan(bindingList[2].Duration), 
        "Duration TimeSpan 3");
      Assert.AreEqual(piece3.AudioUrl, bindingList[2].AudioUrl, "AudioUrl");
      Assert.AreEqual(piece3.VideoUrl, bindingList[2].VideoUrl, "VideoUrl");
      Assert.AreEqual(piece3.Notes, bindingList[2].Notes, "Notes");
    }

    [Test]
    public void ValidateAudioUrl() {
      List = CreatePieceList();
      Populate();
      List.OnRowEnter(2);
      var bindingList = List.BindingList;
      string uniqueUrl = TestData.GenerateUniqueUrl();
      Key otherKey = bindingList[0].Key;
      string otherUrl = bindingList[0].AudioUrl = TestData.GenerateUniqueUrl();
      Assert.DoesNotThrow(
        () => bindingList[2].AudioUrl = uniqueUrl,
        "Changing AudioUrl to unique allowed");
      var exception = Assert.Catch<DatabaseUpdateErrorException>(
        () => bindingList[2].AudioUrl = otherUrl,
        "Changing AudioUrl to duplicate disallowed");
      Assert.AreEqual(
        $"Audio URL cannot be set to '{otherUrl}'. " + 
        $"Piece '{otherKey}' already exists with that Audio URL.",
        exception.Message,
        "Error message on trying to change AudioUrl to duplicate");
      bindingList.AddNew();
      List.OnRowEnter(3);
      bindingList[3].EntityList = List;
      Key newKey = bindingList[3].Key;
      bindingList[3].Duration = "3:45";
      bindingList[3].AudioUrl = otherUrl;
      exception = Assert.Catch<DatabaseUpdateErrorException>(() => List.OnRowValidated(3),
        "Adding Piece with duplicate AudioUrl disallowed");
      Assert.AreEqual(
        $"Piece '{newKey}' cannot be added because Piece '{otherKey}' already exists " +
        $"with the same Audio URL '{otherUrl}'.",
        exception.Message,
        "Error message on trying to add Piece with duplicate AudioUrl");
      uniqueUrl = TestData.GenerateUniqueUrl();
      bindingList[3].AudioUrl = uniqueUrl;
      Assert.DoesNotThrow(
        () => List.OnRowValidated(3),
        "Adding Piece with unique AudioUrl allowed");
    }

    [Test]
    public void ValidateVideoUrl() {
      List = CreatePieceList();
      Populate();
      List.OnRowEnter(2);
      var bindingList = List.BindingList;
      string uniqueUrl = TestData.GenerateUniqueUrl();
      Key otherKey = bindingList[0].Key;
      string otherUrl = bindingList[0].VideoUrl = TestData.GenerateUniqueUrl();
      Assert.DoesNotThrow(
        () => bindingList[2].VideoUrl = uniqueUrl,
        "Changing VideoUrl to unique allowed");
      var exception = Assert.Catch<DatabaseUpdateErrorException>(
        () => bindingList[2].VideoUrl = otherUrl,
        "Changing VideoUrl to duplicate disallowed");
      Assert.AreEqual(
        $"Video URL cannot be set to '{otherUrl}'. " + 
        $"Piece '{otherKey}' already exists with that Video URL.",
        exception.Message,
        "Error message on trying to change VideoUrl to duplicate");
      bindingList.AddNew();
      List.OnRowEnter(3);
      bindingList[3].EntityList = List;
      Key newKey = bindingList[3].Key;
      bindingList[3].Duration = "3:45";
      bindingList[3].VideoUrl = otherUrl;
      exception = Assert.Catch<DatabaseUpdateErrorException>(() => List.OnRowValidated(3),
        "Adding Piece with duplicate VideoUrl disallowed");
      Assert.AreEqual(
        $"Piece '{newKey}' cannot be added because Piece '{otherKey}' already exists " +
        $"with the same Video URL '{otherUrl}'.",
        exception.Message,
        "Error message on trying to add Piece with duplicate VideoUrl");
      uniqueUrl = TestData.GenerateUniqueUrl();
      bindingList[3].VideoUrl = uniqueUrl;
      Assert.DoesNotThrow(
        () => List.OnRowValidated(3),
        "Adding Piece with unique VideoUrl allowed");
    }
 
    private void AddData(bool includingPieces = true) {
      Session.BeginUpdate();
      Data.AddEventTypesPersisted(1, Session);
      Data.AddLocationsPersisted(1, Session);
      Data.AddNewslettersPersisted(1, Session);
      Data.AddSeriesPersisted(1, Session);
      Data.AddEventsPersisted(1, Session);
      Data.AddActsPersisted(1, Session);
      Data.AddGenresPersisted(1, Session);
      Data.AddSetsPersisted(1, Session, Data.Events[0]);
      Data.AddArtistsPersisted(1, Session);
      Data.AddRolesPersisted(1, Session);
      if (includingPieces) {
        Data.AddPiecesPersisted(3, Session, Data.Sets[0]);
      }
      Session.Commit();
    }

    private PieceList CreatePieceList(
      bool addPieces = true, bool isMainList = true) {
      AddData(addPieces);
      var result = new PieceList(isMainList) {Session = Session};
      if (isMainList) {
        ParentList = (result.CreateParentList() as SetList)!;
      }
      return result;
    }

    private void Populate() {
      if (List.IsChildList) {
        ParentList.Populate();
        List.Populate(ParentList.GetIdentifyingParentChildrenForMainList(
          ParentList.Count - 1));
      } else {
        List.Populate();
      }
    }
  }
}