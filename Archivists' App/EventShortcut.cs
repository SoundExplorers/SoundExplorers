using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace SoundExplorers {
  /// <summary>
  ///   A keyboard shortcut that raises an event.
  /// </summary>
  /// <remarks>
  ///   This is used to provide an alternative shortcut
  ///   using a command (⌘) key on a Mac keyboard
  ///   to substitute for a menu item shortcut
  ///   that uses a control key on a Windows keyboard.
  /// </remarks>
  internal class EventShortcut {
    /// <summary>
    ///   Initialises a new instance of the <see cref="EventShortcut" /> class.
    /// </summary>
    /// <param name="keyCode">
    ///   The key code of the key that has to be pressed
    ///   while a Mac keyboard's command (⌘) key or
    ///   a Windows keyboard's Windows (Start) key is down
    ///   to raise the event.
    /// </param>
    /// <param name="target">
    ///   The target object that has the event that the keyboard shortcut will raise.
    /// </param>
    /// <param name="eventName">
    ///   The name of the event that the keyboard shortcut will raise.
    /// </param>
    public EventShortcut(Keys keyCode, object target, string eventName) {
      KeyCode = keyCode;
      Target = target;
      EventName = eventName;
      Subscribers = GetSubscribers();
    }

    /// <summary>
    ///   Gets the name of the event that the keyboard shortcut will raise.
    /// </summary>
    public string EventName { get; }

    /// <summary>
    ///   Gets the key code of the key that has to be pressed
    ///   while a Mac keyboard's command (⌘) key or
    ///   a Windows keyboard's Windows (Start) key is down
    ///   to raise the event.
    /// </summary>
    public Keys KeyCode { get; }

    /// <summary>
    ///   The subscribers to the event.
    /// </summary>
    private Delegate[] Subscribers { get; }

    /// <summary>
    ///   Gets target object that has the event that the keyboard shortcut will raise.
    /// </summary>
    public object Target { get; }

    /// <summary>
    ///   Aborts the program with an error message
    ///   containing the full method name
    ///   of the specified event delegate
    ///   and details of the specified exception.
    /// </summary>
    /// <param name="eventDelegate">
    ///   The event delegate of the event handler
    ///   method in which the specified exception
    ///   was thrown.
    /// </param>
    /// <param name="ex">
    ///   The exception that was thrown.
    /// </param>
    /// <remarks>
    ///   Instead of throwing an exception,
    ///   a message box will be displayed and the
    ///   program will then be terminated.
    ///   For unknown reason,
    ///   perhaps to do with either threading or reflection,
    ///   throwing an exception sends the program
    ///   into a loop which insists that the
    ///   exception that is thrown is unhandled.
    ///   This crashes the program unless run in the
    ///   IDE in Debug mode.
    ///   I've tried all sorts of variations with
    ///   the same result.
    ///   <para>
    ///     The exception's stack trace only leads back
    ///     to <see cref="RaiseEvent" />.
    ///     But it is included the message displayed
    ///     in order to show the origin of the message box.
    ///     Otherwise programmer are likely to search in
    ///     vain for the the display of the message box
    ///     in the invoking application.
    ///   </para>
    /// </remarks>
    private static void AbortProgram(
      Delegate eventDelegate, Exception ex) {
      string message =
        ex.GetType()
        + " was thrown in "
        + eventDelegate.Target.ToString().Split(',')[0]
        + "." + eventDelegate.Method.Name + ":"
        + Environment.NewLine
        + ex;
      MessageBox.Show(
        message,
        Application.ProductName,
        MessageBoxButtons.OK,
        MessageBoxIcon.Error);
      Environment.Exit(0);
    }

    /// <summary>
    ///   Gets the subscribers to the event.
    /// </summary>
    /// <returns>
    ///   The subscribers to the event.
    /// </returns>
    /// <remarks>
    ///   Based on
    ///   "How to obtain the invocation list of any event"
    ///   by Bob Powell:
    ///   http://www.bobpowell.net/eventsubscribers.htm.
    /// </remarks>
    private Delegate[] GetSubscribers() {
      string winFormsEventName = "Event" + EventName;
      var targetType = Target.GetType();
      do {
        var fields = targetType.GetFields(
          BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (var field in fields) {
          if (field.Name == EventName) {
            //we've found the compiler generated event
            var eventDelegate = field.GetValue(Target) as Delegate;
            if (eventDelegate != null) {
              return eventDelegate.GetInvocationList();
            }
          }
          if (field.Name == winFormsEventName) {
            //we've found an EventHandlerList key
            //get the list
            var eventHandlers = (EventHandlerList)Target.GetType().GetProperty(
              "Events",
              BindingFlags.Instance
              | BindingFlags.NonPublic
              | BindingFlags.FlattenHierarchy).GetValue(
              Target, null);
            //and dereference the delegate.
            var eventDelegate = eventHandlers[field.GetValue(Target)];
            if (eventDelegate != null) {
              return eventDelegate.GetInvocationList();
            }
          }
        }
        targetType = targetType.BaseType;
      } while (targetType != null);
      return new Delegate[] { };
    }

    /// <summary>
    ///   Raises the specified event with
    ///   the arguments.
    /// </summary>
    /// <param name="e">
    ///   Event arguments.
    /// </param>
    /// <remarks>
    ///   The event will be raised
    ///   in the thread in which the each target of
    ///   the event was instantiated, provided
    ///   the target is derived from
    ///   <see cref="Control" />
    ///   (such as a <see cref="Form" />
    ///   or <see cref="UserControl" />).
    ///   <para>
    ///     If the current thread is a background thread,
    ///     the thread will be put to sleep for a millisecond
    ///     before the event is raised.
    ///     At that point, any outstanding request
    ///     to interrupt the thread,
    ///     which may have been ignored until then,
    ///     can be expected to be actioned.
    ///   </para>
    /// </remarks>
    public void RaiseEvent(EventArgs e) {
      // Even if there are no subscribers to the event,
      // put the thread to sleep, if a background thread,
      // to facilitate thread interruption.
      if (Thread.CurrentThread.IsBackground) {
        Thread.Sleep(1);
      } //End if
      object[] args = {Target, e};
      //Debug.WriteLine("Subscribers.Count = " + Subscribers.Count().ToString());
      foreach (var subscriber in Subscribers) {
        if (subscriber.Target is Control) {
          var control = (Control)subscriber.Target;
          //Debug.WriteLine("control.Invoke");
          try {
            control.Invoke(
              subscriber,
              args);
            // Throwing the InnerException of a 
            // TargetInvocationException is not a good idea
            // because it obscures the fact that the
            // error was handled here.
            //} catch (TargetInvocationException ex1) {
            //    if (!(ex1.InnerException is ThreadInterruptedException)) {
            //        AbortProgram(subscriber, ex1);
            //    }
          } catch (Exception ex) {
            AbortProgram(subscriber, ex);
          }
        } else {
          //Debug.WriteLine("subscriber.DynamicInvoke");
          try {
            subscriber.DynamicInvoke(args);
            //} catch (TargetInvocationException ex1) {
            //    if (!(ex1.InnerException is ThreadInterruptedException)) {
            //        AbortProgram(subscriber, ex1);
            //    }
          } catch (Exception ex) {
            AbortProgram(subscriber, ex);
          }
        } //End if
      } //End foreach
    }
  } //End of class
} //End of namespace