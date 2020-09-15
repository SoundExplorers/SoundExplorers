using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;
using SoundExplorers.Data;
using VelocityDb.Session;

namespace SoundExplorers.Tests.Data {
  public class TestData {
    static TestData() {
      Chars =
        // ReSharper disable once StringLiteralTypo
        ("abcdefghijklmnopqrstuvwxyz" +
         "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890").ToCharArray();
      EventTypeNames = new List<string> {
        "Performance", "Rehearsal", "Field recording", "Interview", "Installation"
      };
      GenreNames = new List<string> {
        "Free Improvisation", "Jazz", "Composed", "Ambient", "Dance", "Noise", "Folk",
        "Rock"
      };
      LocationNames = new List<string> {
        "Athens", "Berlin", "Copenhagen", "Dublin", "Edinburgh", "Frankfurt", "Geneva",
        "Helsinki"
      };
    }

    public TestData([NotNull] QueryHelper queryHelper) {
      QueryHelper = queryHelper;
      EventTypes = new List<EventType>();
      Genres = new List<Genre>();
      Locations = new List<Location>();
    }

    public IList<EventType> EventTypes { get; }
    public IList<Genre> Genres { get; }
    public IList<Location> Locations { get; }
    private static char[] Chars { get; }
    private static IList<string> EventTypeNames { get; }
    private static IList<string> GenreNames { get; }
    private static IList<string> LocationNames { get; }
    [NotNull] private QueryHelper QueryHelper { get; }

    [NotNull]
    public void AddEventTypesPersisted(int count, [NotNull] SessionBase session) {
      for (var i = 0; i < count; i++) {
        var eventType = new EventType {
          QueryHelper = QueryHelper,
          Name = EventTypes.Count < EventTypeNames.Count
            ? EventTypeNames[EventTypes.Count]
            : CreateUniqueKey(8)
        };
        session.Persist(eventType);
        EventTypes.Add(eventType);
      }
    }

    [NotNull]
    public void AddGenresPersisted(int count, [NotNull] SessionBase session) {
      for (var i = 0; i < count; i++) {
        var genre = new Genre {
          QueryHelper = QueryHelper,
          Name = Genres.Count < GenreNames.Count
            ? GenreNames[Genres.Count]
            : CreateUniqueKey(8)
        };
        session.Persist(genre);
        Genres.Add(genre);
      }
    }

    [NotNull]
    public void AddLocationsPersisted(int count, [NotNull] SessionBase session) {
      for (var i = 0; i < count; i++) {
        var location = new Location {
          QueryHelper = QueryHelper,
          Name = Locations.Count < LocationNames.Count
            ? LocationNames[Locations.Count]
            : CreateUniqueKey(8),
          Notes = CreateUniqueKey(16)
        };
        session.Persist(location);
        Locations.Add(location);
      }
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