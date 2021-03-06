using System.Collections.Generic;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class PieceComparer : Comparer<Piece> {
    public override int Compare(Piece? piece1, Piece? piece2) {
      var set1 = (piece1!.IdentifyingParent as Set)!;
      var set2 = (piece2!.IdentifyingParent as Set)!;
      // Compare Sets first.
      var setComparer = new SetComparer();
      int setComparison = setComparer.Compare(set1, set2);
      return setComparison != 0
        ? setComparison
        // Same Set. Compare PieceNos. 
        : Key.CompareSimpleKeys(piece1.SimpleKey, piece2.SimpleKey);
    }
  }
}