using System;
using System.Collections;
using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  public class ChildrenType<TChild> : IChildrenType where TChild : RelativeBase {
    public ChildrenType([NotNull] IDictionary children) {
      ChildType = typeof(TChild);
      Children = children;
    }

    public Type ChildType { get; }
    public IDictionary Children { get; }
  }
}