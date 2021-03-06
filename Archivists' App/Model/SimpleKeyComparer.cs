using System.Collections.Generic;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class SimpleKeyComparer : Comparer<string> {
    public override int Compare(string? simpleKey1, string? simpleKey2) {
      return Key.CompareSimpleKeys(simpleKey1, simpleKey2);
    }
  }
}