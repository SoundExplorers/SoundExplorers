using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SoundExplorers {
  /// <summary>
  ///   A keyed list of event shortcuts with
  ///   <see cref="EventShortcut.KeyCode" /> as the key.
  /// </summary>
  internal class EventShortcutList : List<EventShortcut> {
    /// <summary>
    ///   Returns the event shortcut with the specified key code,
    ///   if found, otherwise returns a null reference.
    /// </summary>
    /// <param name="keyCode">
    ///   The key code of the key that has to be pressed
    ///   while a Mac keyboard's command (⌘) key or
    ///   a Windows keyboard's Windows (Start) key is down
    ///   to raise the event.
    /// </param>
    /// <returns>
    ///   The event shortcut with the specified key code,
    ///   if found, otherwise a null reference.
    /// </returns>
    public EventShortcut this[Keys keyCode] =>
    (
      from EventShortcut eventShortcut in this
      where eventShortcut.KeyCode == keyCode
      select eventShortcut).FirstOrDefault();

    /// <summary>
    ///   Add the specified event shortcut to the list,
    ///   provided its key code is unique in the list.
    /// </summary>
    /// <param name="eventShortcut">
    ///   The event shortcut to be added.
    /// </param>
    /// <exception cref="ArgumentException">
    ///   An event shortcut for the specified event shortcut's
    ///   key code is already on the list.
    /// </exception>
    public new void Add(EventShortcut eventShortcut) {
      if (ContainsKey(eventShortcut.KeyCode)) {
        throw new ArgumentException(
          "An EventShortcut for \"" + eventShortcut.KeyCode
                                    + "\" already exists.",
          "eventShortcut");
      }
      base.Add(eventShortcut);
    }

    /// <summary>
    ///   Returns whether the list contains
    ///   an event shortcut with the specified key code.
    /// </summary>
    /// <param name="keyCode">
    ///   The key code of the key that has to be pressed
    ///   while a Mac keyboard's command (⌘) key or
    ///   a Windows keyboard's Windows (Start) key is down
    ///   to raise the event.
    /// </param>
    /// <returns>
    ///   Whether the list contains
    ///   an event shortcut with the specified key code.
    /// </returns>
    public bool ContainsKey(Keys keyCode) {
      return (
        from EventShortcut eventShortcut in this
        where eventShortcut.KeyCode == keyCode
        select eventShortcut).Count() > 0;
    }
  } //End of class
} //End of namespace