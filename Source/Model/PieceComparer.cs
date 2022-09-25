using SoundExplorers.Data;

namespace SoundExplorers.Model; 

public class PieceComparer : DefaultIdentifiedEntityComparer<Piece, Set> {
  static PieceComparer() {
    SetComparer = new SetComparer();
  }

  public PieceComparer() : base(SetComparer) { }
  private static SetComparer SetComparer { get; }
}