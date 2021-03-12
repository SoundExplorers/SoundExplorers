using System.Collections.Generic;
using System.Security.Cryptography;
using SoundExplorers.Data;
using SoundExplorers.Model;

namespace SoundExplorers.Tests.Data {
  public static class ListExtensions {
    public static IList<T> Shuffle<T>(this IEnumerable<T> list) {
      var result = new List<T>(list);
      RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
      int n = result.Count;
      while (n > 1) {
        byte[] box = new byte[1];
        do {
          provider.GetBytes(box);
        } while (!(box[0] < n * (byte.MaxValue / n)));
        int k = box[0] % n;
        n--;
        var value = result[k];
        result[k] = result[n];
        result[n] = value;
      }
      return result;
    }

    public static void Sort<TEntity>(this IList<TEntity> list) where TEntity : IEntity {
      ((List<TEntity>)list).Sort(new TopLevelEntityComparer<TEntity>());
    }
  }
}