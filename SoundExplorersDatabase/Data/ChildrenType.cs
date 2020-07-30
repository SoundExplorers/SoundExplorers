using System;
using System.Collections;
using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  public class ChildrenType {
    public ChildrenType([NotNull] Type childType, [NotNull] IDictionary children) {
      ChildType = childType;
      Children = children;
    }

    public Type ChildType { get; }
    public IDictionary Children { get; }
  }
}