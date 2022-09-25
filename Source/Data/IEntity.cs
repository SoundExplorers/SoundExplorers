namespace SoundExplorers.Data; 

public interface IEntity {
  EntityBase? IdentifyingParent { get; }
  Key Key { get; }
  string SimpleKey { get; }
}