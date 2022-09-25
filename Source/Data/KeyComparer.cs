using VelocityDb.Collection.Comparer;

namespace SoundExplorers.Data; 

public class KeyComparer : VelocityDbComparer<Key> {
  public override int Compare(Key key1, Key key2) {
    if (key1 < key2) {
      return -1;
    }
    return key1 == key2 ? 0 : 1;
  }
}