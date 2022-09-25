using SoundExplorers.Data;

namespace SoundExplorers.Model; 

public class SetComparer : DefaultIdentifiedEntityComparer<Set, Event> {
  static SetComparer() {
    EventComparer = new EventComparer();
  }

  public SetComparer() : base(EventComparer) { }
  private static EventComparer EventComparer { get; }
}