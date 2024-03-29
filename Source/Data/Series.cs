﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace SoundExplorers.Data; 

/// <summary>
///   An entity representing a series of Events.
///   A festival, for example.
/// </summary>
[VelocityDb.Indexing.Index("_simpleKey")]
public class Series : EntityBase, INotablyNamedEntity {
  public const string DefaultName = "";
  private string _notes = null!;

  [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
  public Series() : base(typeof(Series), nameof(Name), null) {
    AllowBlankSimpleKey = true;
    Events = new SortedEntityCollection<Event>();
  }

  public SortedEntityCollection<Event> Events { get; }

  public string Name {
    get => SimpleKey;
    set {
      Update();
      SimpleKey = value;
    }
  }

  public string Notes {
    get => _notes;
    set {
      UpdateNonIndexField();
      _notes = value;
    }
  }

  public static Series CreateDefault() {
    return new Series {
      Name = DefaultName,
      Notes = "Required default"
    };
  }

  protected override ISortedEntityCollection GetChildren(Type childType) {
    return Events;
  }
}