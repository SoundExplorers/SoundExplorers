using System.Collections;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class IdentifyingParentChildren {
    internal IdentifyingParentChildren(IEntity identifyingParent, IList children) {
      IdentifyingParent = identifyingParent;
      Children = children;
    }

    internal IList Children { get; }
    internal IEntity IdentifyingParent { get; }
  }
}