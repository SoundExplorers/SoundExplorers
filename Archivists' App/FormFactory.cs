using System.Windows.Forms;
using SoundExplorers.Controller;

namespace SoundExplorers {
  /// <summary>
  ///   A facility for the instantiation of Forms
  ///   as views with their associated controllers,
  ///   as per the Model-View-Controller design pattern.
  /// </summary>
  public static class FormFactory {
    /// <summary>
    ///   Creates a Form as a view with its associated controller,
    ///   as per the Model-View-Controller design pattern,
    ///   returning the Form instance created.
    /// </summary>
    /// <param name="args">
    ///   An array of arguments to be passed, after the view instance,
    ///   to the constructor of the controller.
    ///   If an empty array or null,
    ///   the view instance will be the only controller constructor argument.
    /// </param>
    public static Form Create<TView, TController>(params object[] args)
      where TView : Form, IView<TController> {
      return (Form)ViewFactory.Create<MainView, MainController>(args);
    }
  }
}