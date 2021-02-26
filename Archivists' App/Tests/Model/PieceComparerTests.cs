using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests.Model {
  [TestFixture]
  public class PieceComparerTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      Data = new TestData(QueryHelper);
      Session = new TestSession();
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
    
    private TestData Data { get; set; } = null!;
    private QueryHelper QueryHelper { get; set; } = null!;
    private TestSession Session { get; set; } = null!;
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