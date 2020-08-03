using System;
using System.Collections;
using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  public class ChildrenType : ChildrenRelation {
    internal ChildrenType(
      [NotNull] ChildrenRelation childrenRelation,
      [NotNull] IDictionary children) : base(childrenRelation) {
      Children = children ??
                 throw new ArgumentNullException(nameof(children));
    }

    public IDictionary Children { get; }
  }
}