using System;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data {
  public class TestPiece : Piece {
    public TestPiece() {
      Duration = new TimeSpan(0, 12, 34);
    }
  }
}