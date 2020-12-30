using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SoundExplorers.Data;
using VelocityDb.Session;

namespace SoundExplorers.Tests.Data {
  public class TestData {
    private EventType? _defaultEventType;

    static TestData() {
      // ReSharper disable once StringLiteralTypo
      ActNames = new List<string> {
        "Miles Davis Quintet", "Art Ensemble of Chicago", "World Saxophone Quartet",
        "Duke Ellington’s Jazz Orchestra", "Count Basie Orchestra", "Jazz Messengers",
        "Cab Calloway Orchestra", "Mahavishnu Orchestra", "Return to Forever",
        "Weather Report"
      };
      Chars =
        // ReSharper disable once StringLiteralTypo
        ("abcdefghijklmnopqrstuvwxyz" +
         "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890").ToCharArray();
      EventTypeNames = new List<string> {
        "Performance", "Rehearsal", "Field recording", "Interview", "Installation"
      };
      GenreNames = new List<string> {
        "Free Improvisation", "Jazz", "Composed", "Ambient", "Dance", "Noise", "Folk",
        "Rock", "Electronic", "Global"
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

    public TestData(QueryHelper queryHelper) {
      QueryHelper = queryHelper;
      Acts = new List<Act>();
      Events = new List<Event>();
      EventTypes = new List<EventType>();
      Genres = new List<Genre>();
      Locations = new List<Location>();
      Newsletters = new List<Newsletter>();
      Pieces = new List<Piece>();
      Series = new List<Series>();
      Sets = new List<Set>();
    }

    public IList<Act> Acts { get; }

    private EventType DefaultEventType =>
      _defaultEventType ??= GetDefaultEventType();

    public IList<Event> Events { get; }
    public IList<EventType> EventTypes { get; }
    public IList<Genre> Genres { get; }
    public IList<Location> Locations { get; }
    public IList<Newsletter> Newsletters { get; }
    public IList<Piece> Pieces { get; }
    public IList<Series> Series { get; }
    public IList<Set> Sets { get; }
    private static IList<string> ActNames { get; }
    private static char[] Chars { get; }
    private static IList<string> EventTypeNames { get; }
    private static IList<string> GenreNames { get; }
    private static IList<string> LocationNames { get; }
    private QueryHelper QueryHelper { get; }
    private static IList<string> SeriesNames { get; }

    public void AddActsPersisted(int count, SessionBase session) {
      for (int i = 0; i < count; i++) {
        var act = new Act {
          QueryHelper = QueryHelper,
          Name = Acts.Count < ActNames.Count
            ? ActNames[Acts.Count]
            : GenerateUniqueName(8),
          Notes = GenerateUniqueName(16)
        };
        session.Persist(act);
        Acts.Add(act);
      }
      Sort(Acts);
    }

    public void AddEventsPersisted(int count, SessionBase session,
      Location? location = null, EventType? eventType = null) {
      var date = DateTime.Parse("2020/01/09");
      for (int i = 0; i < count; i++) {
        var @event = new Event {
          QueryHelper = QueryHelper,
          Date = date,
          Location = location ?? GetDefaultLocation(),
          EventType = eventType ?? DefaultEventType,
          Notes = GenerateUniqueName(16)
        };
        session.Persist(@event);
        Events.Add(@event);
        date = date.AddDays(7);
      }
    }

    public void AddEventTypesPersisted(int count, SessionBase session) {
      for (int i = 0; i < count; i++) {
        var eventType = new EventType {
          QueryHelper = QueryHelper,
          Name = EventTypes.Count < EventTypeNames.Count
            ? EventTypeNames[EventTypes.Count]
            : GenerateUniqueName(8)
        };
        session.Persist(eventType);
        EventTypes.Add(eventType);
      }
      Sort(EventTypes);
    }

    public void AddGenresPersisted(int count, SessionBase session) {
      for (int i = 0; i < count; i++) {
        var genre = new Genre {
          QueryHelper = QueryHelper,
          Name = Genres.Count < GenreNames.Count
            ? GenreNames[Genres.Count]
            : GenerateUniqueName(8)
        };
        session.Persist(genre);
        Genres.Add(genre);
      }
      Sort(Genres);
    }

    public void AddLocationsPersisted(int count, SessionBase session) {
      for (int i = 0; i < count; i++) {
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
      Sort(Locations);
    }

    public void AddNewslettersPersisted(int count, SessionBase session) {
      var date = DateTime.Parse("2020/01/06");
      for (int i = 0; i < count; i++) {
        var newsletter = new Newsletter {
          QueryHelper = QueryHelper,
          Date = date,
          Url = GenerateUniqueUrl()
        };
        session.Persist(newsletter);
        Newsletters.Add(newsletter);
        date = date.AddDays(7);
      }
    }

    public void AddPiecesPersisted(int count, SessionBase session,
      Set? set = null) {
      int pieceNo = 1;
      for (int i = 0; i < count; i++) {
        var piece = new Piece {
          QueryHelper = QueryHelper,
          PieceNo = pieceNo,
          Set = set ?? GetDefaultSet(),
          AudioUrl = GenerateUniqueUrl(),
          VideoUrl = GenerateUniqueUrl(),
          Title = GenerateUniqueName(8),
          Notes = GenerateUniqueName(16)
        };
        session.Persist(piece);
        Pieces.Add(piece);
        pieceNo++;
      }
    }

    public void AddSeriesPersisted(int count, SessionBase session) {
      for (int i = 0; i < count; i++) {
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
      Sort(Series);
    }

    public void AddSetsPersisted(int count, SessionBase session,
      Event? @event = null, Genre? genre = null) {
      int setNo = 1;
      for (int i = 0; i < count; i++) {
        var set = new Set {
          QueryHelper = QueryHelper,
          SetNo = setNo,
          Event = @event ?? GetDefaultEvent(),
          Genre = genre ?? GetDefaultGenre(),
          Notes = GenerateUniqueName(16)
        };
        session.Persist(set);
        Sets.Add(set);
        setNo++;
      }
    }

    private Event GetDefaultEvent() {
      return Events.Count >= 0
        ? Events[0]
        : throw new InvalidOperationException("An Event must be added first.");
    }

    private EventType GetDefaultEventType() {
      return EventTypes.Count >= 0
        ? (from eventType in EventTypes
          where eventType.Name == "Performance"
          select eventType).First()
        : throw new InvalidOperationException("An EventType must be added first.");
    }

    private Genre GetDefaultGenre() {
      return Genres.Count >= 0
        ? Genres[0]
        : throw new InvalidOperationException("A Genre must be added first.");
    }

    private Location GetDefaultLocation() {
      return Locations.Count >= 0
        ? Locations[0]
        : throw new InvalidOperationException("A Location must be added first.");
    }

    private Set GetDefaultSet() {
      return Sets.Count >= 0
        ? Sets[0]
        : throw new InvalidOperationException("A Set must be added first.");
    }

    private static string GenerateUniqueName(int size) {
      byte[] data = new byte[4 * size];
      using (var crypto = new RNGCryptoServiceProvider()) {
        crypto.GetBytes(data);
      }
      var result = new StringBuilder(size);
      for (int i = 0; i < size; i++) {
        uint rnd = BitConverter.ToUInt32(data, i * 4);
        uint idx = Convert.ToUInt32(rnd % Chars.Length);
        result.Append(Chars[idx]);
      }
      return result.ToString();
    }

    private static string GenerateUniqueUrl() {
      return new Uri($"https://{GenerateUniqueName(8)}.com/{GenerateUniqueName(6)}",
        UriKind.Absolute).ToString();
    }

    private static void Sort<TEntity>(IList<TEntity> list) where TEntity : IEntity {
      ((List<TEntity>)list).Sort(new EntityComparer<TEntity>());
    }
  }
}