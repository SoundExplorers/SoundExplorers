using System;

namespace SoundExplorers.Data {
  internal class DeepCloneable<T> : ICloneable {
    object ICloneable.Clone() {
      return Clone();
    }

    private T Clone() {
      return (T)MemberwiseClone();
    }
  }
}