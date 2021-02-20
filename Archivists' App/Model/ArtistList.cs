using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class ArtistList  : EntityListBase<Artist, ArtistBindingItem> {
    protected override ArtistBindingItem CreateBindingItem(Artist artist) {
      return new ArtistBindingItem {
        Forename = artist.Forename,
        Surname = artist.Surname,
        Notes = artist.Notes
      };
    }

    protected override BindingColumnList CreateColumns() {
      return new BindingColumnList {
        new BindingColumn(nameof(Artist.Forename), typeof(string)) {IsInKey = true},
        new BindingColumn(nameof(Artist.Surname), typeof(string)) {IsInKey = true},
        new BindingColumn(nameof(Artist.Notes), typeof(string))
      };
    }
  }
}