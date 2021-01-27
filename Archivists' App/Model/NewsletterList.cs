using System;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class NewsletterList : EntityListBase<Newsletter, NewsletterBindingItem> {
    protected override NewsletterBindingItem CreateBindingItem(Newsletter newsletter) {
      return new() {
        Date = newsletter.Date,
        Url = newsletter.Url
      };
    }

    protected override BindingColumnList CreateColumns() {
      return new() {
        new BindingColumn(nameof(Newsletter.Date), typeof(DateTime)) {IsInKey = true},
        new BindingColumn(nameof(Newsletter.Url), typeof(string)) {DisplayName = "URL"}
      };
    }
  }
}