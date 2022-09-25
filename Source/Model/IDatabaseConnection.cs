namespace SoundExplorers.Model; 

public interface IDatabaseConnection {
  SchemaUpgradeStatus SchemaUpgradeStatus { get; }
  void Open();
}