namespace SoundExplorers.Model {
  public interface IDatabaseConnection {
    bool MustBackup { get; }
    void Open();
  }
}