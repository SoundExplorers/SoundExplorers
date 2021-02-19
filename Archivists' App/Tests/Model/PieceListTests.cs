using System;
using System.Data;
using System.Linq;
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
    private PieceList List { get; set; } = null!;
    private QueryHelper QueryHelper { get; set; } = null!;
    private TestSession Session { get; set; } = null!;

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
      var set = Data.Sets[0];
      var setChildren = CreateSetChildren(set);
      List.Populate(setChildren);
      var bindingList = List.BindingList;
      bindingList.AddNew();
      List.OnRowEnter(3);
      bindingList[3].Duration = TimeSpan.FromMinutes(3);
      List.OnRowValidated(3);
      Assert.AreEqual(4, set.Pieces.Count, "Count");
      setChildren = CreateSetChildren(set);
      List.Populate(setChildren);
      Assert.AreEqual(4, bindingList[3].PieceNo, "PieceNo in binding list");
      Assert.AreEqual(4, List[3].PieceNo, "PieceNo in List");
      Assert.AreEqual(4, set.Pieces[3].PieceNo, "PieceNo in Event.Pieces");
    }

    [Test]
    public void DefaultPieceNoWithExistingPieces() {
      List = CreatePieceList();
      List.Populate();
      List.BindingList.AddNew();
      Assert.AreEqual(4, List.BindingList[3].PieceNo);
    }

    [Test]
    public void DefaultPieceNoWithNoExistingPieces() {
      List = CreatePieceList(false);
      List.Populate();
      List.BindingList.AddNew();
      Assert.AreEqual(1, List.BindingList[0].PieceNo);
    }

    [Test]
    public void DisallowAddPieceWithoutDuration() {
      List = CreatePieceList(false);
      var set = Data.Sets[0];
      var setChildren = CreateSetChildren(set);
      List.Populate(setChildren);
      var bindingList = List.BindingList;
      bindingList.AddNew();
      List.OnRowEnter(0);
      bindingList[0].Title = "This is my song";
      var exception = Assert.Catch<DatabaseUpdateErrorException>(
        () => List.OnRowValidated(0),
        "Adding Piece without Duration disallowed.");
      Assert.AreEqual(
        $"Piece '{bindingList[0].CreateKey()}' cannot be added because its Duration has not been specified.",
        exception.Message, "Error message");
    }

    [Test]
    public void DisallowDuplicateKey() {
      List = CreatePieceList();
      var setChildren = CreateSetChildren(Data.Sets[0]);
      List.Populate(setChildren);
      List.OnRowEnter(2);
      var bindingList = List.BindingList;
      Assert.DoesNotThrow(() => bindingList[2].PieceNo = 3);
      Exception exception = Assert.Catch<DuplicateNameException>(
        () => bindingList[2].PieceNo = 1,
        "Changing PieceNo to duplicate for Set disallowed");
      Assert.AreEqual(
        $"Another Piece with key '{bindingList[2].CreateKey()}' already exists.",
        exception.Message,
        "Error message on trying to change PieceNo to duplicate for Set");
      bindingList.AddNew();
      List.OnRowEnter(3);
      bindingList[3].PieceNo = 2;
      exception = Assert.Catch<DatabaseUpdateErrorException>(() => List.OnRowValidated(3),
        "Adding Piece with PieceNo duplicate for Set disallowed");
      Assert.AreEqual(
        $"Another Piece with key '{bindingList[3].CreateKey()}' already exists.",
        exception.Message,
        "Error message on trying to add Piece with duplicate PieceNo for Set");
    }

    [Test]
    public void GetIdentifyingParentChildrenForMainList() {
      List = CreatePieceList();
      Session.BeginUpdate();
      Data.AddCreditsPersisted(5, Session);
      Session.Commit();
      List.Populate();
      var identifyingParentChildren = List.GetIdentifyingParentChildrenForMainList(0);
      Assert.AreSame(Data.Pieces[0], identifyingParentChildren.IdentifyingParent,
        "IdentifyingParent");
      Assert.AreEqual(5, identifyingParentChildren.Children.Count, "Count");
      Assert.IsInstanceOf<Credit>(identifyingParentChildren.Children[0], "Child type");
    }

    [Test]
    public void InvalidFormatPieceNo() {
      List = CreatePieceList();
      List.Populate();
      List.OnValidationError(1, "PieceNo",
        new FormatException("Value cannot be cast to Int32"));
      Assert.AreEqual("PieceNo must be an integer between 1 and 99.",
        List.LastDatabaseUpdateErrorException!.Message, "Error message");
    }

    [Test]
    public void ReadAsParentList() {
      var newDuration = TimeSpan.FromHours(1);
      List = CreatePieceList(true, false);
      Assert.IsFalse(List.IsChildList, "IsChildList");
      Session.BeginUpdate();
      var piece = Data.Pieces[2];
      piece.Duration = newDuration;
      Session.Commit();
      List.Populate();
      var bindingList = List.BindingList;
      Assert.AreEqual(piece.Set.Event.Date, bindingList[2].Date, "Date");
      Assert.AreEqual(piece.Set.Event.Location.Name, bindingList[2].Location, 
        "Location");
      Assert.AreEqual(piece.Set.SetNo, bindingList[2].SetNo, "SetNo");
      Assert.AreEqual(piece.PieceNo, bindingList[2].PieceNo, "PieceNo");
      Assert.AreEqual(piece.Title, bindingList[2].Title, "Title");
      Assert.AreEqual(newDuration, bindingList[2].Duration, "Duration");
      Assert.AreEqual(piece.AudioUrl, bindingList[2].AudioUrl, "AudioUrl");
      Assert.AreEqual(piece.VideoUrl, bindingList[2].VideoUrl, "VideoUrl");
      Assert.AreEqual(piece.Notes, bindingList[2].Notes, "Notes");
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
      Data.AddSetsPersisted(1, Session);
      Data.AddArtistsPersisted(1, Session);
      Data.AddRolesPersisted(1, Session);
      if (includingPieces) {
        Data.AddPiecesPersisted(3, Session);
      }
      Session.Commit();
    }

    private static IdentifyingParentChildren CreateSetChildren(Set set) {
      return new IdentifyingParentChildren(set, set.Pieces.Values.ToList());
    }

    private PieceList CreatePieceList(
      bool addPieces = true, bool isChildList = true) {
      AddData(addPieces);
      return new PieceList(isChildList) {Session = Session};
    }
  }
}