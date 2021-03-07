using System;
using VelocityDb.Collection.Comparer;

namespace SoundExplorers.Data {
  public class KeyComparer : VelocityDbComparer<Key> {
    public override int Compare(Key? key1, Key? key2) {
      ValidateKeys(key1, key2);
      if (key1 < key2) {
        return -1;
      }
      return key1 == key2 ? 0 : 1;
    }

    private static void ValidateKeys(Key? key1, Key? key2) {
      if (key1 == null || key2 == null) {
        string key1String = key1 == null ? "null" : $"'{key1}'";
        string key2String = key2 == null ? "null" : $"'{key2}'";
        throw new InvalidOperationException(
          "A null Key argument was passed to KeyComparer.Compare: " +
          $"key1 = {key1String}; key2 = {key2String}.");
      }
    }
  }
}