namespace SoundExplorers.Data {
  public interface ISortedChildList {
    IEntity this[Key key] { get; }
    int Count { get; }
    void Add(Key key, IEntity entity);
    bool Contains(Key key);
    void Remove(Key key);
  }
}