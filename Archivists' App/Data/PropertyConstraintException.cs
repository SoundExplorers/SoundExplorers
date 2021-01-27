using System.Data;

namespace SoundExplorers.Data {
  /// <summary>
  ///   Substitute for a .Net Framework class that is not supported in .Net 5.
  /// </summary>
  public class PropertyConstraintException : ConstraintException {
    public PropertyConstraintException(string message,
      string propertyName) : base(message) {
      PropertyName = propertyName;
    }

    public string PropertyName { get; }
  }
}