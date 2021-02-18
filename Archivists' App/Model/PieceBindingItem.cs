using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  [NoReorder]
  public class PieceBindingItem : BindingItemBase<Piece, PieceBindingItem> {
    private DateTime _date;
    private string _location = null!;
    private int _setNo;
    private int _pieceNo;
    private string _title = null!;
    private TimeSpan _duration;
    private string _audioUrl = null!;
    private string _videoUrl = null!;
    private string _notes = null!;

    public DateTime Date {
      get => _date;
      set {
        _date = value;
        OnPropertyChanged(nameof(Date));
      }
    }

    public string Location {
      get => _location;
      set {
        _location = value;
        OnPropertyChanged(nameof(Location));
      }
    }

    public int SetNo {
      get => _setNo;
      set {
        _setNo = value;
        OnPropertyChanged(nameof(SetNo));
      }
    }

    public int PieceNo {
      get => _pieceNo;
      set {
        _pieceNo = value;
        OnPropertyChanged(nameof(PieceNo));
      }
    }

    public string Title {
      get => _title;
      set {
        _title = value;
        OnPropertyChanged(nameof(Title));
      }
    }

    public TimeSpan Duration {
      get => _duration;
      set {
        _duration = value;
        OnPropertyChanged(nameof(Duration));
      }
    }

    public string AudioUrl {
      get => _audioUrl;
      set {
        _audioUrl = value;
        OnPropertyChanged(nameof(AudioUrl));
      }
    }
    
    public string VideoUrl {
      get => _videoUrl;
      set {
        _videoUrl = value;
        OnPropertyChanged(nameof(VideoUrl));
      }
    }

    public string Notes {
      get => _notes;
      set {
        _notes = value;
        OnPropertyChanged(nameof(Notes));
      }
    }

    private Set Set { get; set; } = null!;

    protected override IDictionary<string, object?>
      CreateEntityPropertyValueDictionary() {
      Set = (EntityList.IdentifyingParent as Set)!;
      return new Dictionary<string, object?> {
        [nameof(Date)] = Set.Event.Date,
        [nameof(Location)] = Set.Event.Location,
        [nameof(SetNo)] = Set.SetNo,
        [nameof(PieceNo)] = PieceNo,
        [nameof(Title)] = Title,
        [nameof(Duration)] = Duration,
        [nameof(AudioUrl)] = AudioUrl,
        [nameof(VideoUrl)] = VideoUrl,
        [nameof(Notes)] = Notes
      };
    }

    protected override void CopyValuesToEntityProperties(Piece piece) {
      // PieceNo must be set before Set so that Set.Pieces will be in the right sort
      // order.
      piece.PieceNo = PieceNo;
      piece.Set = Set;
      piece.Title = Title;
      // If Duration has not been specified (i.e. is Zero), we want the more meaningful
      // error message that the Piece cannot be added unless Duration is specified.
      // The Piece entity's Duration will already be Zero, as that is a TimeSpan's
      // default value. If we were to attempt to set the Piece entity's Duration to Zero
      // we would get the less meaningful error message that the Duration is out of
      // range.
      if (Duration != TimeSpan.Zero) {
        piece.Duration = Duration;
      }
      piece.AudioUrl = AudioUrl;
      piece.VideoUrl = VideoUrl;
      piece.Notes = Notes;
    }

    protected override string GetSimpleKey() {
      return EntityBase.IntegerToSimpleKey(PieceNo, nameof(PieceNo));
    }
  }
}