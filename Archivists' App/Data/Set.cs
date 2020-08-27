﻿using System;

namespace SoundExplorers.Data {
  /// <summary>
  ///   Set entity.
  /// </summary>
  internal class Set : PieceOwningMediaEntity<Set> {
    [ReferencedField("Name")] public string Act { get; set; }
    [Field] public string Comments { get; set; }
    [PrimaryKeyField] public DateTime Date { get; set; }

    [PrimaryKeyField]
    [ReferencedField("Name")]
    public string Location { get; set; }

    [ReferencedField("Date")] public DateTime Newsletter { get; set; }
    [PrimaryKeyField] public int SetNo { get; set; }

    /// <summary>
    ///   Fetches the Set's Newsletter (i.e. not just the Newsletter date)
    ///   from the database.
    /// </summary>
    /// <returns>
    ///   The Set's Newsletter.
    /// </returns>
    public virtual Newsletter FetchNewsletter() {
      var newsletter = new Newsletter();
      newsletter.Date = Newsletter;
      newsletter.Fetch();
      return newsletter;
    }
  } //End of class
} //End of namespace