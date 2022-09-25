namespace SoundExplorers.Model; 

public interface IDatabaseConfig {
  string ConfigFilePath { get; }
  string DatabaseFolderPath { get; }
  void Load();
}