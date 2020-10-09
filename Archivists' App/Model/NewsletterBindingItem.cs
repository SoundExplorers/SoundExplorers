using System;
using JetBrains.Annotations;

namespace SoundExplorers.Model {
  [NoReorder]
  public class NewsletterBindingItem : BindingItemBase {
    private DateTime _date;
    private Uri _url;

    public DateTime Date {
      get => _date;
      set {
        _date = value;
        OnPropertyChanged(nameof(Date));
      }
    }

    public Uri Url {
      get => _url;
      set {
        _url = value;
        OnPropertyChanged(nameof(Url));
      }
    }
  }
}