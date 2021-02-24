using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    private string _duration = null!;
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

    public string Duration {
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
    private SetList SetList => (EntityList as PieceList)!.SetList;

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
      // PieceNo must be set before Set so that Set.Pieces will have the correct key for
      // the Piece and therefore be in the right sort order.
      piece.PieceNo = PieceNo;
      piece.Set = Set;
      piece.Title = Title;
      // If Duration has not been specified (i.e. is Zero), we want the more meaningful
      // error message that the Piece cannot be added unless Duration is specified.
      // The Piece entity's Duration will already be Zero, as that is a TimeSpan's
      // default value. If we were to attempt to set the Piece entity's Duration to Zero
      // we would get the less meaningful error message that the Duration is out of
      // range.
      string durationString = GetPropertyValue(nameof(Duration))?.ToString()!;
      var durationTimeSpan = ToDurationTimeSpan(durationString);
      if (durationTimeSpan != TimeSpan.Zero) {
        piece.Duration = durationTimeSpan;
      }
      piece.AudioUrl = AudioUrl;
      piece.VideoUrl = VideoUrl;
      piece.Notes = Notes;
    }

    private void DisallowChangeAudioUrlToDuplicate(Piece piece) {
      if (AudioUrl != piece.AudioUrl &&
          !string.IsNullOrWhiteSpace(AudioUrl)) {
        var foundPiece = FindPieceWithAudioUrl(AudioUrl);
        if (foundPiece != null && !foundPiece.Oid.Equals(piece.Oid)) {
          throw Piece.CreateDuplicateAudioUrlUpdateException(foundPiece);
        }
      }
    }

    private void DisallowChangeVideoUrlToDuplicate(Piece piece) {
      if (VideoUrl != piece.VideoUrl &&
          !string.IsNullOrWhiteSpace(VideoUrl)) {
        var foundPiece = FindPieceWithVideoUrl(VideoUrl);
        if (foundPiece != null && !foundPiece.Oid.Equals(piece.Oid)) {
          throw Piece.CreateDuplicateVideoUrlUpdateException(foundPiece);
        }
      }
    }

    private void DisallowInsertWithDuplicateAudioUrl() {
      if (!string.IsNullOrWhiteSpace(AudioUrl)) {
        var duplicate = FindPieceWithAudioUrl(AudioUrl);
        if (duplicate != null) {
          throw Piece.CreateDuplicateAudioUrlInsertException(CreateKey(), duplicate); 
        }
      }
    }

    private void DisallowInsertWithDuplicateVideoUrl() {
      if (!string.IsNullOrWhiteSpace(VideoUrl)) {
        var duplicate = FindPieceWithVideoUrl(VideoUrl);
        if (duplicate != null) {
          throw Piece.CreateDuplicateVideoUrlInsertException(CreateKey(), duplicate); 
        }
      }
    }

    internal override void ValidateInsertion() {
      base.ValidateInsertion();
      DisallowInsertWithDuplicateAudioUrl();
      DisallowInsertWithDuplicateVideoUrl();
    }

    internal override void ValidatePropertyUpdate(string propertyName,
      Piece piece) {
      base.ValidatePropertyUpdate(propertyName, piece);
      switch (propertyName) {
        case nameof(Piece.AudioUrl):
          DisallowChangeAudioUrlToDuplicate(piece);
          break;
        case nameof(Piece.VideoUrl):
          DisallowChangeVideoUrlToDuplicate(piece);
          break;
      }
    }

    private Piece? FindPieceWithAudioUrl(string audioUrl) {
      return SetList
        .Select(set => (
            from piece in set.Pieces.Values
            where piece.AudioUrl == audioUrl
            select piece)
          .FirstOrDefault()).FirstOrDefault(foundPiece => foundPiece != null);
    }

    private Piece? FindPieceWithVideoUrl(string videoUrl) {
      return SetList
        .Select(set => (
            from piece in set.Pieces.Values
            where piece.VideoUrl == videoUrl
            select piece)
          .FirstOrDefault()).FirstOrDefault(foundPiece => foundPiece != null);
    }

    protected override object? GetEntityPropertyValue(PropertyInfo property,
      PropertyInfo entityProperty) {
      if (property.Name != nameof(Duration)) {
        return base.GetEntityPropertyValue(property, entityProperty);
      }
      string durationString = GetPropertyValue(nameof(Duration))!.ToString()!;
      return ToDurationTimeSpan(durationString);
    }

    protected override string GetSimpleKey() {
      return EntityBase.IntegerToSimpleKey(PieceNo, nameof(PieceNo));
    }

    internal static TimeSpan ToDurationTimeSpan(string value) {
      if (string.IsNullOrWhiteSpace(value)) {
        return TimeSpan.Zero;
      }
      bool isValid = TimeSpan.TryParseExact(
        value.Trim(), new[] {"h\\:m\\:s", "m\\:s"},
        null, out var duration);
      if (isValid) {
        return duration;
      }
      throw new PropertyConstraintException(
        Piece.DurationErrorMessage + Environment.NewLine +
        "Accepted formats" + Environment.NewLine +
        "    minutes:seconds" + Environment.NewLine +
        "        Examples" + Environment.NewLine +
        "            1:2 or 01:02 or 1:02 or 12:34 but not 62:34" +
        Environment.NewLine +
        "    hours:minutes:seconds" + Environment.NewLine +
        "        Examples" + Environment.NewLine +
        "            0:1:2 or 1:2:3 or 01:02:03 or 1:02:34 or 1:23:04 or 1:23:45",
        nameof(Duration));
    }
  }
}