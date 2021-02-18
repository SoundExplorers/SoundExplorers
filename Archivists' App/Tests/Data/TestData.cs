using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SoundExplorers.Data;
using VelocityDb.Session;

namespace SoundExplorers.Tests.Data {
  public class TestData {
    private IList<string>? _actNames;
    private char[]? _chars;
    private EventType? _defaultEventType;
    private IList<string>? _eventTypeNames;
    private IList<string>? _genreNames;
    private IList<string>? _locationNames;
    private IList<string>? _roleNames;
    private IList<string>? _seriesNames;

    public TestData(QueryHelper queryHelper) {
      QueryHelper = queryHelper;
      Acts = new List<Act>();
      Events = new List<Event>();
      EventTypes = new List<EventType>();
      Genres = new List<Genre>();
      Locations = new List<Location>();
      Newsletters = new List<Newsletter>();
      Pieces = new List<Piece>();
      Roles = new List<Role>();
      Series = new List<Series>();
      Sets = new List<Set>();
    }

    public IList<Act> Acts { get; }
    public IList<Event> Events { get; }
    public IList<EventType> EventTypes { get; }
    public IList<Genre> Genres { get; }
    public IList<Location> Locations { get; }
    public IList<Newsletter> Newsletters { get; }
    public IList<Piece> Pieces { get; }
    public IList<Role> Roles { get; }
    public IList<Series> Series { get; }
    public IList<Set> Sets { get; }
    private IList<string> ActNames => _actNames ??= CreateActNames();
    private QueryHelper QueryHelper { get; }
    private char[] Chars => _chars ??= CreateChars();
    private EventType DefaultEventType => _defaultEventType ??= GetDefaultEventType();
    private IList<string> EventTypeNames => _eventTypeNames ??= CreateEventTypeNames();
    private IList<string> GenreNames => _genreNames ??= CreateGenreNames();
    private IList<string> LocationNames => _locationNames ??= CreateLocationNames();
    private IList<string> RoleNames => _roleNames ??= CreateRoleNames();
    private IList<string> SeriesNames => _seriesNames ??= CreateSeriesNames();

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static IList<string> CreateActNames() {
      return new List<string> {
        "Miles Davis Quintet", "Art Ensemble of Chicago", "World Saxophone Quartet",
        "Duke Ellington’s Jazz Orchestra", "Count Basie Orchestra", "Jazz Messengers",
        "Cab Calloway Orchestra", "Mahavishnu Orchestra", "Return to Forever",
        "Weather Report", "AMM", "Borbetomagus", "Last Exit", "M.I.M.E.O.", 
        "Spontaneous Music Ensemble", "Massacre", "Supersilent", "Voice Crack", 
        "Musica Elettronica Viva", "Necks, The", "Iskra 1903", "Fushitsusha", 
        "Die Like a Dog Quartet", "nmperign", "Thing, The", "Company", 
        "Blue Humans, The", "Naked City", "Music Improvisation Company, The", "NoHome",
        "Knead", "Spring Heel Jack", "Family Underground", "Diskaholics Anonymous Trio",
        "Sonic Youth", "Globe Unity Orchestra", "A Handful of Dust", "Vandermark 5, The",
        "Bark!", "Sult", "Sandoz Lab Technicians", "Baloni", "Red Trio", 
        "Jazz Group Arkhangelsk", "Ground Zero", "Revolutionary Ensemble", 
        "Quartet Noir", "DKV Trio", "Full Blast", "Tim Berne's Snakeoil", "Amalgam", 
        "SSSD", "Trockeneis"
      };
    }

    private static char[] CreateChars() {
      // ReSharper disable once StringLiteralTypo
      return
        ("abcdefghijklmnopqrstuvwxyz" +
         "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890").ToCharArray();
    }

    private static IList<string> CreateEventTypeNames() {
      return new List<string> {
        "Performance", "Rehearsal", "Field recording", "Interview", "Installation"
      };
    }

