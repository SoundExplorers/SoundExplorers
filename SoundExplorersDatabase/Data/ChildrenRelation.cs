using System;
using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  public class ChildrenRelation {
    internal ChildrenRelation(
      [NotNull] Type childType,
      bool isMembershipMandatory) {
      ChildType = childType ??
                  throw new ArgumentNullException(nameof(childType));
      IsMembershipMandatory = isMembershipMandatory;
    }

    internal ChildrenRelation(
      [NotNull] ChildrenRelation sourceChildrenRelation) {
      var source = sourceChildrenRelation ??
                   throw new ArgumentNullException(
                     nameof(sourceChildrenRelation));
      ChildType = source.ChildType;
      IsMembershipMandatory = source.IsMembershipMandatory;
    }

    [NotNull] internal Type ChildType { get; }
    public bool IsMembershipMandatory { get; }
  }
}