using System;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  [NoReorder]
  public class NewsletterBindingItem
    : BindingItemBase<Newsletter, NewsletterBindingItem> {
    private DateTime _date;
    private string _url = null!;

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

    protected override string GetSimpleKey() {
      return EntityBase.DateToSimpleKey(Date);
    }
  }
}