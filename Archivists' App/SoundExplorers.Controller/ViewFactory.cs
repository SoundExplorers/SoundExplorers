using System;
using System.Collections.Generic;
using System.Reflection;

namespace SoundExplorers.Controller {
  /// <summary>
  ///   A facility for the instantiation of views and their associated controllers,
  ///   as per the Model-View-Controller design pattern.
  /// </summary>
  public static class ViewFactory {
    /// <summary>
    ///   Creates a view and its associated controllers,
    ///   as per the Model-View-Controller design pattern,
    ///   returning the view instance created.
    /// </summary>
    /// <param name="args">
    ///   An array of arguments to be passed, after to the view instance,
    ///   to the constructor of the controller.
    ///   If an empty array or null,
    ///   the view instance will be the only controller constructor argument.
    /// </param>
    public static IView<TController> Create<TView, TController>(params object[] args)
      where TView : IView<TController>, new() {
      var result = new TView();
      try {
        Activator.CreateInstance(typeof(TController), PrependViewToArgs(result, args));
      } catch (TargetInvocationException ex) {
        throw ex.InnerException ?? ex;
      }
      return result;
    }

    private static object[] PrependViewToArgs(object view, params object[] args) {
      var list = new List<object> {view};
      list.AddRange(args);
      return list.ToArray();
    }
  }
}