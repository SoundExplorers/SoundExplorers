using System;

namespace SoundExplorers.Data {
  /// <summary>
  ///   Performance entity.
  /// </summary>
  internal class Performance : PieceOwningMediaEntity<Performance> {
    [Field] public string Comments { get; set; }
    [PrimaryKeyField] public DateTime Date { get; set; }

    [PrimaryKeyField]
    [ReferencedField("Name")]
    public string Location { get; set; }

    [ReferencedField("Date")] public DateTime Newsletter { get; set; }
    [ReferencedField("Name")] public string Series { get; set; }

    // [PrimaryKeyField] public DateTime Date { get; set; }
    // [PrimaryKeyField] [ReferencedField("Name")] public string Location { get; set; }
    // [ReferencedField("Name")] public string Series { get; set; }
    // [ReferencedField("Date")] public DateTime Newsletter { get; set; }
    // [Field] public string Comments { get; set; }

    /// <summary>
    ///   Fetches the Performance's Newsletter (i.e. not just the Newsletter date)
    ///   from the database.
    /// </summary>
    /// <returns>
    ///   The Performance's Newsletter.
    /// </returns>
    public virtual Newsletter FetchNewsletter() {
      var newsletter = new Newsletter();
      newsletter.Date = Newsletter;
      newsletter.Fetch();
      return newsletter;
    }
  } //End of class
} //End of namespace