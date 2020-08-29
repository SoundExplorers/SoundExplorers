using System;

namespace SoundExplorers.Data {
  /// <summary>
  ///   Performance entity.
  /// </summary>
  internal class Performance : PieceOwningMediaEntity<Performance> {
    [Field(5)] public string Comments { get; set; }
    [PrimaryKeyField(1)] public DateTime Date { get; set; }

    [PrimaryKeyField(2)]
    [ReferencedField("Name", 2)]
    public string Location { get; set; }

    [ReferencedField("Date", 4)] public DateTime Newsletter { get; set; }
    [ReferencedField("Name", 3)] public string Series { get; set; }

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