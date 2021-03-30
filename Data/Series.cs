﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity representing a series of Events.
  ///   A festival, for example.
  /// </summary>
  public class Series : EntityBase, INotablyNamedEntity {
    public const string DefaultName = "";
    private string _notes = null!;

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public Series(SortedEntityCollection<Series> root) : base(
      root,typeof(Series), nameof(Name), null) {
      AllowBlankSimpleKey = true;
      Events = new SortedEntityCollection<Event>();
    }

    public SortedEntityCollection<Event> Events { get; }

    public string Name {
      get => SimpleKey;
      set {
        UpdateNonIndexField();
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

    public static Series CreateDefault(SortedEntityCollection<Series> root) {
      return new Series(root) {
        Name = DefaultName,
        Notes = "Required default"
      };
    }

    protected override ISortedEntityCollection GetChildren(Type childType) {
      return Events;
    }
  }
}