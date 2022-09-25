using System.Data;

namespace SoundExplorers.Data; 

/// <summary>
///   Substitute for a .Net Framework class that is not supported in .Net 5/Core.
/// </summary>
/// <remarks>
///   This class is presumably implemented in the Windows Compatibility Pack, which can
///   be referenced in .Net 5/Core. But, for forward compatibility with possible future
///   implementations of the application on other operating systems (which would
///   require a user interface other than Windows Forms, I would rather avoid
///   referencing the Windows Compatibility Pack.
/// </remarks>
public class PropertyConstraintException : ConstraintException {
  public PropertyConstraintException(string message,
    string propertyName) : base(message) {
    PropertyName = propertyName;
  }

  public string PropertyName { get; }
}