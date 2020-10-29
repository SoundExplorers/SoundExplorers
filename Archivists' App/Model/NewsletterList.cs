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

    protected override BindingColumnList CreateColumns() {
      return new BindingColumnList {
        new BindingColumn(nameof(Newsletter.Date)),
        new BindingColumn(nameof(Newsletter.Url)) {DisplayName = "URL"}
      };
    }

    [ExcludeFromCodeCoverage]
    public override IList GetChildrenForMainList(int rowIndex) {
      throw new NotSupportedException();
    }
  }
}