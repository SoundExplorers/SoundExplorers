using System;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Tests.Data {
  public class TestDataFactory {
    static TestDataFactory() {
      Chars =
        // ReSharper disable once StringLiteralTypo
        ("abcdefghijklmnopqrstuvwxyz" +
         "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890").ToCharArray();
    }

    public TestDataFactory([NotNull] QueryHelper queryHelper) {
      QueryHelper = queryHelper;
    }

    private static char[] Chars { get; }
    [NotNull] private QueryHelper QueryHelper { get; }

    [NotNull]
    public EventType CreateEventTypePersisted([NotNull] SessionBase session) {
      var result = new EventType {
        QueryHelper = QueryHelper,
        Name = CreateUniqueKey(8)
      };
      session.Persist(result);
      return result;
    }

    [NotNull]
    public Genre CreateGenrePersisted([NotNull] SessionBase session) {
      var result = new Genre {
        QueryHelper = QueryHelper,
        Name = CreateUniqueKey(8)
      };
      session.Persist(result);
      return result;
    }

    [NotNull]
    public Location CreateLocationPersisted([NotNull] SessionBase session) {
      var result = new Location {
        QueryHelper = QueryHelper,
        Name = CreateUniqueKey(8),
        Notes = CreateUniqueKey(16)
      };
      session.Persist(result);
      return result;
    }

    private static string CreateUniqueKey(int size) {
      var data = new byte[4 * size];
      using (var crypto = new RNGCryptoServiceProvider()) {
        crypto.GetBytes(data);
      }
      var result = new StringBuilder(size);
      for (var i = 0; i < size; i++) {
        var rnd = BitConverter.ToUInt32(data, i * 4);
        var idx = Convert.ToUInt32(rnd % Chars.Length);
        result.Append(Chars[idx]);
      }
      return result.ToString();
    }
  }
}