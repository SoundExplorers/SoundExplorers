using System;
using System.Threading;
using NUnit.Framework;

namespace SoundExplorers.Tests {
  public static class TestHelper {
    public static void WaitUntilTrue(Func<bool> condition, string message, 
      int maxCount = 1000, int millisecondsInterval = 1) {
      bool finished = false;
      for (int i = 0; i < maxCount; i++) {
        Thread.Sleep(millisecondsInterval);
        if (condition.Invoke()) {
          finished = true;
          break;
        }
      }
      Assert.IsTrue(finished, message);
    }
  }
}