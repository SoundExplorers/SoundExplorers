using System;
using System.Collections.Generic;
using System.Linq;

namespace SoundExplorers.Tests.Data;

public static class ListExtensions {
  public static IList<T> Shuffle<T>(this IEnumerable<T> list) {
    var array = list.ToArray();
    // Perform an in situ Fisher–Yates shuffle on the array.
    var random = new Random();
    int n = array.Length;
    while (n > 1) {
      int k = random.Next(n--);
      (array[n], array[k]) = (array[k], array[n]);
    }
    return array.ToList();
  }
}