namespace SoundExplorers.Data {
  public interface INamedEntity : IEntity {
    string Name { get; set; }
  }
}