using System.Collections;

namespace SoundExplorers.Common {
  public class IdentifyingParentChildren {
    public IdentifyingParentChildren(object identifyingParent, IList children) {
      IdentifyingParent = identifyingParent;
      Children = children;
    }
    public IList Children { get; }
    public object IdentifyingParent { get; }
  }
}