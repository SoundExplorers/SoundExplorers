using System.Data;
using JetBrains.Annotations;

namespace SoundExplorers.Data {
  public class PropertyConstraintException : ConstraintException {
    public PropertyConstraintException([NotNull] string message,
      [NotNull] string propertyName) : base(message) {
      PropertyName = propertyName;
    }

    [NotNull] public string PropertyName { get; }
  }
}