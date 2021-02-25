using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SoundExplorers.Data;
using VelocityDb.Session;

namespace SoundExplorers.Tests.Data {
  [ExcludeFromCodeCoverage]
  public class TestData {
    private static char[]? _letters;
    private IList<string>? _actNames;
    private EventType? _defaultEventType;
    private IList<string>? _eventTypeNames;
    private IList<string>? _forenames;
    private IList<string>? _genreNames;
    private IList<string>? _locationNames;
    private IList<string>? _roleNames;
    private IList<string>? _seriesNames;
    private IList<string>? _surnames;

    public TestData(QueryHelper queryHelper) {
      QueryHelper = queryHelper;
      Acts = new List<Act>();
      Artists = new List<Artist>();
      Credits = new List<Credit>();
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
    public IList<Artist> Artists { get; }
    public IList<Credit> Credits { get; }
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
    private EventType DefaultEventType => _defaultEventType ??= GetDefaultEventType();
    private IList<string> EventTypeNames => _eventTypeNames ??= CreateEventTypeNames();
    private IList<string> Forenames => _forenames ??= CreateForenames();
    private IList<string> GenreNames => _genreNames ??= CreateGenreNames();
    private static char[] Letters => _letters ??= CreateLetters();
    private IList<string> LocationNames => _locationNames ??= CreateLocationNames();
    private IList<string> RoleNames => _roleNames ??= CreateRoleNames();
    private IList<string> SeriesNames => _seriesNames ??= CreateSeriesNames();
    private IList<string> Surnames => _surnames ??= CreateSurnames();

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
          Notes = GenerateNotes()
        };
        session.Persist(act);
        Acts.Add(act);
      }
      Acts.Sort();
    }

    public void AddArtistsPersisted(int count, SessionBase session) {
      var names = new HashSet<string>(count);
      for (int i = 0; i < count; i++) {
        var artist = new Artist {
          QueryHelper = QueryHelper,
          Notes = GenerateNotes()
        };
        do {
          artist.Forename = GetRandomForename();
          artist.Surname = GetRandomSurname();
        } while (names.Contains(artist.Name));
        names.Add(artist.Name);
        session.Persist(artist);
        Artists.Add(artist);
      }
      Artists.Sort();
    }

    public void AddCreditsPersisted(int count, SessionBase session,
      Piece? piece = null, Artist? artist = null, Role? role = null) {
      var parentPiece = piece ?? GetDefaultPiece();
      int creditNo = parentPiece.Credits.Count == 0
        ? 1
        : parentPiece.Credits[^1].CreditNo + 1;
      for (int i = 0; i < count; i++) {
        var credit = new Credit {
          QueryHelper = QueryHelper,
          CreditNo = creditNo,
          Piece = parentPiece,
          Artist = artist ?? GetDefaultArtist(),
          Role = role ?? GetDefaultRole()
        };
        session.Persist(credit);
        Credits.Add(credit);
        creditNo++;
      }
    }

    public void AddEventsPersisted(int count, SessionBase session,
      Location? location = null, EventType? eventType = null) {
      var date = Events.Count == 0
        ? DateTime.Parse("2020/01/09")
        : Events[^1].Date.AddDays(7);
      for (int i = 0; i < count; i++) {
        var @event = new Event {
          QueryHelper = QueryHelper,
          Date = date,
          Location = location ?? GetDefaultLocation(),
          EventType = eventType ?? DefaultEventType,
          Notes = GenerateNotes()
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
      EventTypes.Sort();
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
      Genres.Sort();
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
          Notes = GenerateNotes()
        };
        session.Persist(location);
        Locations.Add(location);
      }
      Locations.Sort();
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
      var parentSet = set ?? GetDefaultSet();
      int pieceNo = parentSet.Pieces.Count == 0 ? 1 : parentSet.Pieces[^1].PieceNo + 1;
      for (int i = 0; i < count; i++) {
        var piece = new Piece {
          QueryHelper = QueryHelper,
          PieceNo = pieceNo,
          Set = parentSet,
          AudioUrl = GenerateUniqueUrl(),
          VideoUrl = GenerateUniqueUrl(),
          Title = GenerateUniqueName(8),
          Duration = GetRandomDuration(),
          Notes = GenerateNotes()
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
      Roles.Sort();
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
          Notes = GenerateNotes()
        };
        session.Persist(series);
        Series.Add(series);
      }
      Series.Sort();
    }

    public void AddSetsPersisted(int count, SessionBase session,
      Event? @event = null, Genre? genre = null) {
      var parentEvent = @event ?? GetDefaultEvent();
      int setNo = parentEvent.Sets.Count == 0 ? 1 : parentEvent.Sets[^1].SetNo + 1;
      for (int i = 0; i < count; i++) {
        var set = new Set {
          QueryHelper = QueryHelper,
          SetNo = setNo,
          Event = parentEvent,
          Genre = genre ?? GetDefaultGenre(),
          Notes = GenerateNotes()
        };
        session.Persist(set);
        Sets.Add(set);
        setNo++;
      }
    }

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
        "Blue Humans", "Naked City", "Music Improvisation Company", "NoHome",
        "Knead", "Spring Heel Jack", "Family Underground", "Diskaholics Anonymous Trio",
        "Sonic Youth", "Globe Unity Orchestra", "A Handful of Dust", "Vandermark 5",
        "Bark!", "Sandoz Lab Technicians", "Baloni", "Red Trio",
        "Jazz Group Arkhangelsk", "Ground Zero", "Revolutionary Ensemble",
        "Quartet Noir", "DKV Trio", "Full Blast", "Tim Berne's Snakeoil", "Amalgam",
        "SSSD", "Trockeneis"
      }.Shuffle();
    }

    private static IList<string> CreateEventTypeNames() {
      return new List<string> {
        "Performance", "Rehearsal", "Field recording", "Interview", "Installation"
      };
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static IList<string> CreateForenames() {
      return new List<string> {
        "Simon", "Dan", "Siobhan", "Jessica", "Peter", "Patricia", "Sara", "Sean",
        "Giovanni", "Francois", "Francoise", "Terry", "Joanne", "John", "Susan",
        "Gaston", "Pierre", "Gerry", "Geoff", "Andy", "Andrea", "Scot", "Seamus",
        "Rose", "Rory", "Alice", "Alison", "Mike", "Morton", "Quentin", "Jim", "James",
        "Portia", "Jill", "Sam", "Samantha", "Maria", "Mary", "Bill", "William", "Will",
        "Andrew", "Gloria", "Sean", "Martin", "Martina", "Paul", "Paula", "Paulo",
        "Pauline", "Vivian", "Vivienne", "Constance", "Connie", "Jon", "Jonathan",
        "Jonny", "Gerard", "Gerd", "Astrid", "Zoe", "Yvette", "Olive", "Chris",
        "Christine", "Barbara", "Frank", "Bernard", "Brenda", "Tom", "Thomas", "Fergus",
        "Molly", "Katherine", "Linda", "Lionel", "Len", "Ben", "Benjamin", "Leonard",
        "Leonardo", "Francesca", "Francis", "Frances", "Terri", "Miguel", "Joan",
        "Catherine", "Maia", "Julie", "Julian", "Robert", "Bob", "Robbie", "Rachael",
        "Rebecca", "Jude", "Judy", "Henry", "Enrico", "Harriet", "Harry", "Barry",
        "Larry", "Laurence", "Lawrence", "Stan", "Sally", "Tim", "Charlotte", "Charles",
        "Charlie", "Karen", "Kieran", "Victoria", "Laurel", "Gary", "Giselle", "Derek",
        "Richard", "Rick", "Ricardo", "Sandra", "Dai", "Saed", "George", "Charmaine",
        "Hermione", "Allan", "Francesco", "Toshio", "Mauricio", "Luigi", "Tristan",
        "Antony", "Anthony", "Antonia", "Toni", "Olga", "Roberto", "Thierry", "Mauro",
        "Liza", "Robin", "Aaron", "Eduardo", "Enno", "Iannis", "Arnulf", "Uwe", "Stefan",
        "Simeon", "Kaija", "Brian", "Gerald", "Geraldine", "Claude", "Conrad", "Heather",
        "Tracy", "Carlos"
      }.Shuffle();
    }

    private static IList<string> CreateGenreNames() {
      return new List<string> {
        "Free Improvisation", "Jazz", "Composed", "Ambient", "Dance", "Noise", "Folk",
        "Rock", "Electronic", "Global"
      }.Shuffle();
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static char[] CreateLetters() {
      return
        ("abcdefghijklmnopqrstuvwxyz" +
         "ABCDEFGHIJKLMNOPQRSTUVWXYZ").ToCharArray();
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static IList<string> CreateLocationNames() {
      return new List<string> {
        "Fred's", "Pyramid Club", "Knitting Factory", "Make It Up Club",
        "Little Theatre Club", "Red Room", "Luggage Store", "Cafe Oto", "Boat-Ting",
        "Vortex", "Klinker", "Exploratorium", "Petit Mignon", "Noiseberg"
      }.Shuffle();
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static IList<string> CreateRoleNames() {
      return new List<string> {
        "guitar", "electric guitar", "acoustic guitar", "nylon-string guitar", "bass",
        "bass guitar", "drums", "percussion", "keyboard", "piano", "marimba",
        "vibraphone", "gongs", "maracas", "vocal", "flute", "alto flute", "bass flute",
        "synthesizer", "electronics", "saxophone", "soprano saxophone", "alto saxophone",
        "tenor saxophone", "baritone saxophone", "bass saxophone", "violin", "viola",
        "cello", "double bass", "clarinet", "basset horn", "bass clarinet", "oboe",
        "cor anglais", "bassoon", "contrabassoon", "french horn", "cornet", "trumpet",
        "trombone", "valve trombone", "bass trombone", "tuba", "euphonium", "tenor horn",
        "sousaphone", "harp", "theorbo", "serpent"
      }.Shuffle();
    }

    private static IList<string> CreateSeriesNames() {
      return new List<string> {
        "Jazz Classics", "New Composers", "Improvised Extravaganza", "Welsh Week",
        "French Fortnight", "Mongolian Month", "Beer Festival", "76 Trombonists"
      }.Shuffle();
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static IList<string> CreateSurnames() {
      return new List<string> {
        "O'Rorke", "Beban", "Marks", "Monaghan", "Daly", "Richardson", "Prosser",
        "Murray", "Monks", "Jenkins", "Franks", "Torricelli", "Meunes", "Jervois",
        "Smithson", "Jackson", "Bern", "Norris", "Anderson", "Brandt", "Brand",
        "Sanderson", "O'Leary", "Beethoven", "Bach", "Coltrane", "Bartlet", "Fujikura",
        "Grisey", "Feldman", "Stockhausen", "Brown", "Levinas", "Pesson", "Keller",
        "Haddad", "Lewis", "Lee", "Cheung", "Pluta", "Webern", "Wuorinen", "Schoenberg",
        "Johnson", "Babbitt", "Petterson", "Filidei", "Markoulaki", "Penderecki",
        "Hosokawa", "Kagel", "Nono", "Carter", "Murail", "Ligeti", "Turnage", "Neuwirth",
        "Gerhard", "Barrett", "Blondeau", "Saunders", "Lanza", "Dillon", "Lim",
        "Hoffman", "Cassidy", "Hayden", "Pelzel", "Moguillansky", "Poppe", "Xenakis",
        "Herrmann", "Dierksen", "Crumb", "Prins", "Cage", "ten Holt", "Saariaho",
        "Ferneyhough", "Boulez", "Gahn", "Eckert", "Adams", "Psathas", "Edlund", "Wood",
        "Hendrickson", "Larsen", "Li", "van Bavel", "Scheidel", "Lowe", "Shaughnesy",
        "Brook", "Beard", "Comninel", "Teschke", "Post", "Dimmock", "Wickham", "Brenner",
        "Huang", "Meillassoux", "Prakash", "Luxemburg", "Hall", "Sahlins", "Cunliffe",
        "Goldthwaite", "Totman", "Bellwood", "Wolf", "Calloway", "Renfrew", "Wilkinson",
        "Heather", "Banks", "Chevalier", "Zafon", "Young", "Wright", "Wong", "Winant",
        "Wilson", "Whitehead", "Westbrook", "Watkins", "Wallace", "Vinkeloe", "Ueno",
        "Swanagon", "Staiano", "Stackpole", "Sonami", "Skatchit", "Shola", "Shiurba",
        "Shen", "Scandura", "Santomieri", "Ryan", "Ruviaro", "Romus", "Rodriguez",
        "Robinson", "Robair", "Rivero", "Rieman", "Reid", "Reed", "Raskin", "Powell",
        "Pontecorvo", "Plonsey", "Perley", "Perkis", "Pek", "Pearce", "Payne",
        "Pascucci", "Orr", "Ochs", "Nunn", "Nordeson", "Noertker", "Neuburg", "Moller",
        "Mitchell", "Mihaly", "Michalak", "Mezzacappa", "Menegon", "McKean", "McDonas",
        "McCaslin", "Marshall", "Lynner", "Lopez", "Lockhart", "Levin", "Leikam", "Lee",
        "Koskinen", "Josephson", "Jordan", "Johnston", "Jamieson", "Jahde", "Ingle",
        "Ingalls", "Hsu", "Honda", "Holm", "Hikage", "Heule", "Hertz", "Herndon",
        "Heglin", "Hardy", "Hammond", "Golden", "Goldberg", "Gelb", "Gale", "Frith",
        "Fei", "Farhadian", "Everett", "Dutton", "Dunkelman", "Dubowsky", "Djll",
        "Diomede", "Dingalls", "Dimuzio", "Diaz-Infante", "DeGruttola", "Decosta",
        "DeCillis", "Day", "Davignon", "Crossman", "Corcoran", "Cooke", "Condry",
        "Coleman", "Clevenger", "Chen", "Chaudhary", "Chandavarkar", "Carson", "Carroll",
        "Carey", "Cahill", "Burns", "Bruckmann", "Boisen", "Bischoff", "Bickley",
        "Bennett", "Benedict", "Beckman", "Banerji", "Ateria", "Atchley", "Arkin",
        "Amendola"
      }.Shuffle();
    }

    private static string GenerateNotes() {
      var writer = new StringWriter();
      int chance = GetRandomInteger(1, 3);
      if (chance == 1) {
        int wordCount = GetRandomInteger(1, 4);
        for (int i = 0; i < wordCount; i++) {
          int wordLength = GetRandomInteger(2, 10);
          writer.Write($"{GenerateUniqueName(wordLength)} ");
        }
      }
      return writer.ToString().TrimEnd();
    }

    private static string GenerateUniqueName(int size) {
      byte[] data = new byte[4 * size];
      using (var crypto = new RNGCryptoServiceProvider()) {
        crypto.GetBytes(data);
      }
      var result = new StringBuilder(size);
      for (int i = 0; i < size; i++) {
        uint rnd = BitConverter.ToUInt32(data, i * 4);
        uint idx = Convert.ToUInt32(rnd % Letters.Length);
        result.Append(Letters[idx]);
      }
      return result.ToString();
    }

    public static string GenerateUniqueUrl() {
      return new Uri(
        $"https://{GenerateUniqueName(8)}.com/{GenerateUniqueName(6)}",
        UriKind.Absolute).ToString();
    }

    private Artist GetDefaultArtist() {
      return GetEntity<Artist, IList<Artist>>(Artists, 0);
    }

    private Event GetDefaultEvent() {
      return GetEntity<Event, IList<Event>>(Events, 0);
    }

    private EventType GetDefaultEventType() {
      return EventTypes.Count >= 0
        ? (from eventType in EventTypes
          where eventType.Name == "Performance"
          select eventType).First()
        : throw new InvalidOperationException(
          "Default EventType 'Performance' must be added first.");
    }

    private Genre GetDefaultGenre() {
      return GetEntity<Genre, IList<Genre>>(Genres, 0);
    }

    private Location GetDefaultLocation() {
      return GetEntity<Location, IList<Location>>(Locations, 0);
    }

    private Piece GetDefaultPiece() {
      return GetEntity<Piece, IList<Piece>>(Pieces, 0);
    }

    private Role GetDefaultRole() {
      return GetEntity<Role, IList<Role>>(Roles, 0);
    }

    private Set GetDefaultSet() {
      return GetEntity<Set, IList<Set>>(Sets, 0);
    }

    private static TEntity GetEntity<TEntity, TList>(TList list, int index)
      where TEntity : EntityBase
      where TList : IList<TEntity> {
      return index >= 0 && index < list.Count
        ? list[index]
        : throw new InvalidOperationException(
          $"{typeof(TEntity).Name} {index} has not been added.");
    }

    public Act GetRandomAct() {
      return GetRandomEntity<Act, IList<Act>>(Acts);
    }

    public Artist GetRandomArtist() {
      return GetRandomEntity<Artist, IList<Artist>>(Artists);
    }

    public static TimeSpan GetRandomDuration() {
      int minutes = GetRandomInteger(1, 14);
      int seconds = GetRandomInteger(0, 59);
      return new TimeSpan(0, minutes, seconds);
    }

    private static TEntity GetRandomEntity<TEntity, TList>(TList list)
      where TEntity : EntityBase
      where TList : IList<TEntity> {
      return list.Count > 0
        ? list[GetRandomInteger(0, list.Count - 1)]
        : throw new InvalidOperationException(
          $"No {typeof(TEntity).Name}s have been added.");
    }

    public EventType GetRandomEventType() {
      return GetRandomEntity<EventType, IList<EventType>>(EventTypes);
    }

    private string GetRandomForename() {
      return Forenames[GetRandomInteger(0, Forenames.Count - 1)];
    }

    public Genre GetRandomGenre() {
      return GetRandomEntity<Genre, IList<Genre>>(Genres);
    }

    public static int GetRandomInteger(int min, int max) {
      RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
      byte[] buffer = new byte[4];
      rng.GetBytes(buffer);
      int result = BitConverter.ToInt32(buffer, 0);
      return new Random(result).Next(min, max + 1);
    }

    public Location GetRandomLocation() {
      return GetRandomEntity<Location, IList<Location>>(Locations);
    }

    public Role GetRandomRole() {
      return GetRandomEntity<Role, IList<Role>>(Roles);
    }

    public Series GetRandomSeries() {
      return GetRandomEntity<Series, IList<Series>>(Series);
    }

    private string GetRandomSurname() {
      return Surnames[GetRandomInteger(0, Surnames.Count - 1)];
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
  }
}