    private static IList<string> CreateGenreNames() {
      return new List<string> {
        "Free Improvisation", "Jazz", "Composed", "Ambient", "Dance", "Noise", "Folk",
        "Rock", "Electronic", "Global"
      };
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static IList<string> CreateLocationNames() {
      return new List<string> {
        "Fred's", "Pyramid Club", "Knitting Factory", "Make It Up Club", 
        "Little Theatre Club", "Red Room", "Luggage Store", "Cafe Oto", "Boat-Ting", 
        "Vortex", "Klinker", "Exploratorium", "Petit Mignon", "Noiseberg" 
      };
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static IList<string> CreateRoleNames() {
      return new List<string> {
        "electric guitar", "acoustic guitar", "nylon-string guitar", "bass", 
        "bass guitar", "drums", "percussion", "keyboard", "piano", "marimba", 
        "vibraphone", "gongs", "maracas", "vocal", "flute", "alto flute", "bass flute", 
        "synthesizer", "electronics", "saxophone", "soprano saxophone", "alto saxophone", 
        "tenor saxophone", "baritone saxophone", "bass saxophone", "violin", "viola", 
        "cello", "double bass", "clarinet", "basset horn", "bass clarinet", "oboe", 
        "cor anglais", "bassoon", "contrabassoon", "french horn", "cornet", "trumpet",
        "trombone", "valve trombone", "bass trombone", "tuba", "euphonium", "tenor horn",
        "sousaphone", "harp", "theorbo"
      };
    }

    private static IList<string> CreateSeriesNames() {
      return new List<string> {
        "Jazz Classics", "New Composers", "Improvised Extravaganza", "Welsh Week",
        "French Fortnight", "Mongolian Month", "Beer Festival", "76 Trombonists"
      };
    }

    public void AddActsPersisted(SessionBase session) {
      AddActsPersisted(ActNames.Count, session);
    }

    public void AddActsPersisted(int count, SessionBase session) {
      InsertDefaultAct(session);
      for (int i = 1; i < count; i++) {
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

    //public void AddCreditsPersisted

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

    public void AddEventTypesPersisted(SessionBase session) {
      AddEventTypesPersisted(EventTypeNames.Count, session);
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

    public void AddGenresPersisted(SessionBase session) {
      AddGenresPersisted(GenreNames.Count, session);
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

    public void AddLocationsPersisted(SessionBase session) {
      AddLocationsPersisted(LocationNames.Count, session);
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
      InsertDefaultNewsletter(session);
      var date = DateTime.Parse("2020/01/06");
      for (int i = 1; i < count; i++) {
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
          Duration = TimeSpan.FromSeconds((i + 1) * 10),
          Notes = GenerateUniqueName(16)
        };
        session.Persist(piece);
        Pieces.Add(piece);
        pieceNo++;
      }
    }

    public void AddRolesPersisted(SessionBase session) {
      AddRolesPersisted(RoleNames.Count, session);
    }

    public void AddRolesPersisted(int count, SessionBase session) {
      for (int i = 0; i < count; i++) {
        var role = new Role {
          QueryHelper = QueryHelper,
          Name = Roles.Count < RoleNames.Count
            ? RoleNames[Roles.Count]
            : GenerateUniqueName(8)
        };
        session.Persist(role);
        Roles.Add(role);
      }
      Sort(Roles);
    }

    public void AddSeriesPersisted(SessionBase session) {
      AddSeriesPersisted(SeriesNames.Count, session);
    }

    public void AddSeriesPersisted(int count, SessionBase session) {
      InsertDefaultSeries(session);
      for (int i = 1; i < count; i++) {
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

    private void InsertDefaultAct(SessionBase session) {
      var act = Act.CreateDefault();
      session.Persist(act);
      Acts.Insert(0, act);
    }

    private void InsertDefaultNewsletter(SessionBase session) {
      var newsletter = Newsletter.CreateDefault();
      session.Persist(newsletter);
      Newsletters.Insert(0, newsletter);
    }

    private void InsertDefaultSeries(SessionBase session) {
      var series = SoundExplorers.Data.Series.CreateDefault();
      session.Persist(series);
      Series.Insert(0, series);
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

    private string GenerateUniqueName(int size) {
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

    private string GenerateUniqueUrl() {
      return new Uri($"https://{GenerateUniqueName(8)}.com/{GenerateUniqueName(6)}",
        UriKind.Absolute).ToString();
    }
    
    private static void Shuffle(IList list) {
      RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
      int n = list.Count;
      while (n > 1)
      {
        byte[] box = new byte[1];
        do provider.GetBytes(box);
        while (!(box[0] < n * (byte.MaxValue / n)));
        int k = (box[0] % n);
        n--;
        var value = list[k];
        list[k] = list[n];
        list[n] = value;
      }
    }

    private static void Sort<TEntity>(IList<TEntity> list) where TEntity : IEntity {
      ((List<TEntity>)list).Sort(new EntityComparer<TEntity>());
    }
  }
}