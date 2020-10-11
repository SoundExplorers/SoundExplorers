using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class NewsletterList : EntityListBase<Newsletter, NewsletterBindingItem> {
    protected override NewsletterBindingItem CreateBackupBindingItem(
      NewsletterBindingItem bindingItem) {
      return new NewsletterBindingItem {
        Date = bindingItem.Date,
        Url = bindingItem.Url
      };
    }

    protected override Newsletter CreateBackupEntity(Newsletter newsletter) {
      return new Newsletter {
        Date = newsletter.Date,
        Url = newsletter.Url
      };
    }

    protected override NewsletterBindingItem CreateBindingItem(Newsletter newsletter) {
      return new NewsletterBindingItem {
        Date = newsletter.Date,
        Url = newsletter.Url
      };
    }

    protected override EntityColumnList CreateColumns() {
      return new EntityColumnList {
        new EntityColumn(nameof(Newsletter.Date)),
        new EntityColumn(nameof(Newsletter.Url))
      };
    }

    [ExcludeFromCodeCoverage]
    public override IList GetChildren(int rowIndex) {
      throw new NotSupportedException();
    }

    protected override void RestoreBindingItemPropertiesFromBackup(
      NewsletterBindingItem backupBindingItem,
      NewsletterBindingItem bindingItemToRestore) {
      bindingItemToRestore.Date = backupBindingItem.Date;
      bindingItemToRestore.Url = backupBindingItem.Url;
    }

    protected override void RestoreEntityPropertiesFromBackup(Newsletter backupNewsletter,
      Newsletter newsletterToRestore) {
      newsletterToRestore.Date = backupNewsletter.Date;
      newsletterToRestore.Url = backupNewsletter.Url;
    }

    protected override void UpdateEntity(NewsletterBindingItem bindingItem,
      Newsletter newsletter) {
      newsletter.Date = bindingItem.Date;
      newsletter.Url = bindingItem.Url;
    }

    protected override void UpdateEntityProperty(string propertyName, object newValue,
      Newsletter newsletter) {
      switch (propertyName) {
        case nameof(newsletter.Date):
          newsletter.Date = (DateTime?)newValue ??
                            throw new NullReferenceException(nameof(newsletter.Date));
          break;
        case nameof(newsletter.Url):
          newsletter.Url = newValue != null
            ? newValue.ToString()
            : throw new NullReferenceException(nameof(newsletter.Url));
          break;
        default:
          throw new ArgumentException(
            $"{nameof(propertyName)} '{propertyName}' is not supported.",
            nameof(propertyName));
      }
    }
  }
}