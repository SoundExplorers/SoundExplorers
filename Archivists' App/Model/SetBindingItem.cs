﻿using System;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  [NoReorder]
  public class SetBindingItem : BindingItemBase<Set, SetBindingItem> {
    private DateTime _date;
    private string _location = null!;
    private int _setNo;
    private string? _act;
    private string? _genre;
    private bool _isPublic;
    private string _notes = null!;

    public SetBindingItem() {
      IsPublic = true;
    }

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

    public string? Act {
      get => _act;
      set {
        _act = value;
        OnPropertyChanged(nameof(Act));
      }
    }

    public string? Genre {
      get => _genre;
      set {
        _genre = value;
        OnPropertyChanged(nameof(Genre));
      }
    }

    public bool IsPublic {
      get => _isPublic;
      set {
        _isPublic = value;
        OnPropertyChanged(nameof(IsPublic));
      }
    }

    public string Notes {
      get => _notes;
      set {
        _notes = value;
        OnPropertyChanged(nameof(Notes));
      }
    }

    protected override void CopyValuesToEntityProperties(Set set) {
      base.CopyValuesToEntityProperties(set);
      set.Event = (Event)EntityList.IdentifyingParent!;
    }

    protected override string GetSimpleKey() {
      return EntityBase.IntegerToSimpleKey(SetNo, nameof(SetNo));
    }
  }
}