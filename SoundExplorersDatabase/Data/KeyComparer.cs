using System;
using System.Collections.Generic;

namespace SoundExplorersDatabase.Data {
  public class KeyComparer : Comparer<Key> {
    public override int Compare(Key x, Key y) {
      if (x < y) {
        return -1;
      }
      return x == y ? 0 : 1;
    }
  }
}