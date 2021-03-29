using System.Collections.Generic;

namespace SoundExplorers.Data {
  public interface ISortedEntityCollection {
    IEntity this[Key key] { get; }
    int Count { get; }
    IEnumerable<IEntity> Values { get; }
    void Add(Key key, IEntity entity);
    bool Contains(Key key);
    void Remove(Key key);
  }
}