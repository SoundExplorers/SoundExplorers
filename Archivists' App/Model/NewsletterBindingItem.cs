using System;
using JetBrains.Annotations;

namespace SoundExplorers.Model {
  [NoReorder]
  public class NewsletterBindingItem : BindingItemBase<NewsletterBindingItem> {
    private DateTime _date;
    private string _url;

    public NewsletterBindingItem() {
      Date = DateTime.Today;
    }

    public DateTime Date {
      get => _date;
      set {
        _date = value;
        OnPropertyChanged(nameof(Date));
      }
    }

    public string Url {
      get => _url;
      set {
        _url = value;
        OnPropertyChanged(nameof(Url));
      }
    }
  }
}