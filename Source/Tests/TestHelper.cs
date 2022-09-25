using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using NUnit.Framework;

namespace SoundExplorers.Tests; 

public static class TestHelper {
  /// <summary>
  ///   Waits for a process that is running on another thread to finish.
  /// </summary>
  /// <param name="condition">
  ///   A condition that, when true, will prove that the process has finished.
  /// </param>
  /// <param name="description">
  ///   A description of what we are waiting for, to be shown in an error message if
  ///   the wait times out.
  /// </param>
  /// <param name="maxCount">
  ///   The maximum number of times we should check to ascertain whether the process
  ///   has finished before timing out. Default: 1,000.
  /// </param>
  /// <param name="intervalMilliseconds">
  ///   The interval in milliseconds between checks to ascertain whether the process
  ///   has finished. Default: 1 millisecond.
  /// </param>
  [ExcludeFromCodeCoverage]
  public static void WaitUntil(Func<bool> condition, string description,
    int maxCount = 1000, int intervalMilliseconds = 1) {
    bool finished = false;
    for (int i = 0; i < maxCount; i++) {
      Thread.Sleep(intervalMilliseconds);
      if (condition.Invoke()) {
        finished = true;
        break;
      }
    }
    Assert.IsTrue(finished, description);
  }
}