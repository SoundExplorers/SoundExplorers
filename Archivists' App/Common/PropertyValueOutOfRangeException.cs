using System.Data;

namespace SoundExplorers.Common {
  public class PropertyValueOutOfRangeException : ConstraintException {
    public PropertyValueOutOfRangeException(string message,
      string propertyName) : base(message) {
      PropertyName = propertyName;
    }

    public string PropertyName { get; }
  }
}