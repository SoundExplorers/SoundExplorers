using System.Data;
using JetBrains.Annotations;

namespace SoundExplorers.Data {
  /// <summary>
  ///   Substitute for .Net Framework class that is not supported in .Net 5.
  /// </summary>
  public class PropertyConstraintException : ConstraintException {
    public PropertyConstraintException([NotNull] string message,
      [NotNull] string propertyName) : base(message) {
      PropertyName = propertyName;
    }

    [NotNull] public string PropertyName { get; }
  }
}