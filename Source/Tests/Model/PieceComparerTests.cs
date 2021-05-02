using NUnit.Framework;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests.Model {
  [TestFixture]
  public class PieceComparerTests : TestFixtureBase {
    [SetUp]
    public override void Setup() {
      base.Setup();
      PieceComparer = new PieceComparer();
      Session.BeginUpdate();
      Data.AddEventTypesPersisted(1, Session);
      Data.AddLocationsPersisted(1, Session);
      Data.AddNewslettersPersisted(1, Session);
      Data.AddSeriesPersisted(1, Session);
      Data.AddEventsPersisted(1, Session);
      Data.AddActsPersisted(1, Session);
      Data.AddGenresPersisted(1, Session);
      Session.Commit();
    }

    private PieceComparer PieceComparer { get; set; } = null!;

    [Test]
    public void DifferentSets() {
      Session.BeginUpdate();
      Data.AddSetsPersisted(2, Session);
      Data.AddPiecesPersisted(1, Session, Data.Sets[0]);
      Data.AddPiecesPersisted(1, Session, Data.Sets[1]);
      Session.Commit();
      var piece1 = Data.Pieces[0];
      var piece2 = Data.Pieces[1];
      Assert.AreEqual(-1, PieceComparer.Compare(piece1, piece2), "piece1, piece2");
      Assert.AreEqual(1, PieceComparer.Compare(piece2, piece1), "piece2, piece1");
    }

    [Test]
    public void SameSetDifferentPieceNos() {
      Session.BeginUpdate();
      Data.AddSetsPersisted(1, Session);
      Data.AddPiecesPersisted(2, Session);
      Session.Commit();
      var piece1 = Data.Pieces[0];
      var piece2 = Data.Pieces[1];
      Assert.AreEqual(-1, PieceComparer.Compare(piece1, piece2), "piece1, piece2");
      Assert.AreEqual(1, PieceComparer.Compare(piece2, piece1), "piece2, piece1");
    }
  }
}