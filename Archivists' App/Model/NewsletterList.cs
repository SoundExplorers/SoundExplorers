using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class NewsletterList : EntityListBase<Newsletter, NewsletterBindingItem> {

    protected override NewsletterBindingItem CreateBindingItem(Newsletter newsletter) {
      return new NewsletterBindingItem {
        Date = newsletter.Date,
        Url = newsletter.Url
      };
    }

    protected override EntityColumnList CreateColumns() {
      return new EntityColumnList {
        new EntityColumn(nameof(Newsletter.Date)),
        new EntityColumn(nameof(Newsletter.Url)) {DisplayName = "URL"}
      };
    }

    [ExcludeFromCodeCoverage]
    public override IList GetChildren(int rowIndex) {
      throw new NotSupportedException();
    }
  }
}