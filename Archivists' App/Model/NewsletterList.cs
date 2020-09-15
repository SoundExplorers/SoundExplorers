using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class NewsletterList : EntityListBase<Newsletter> {
    public override IList GetChildren(int rowIndex) {
      return (IList)this[rowIndex].Events.Values;
    }

    protected override Newsletter CreateBackupEntity(Newsletter newsletter) {
      return new Newsletter {
        Date = newsletter.Date,
        Url = newsletter.Url
      };
    }

    protected override EntityColumnList CreateColumns() {
      return new EntityColumnList {
        new EntityColumn(nameof(Newsletter.Date), typeof(DateTime)),
        new EntityColumn(nameof(Newsletter.Url), typeof(Uri))
      };
    }

    protected override IList<object> GetRowItemValuesFromEntity(Newsletter newsletter) {
      return new List<object> {newsletter.Date, newsletter.Url};
    }

    protected override void RestoreEntityPropertiesFromBackup(Newsletter backupNewsletter,
      Newsletter newsletterToRestore) {
      newsletterToRestore.Date = backupNewsletter.Date;
      newsletterToRestore.Url = backupNewsletter.Url;
    }

    protected override void UpdateEntityAtRow(DataRow row, Newsletter newsletter) {
      var newDate = (DateTime)row[nameof(Newsletter.Date)];
      var newUrl = (Uri)row[nameof(Newsletter.Url)];
      if (newDate != newsletter.Date) {
        newsletter.Date = newDate;
      }
      if (newUrl != newsletter.Url) {
        newsletter.Url = newUrl;
      }
    }
  }
}