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
      SeriesNames = new List<string> {
        "Jazz Classics", "New Composers", "Improvised Extravaganza", "Welsh Week",
        "French Fortnight", "Mongolian Month", "Beer Festival", "76 Trombonists"
      };
    }

    public TestData([NotNull] QueryHelper queryHelper) {
      QueryHelper = queryHelper;
      Events = new List<Event>();
      EventTypes = new List<EventType>();
      Genres = new List<Genre>();
      Locations = new List<Location>();
      Newsletters = new List<Newsletter>();
      Series = new List<Series>();
    }

    public IList<Event> Events { get; }
    public IList<EventType> EventTypes { get; }
    public IList<Genre> Genres { get; }
    public IList<Location> Locations { get; }
    public IList<Newsletter> Newsletters { get; }
    public IList<Series> Series { get; }
    private static char[] Chars { get; }
    private static IList<string> EventTypeNames { get; }
    private static IList<string> GenreNames { get; }
    private static IList<string> LocationNames { get; }
    private static IList<string> SeriesNames { get; }
    [NotNull] private QueryHelper QueryHelper { get; }

    public void AddEventsPersisted(int count, [NotNull] SessionBase session,
      Location location = null, EventType eventType = null) {
      var date = DateTime.Parse("2020/01/09");
      for (var i = 0; i < count; i++) {
        var @event = new Event {
          QueryHelper = QueryHelper,
          Date = date,
          Location = location ?? GetDefaultLocation(),
          EventType = eventType ?? GetDefaultEventType(),
          Notes = GenerateUniqueName(16)
        };
        session.Persist(@event);
        Events.Add(@event);
        date = date.AddDays(7);
      }
    }

    public void AddEventTypesPersisted(int count, [NotNull] SessionBase session) {
      for (var i = 0; i < count; i++) {
        var eventType = new EventType {
          QueryHelper = QueryHelper,
          Name = EventTypes.Count < EventTypeNames.Count
            ? EventTypeNames[EventTypes.Count]
            : GenerateUniqueName(8)
        };
        session.Persist(eventType);
        EventTypes.Add(eventType);
      }
    }

    public void AddGenresPersisted(int count, [NotNull] SessionBase session) {
      for (var i = 0; i < count; i++) {
        var genre = new Genre {
          QueryHelper = QueryHelper,
          Name = Genres.Count < GenreNames.Count
            ? GenreNames[Genres.Count]
            : GenerateUniqueName(8)
        };
        session.Persist(genre);
        Genres.Add(genre);
      }
    }

    public void AddLocationsPersisted(int count, [NotNull] SessionBase session) {
      for (var i = 0; i < count; i++) {
        var location = new Location {
          QueryHelper = QueryHelper,
          Name = Locations.Count < LocationNames.Count
            ? LocationNames[Locations.Count]
            : GenerateUniqueName(8),
          Notes = GenerateUniqueName(16)
        };
        session.Persist(location);
        Locations.Add(location);
      }
    }

    public void AddNewslettersPersisted(int count, [NotNull] SessionBase session) {
      var date = DateTime.Parse("2020/01/06");
      for (var i = 0; i < count; i++) {
        var newsletter = new Newsletter {
          QueryHelper = QueryHelper,
          Date = date,
          Url = new Uri($"https:///{GenerateUniqueName(8)}.com/{GenerateUniqueName(6)}",
            UriKind.Absolute).ToString()
        };
        session.Persist(newsletter);
        Newsletters.Add(newsletter);
        date = date.AddDays(7);
      }
    }

    public void AddSeriesPersisted(int count, [NotNull] SessionBase session) {
      for (var i = 0; i < count; i++) {
        var series = new Series {
          QueryHelper = QueryHelper,
          Name = Series.Count < SeriesNames.Count
            ? SeriesNames[Series.Count]
            : GenerateUniqueName(8),
          Notes = GenerateUniqueName(16)
        };
        session.Persist(series);
        Series.Add(series);
      }
    }

    [NotNull]
    private EventType GetDefaultEventType() {
      if (EventTypes.Count == 0) {
        throw new InvalidOperationException("An EventType must be added first.");
      }
      return EventTypes[0];
    }

    [NotNull]
    private Location GetDefaultLocation() {
      if (Locations.Count == 0) {
        throw new InvalidOperationException("A Location must be added first.");
      }
      return Locations[0];
    }

    private static string GenerateUniqueName(int size) {
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