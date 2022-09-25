using System;
using System.Linq;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Model; 

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

  private void DisallowChangeUrlToDuplicate(Newsletter newsletter) {
    if (Url != newsletter.Url) {
      var foundNewsletter = FindNewsletterWithUrl();
      if (foundNewsletter != null && !foundNewsletter.Oid.Equals(newsletter.Oid)) {
        throw Newsletter.CreateDuplicateUrlUpdateException(foundNewsletter);
      }
    }
  }

  private void DisallowInsertionWithDuplicateUrl() {
    var duplicate = FindNewsletterWithUrl();
    if (duplicate != null) {
      throw Newsletter.CreateDuplicateUrlInsertionException(Url, Date, duplicate);
    }
  }

  private Newsletter? FindNewsletterWithUrl() {
    return (from newsletter in EntityList
      where newsletter.Url == Url
      select newsletter).FirstOrDefault();
  }

  protected override string GetSimpleKey() {
    return EntityBase.DateToSimpleKey(Date);
  }

  internal override void ValidateInsertion() {
    base.ValidateInsertion();
    ValidateUrlOnInsertion();
  }

  internal override void ValidatePropertyUpdate(
    string propertyName, Newsletter newsletter) {
    base.ValidatePropertyUpdate(propertyName, newsletter);
    if (propertyName == nameof(Url)) {
      ValidateUrlOnUpdate(newsletter);
    }
  }

  private void ValidateUrlOnInsertion() {
    Newsletter.ValidateUrlFormatOnInsertion(Url);
    DisallowInsertionWithDuplicateUrl();
  }

  private void ValidateUrlOnUpdate(Newsletter newsletter) {
    Newsletter.ValidateUrlFormatOnUpdate(Url, Date);
    DisallowChangeUrlToDuplicate(newsletter);
  }
}