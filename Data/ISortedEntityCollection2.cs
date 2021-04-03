using System.Collections.Generic;

namespace SoundExplorers.Data {
  public interface ISortedEntityCollection2 {
    IEntity2 this[Key2 key] { get; }
    int Count { get; }
    IEnumerable<IEntity2> Values { get; }
    void Add(Key2 key, IEntity2 entity);
    bool Contains(Key2 key);
    void Remove(Key2 key);
  }
}