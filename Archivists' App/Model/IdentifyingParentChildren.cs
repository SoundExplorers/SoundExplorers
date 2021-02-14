using System.Collections;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class IdentifyingParentChildren {
    /// <summary>
    ///   For a main grid that is a child of a parent grid row, this specifies the grid's
    ///   identifying parent entity and, for populating the grid, its child entities.
    /// </summary>
    internal IdentifyingParentChildren(IEntity identifyingParent, IList children) {
      IdentifyingParent = identifyingParent;
      Children = children;
    }

    internal IList Children { get; }
    internal IEntity IdentifyingParent { get; }
  }
}