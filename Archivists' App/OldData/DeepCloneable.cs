using System;

namespace SoundExplorers.OldData {
  internal class DeepCloneable<T> : ICloneable {
    object ICloneable.Clone() {
      return Clone();
    }

    private T Clone() {
      return (T)MemberwiseClone();
    }
  }
}