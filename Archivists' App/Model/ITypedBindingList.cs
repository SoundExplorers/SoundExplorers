using System.ComponentModel;

namespace SoundExplorers.Model {
  public interface ITypedBindingList : IBindingList {
    /// <summary>
    ///   Used for restoring error values to the new row for correction or edit
    ///   cancellation after an insertion error message hase been shown.
    /// </summary>
    IBindingItem? InsertionErrorItem { get; set; }
  }
}