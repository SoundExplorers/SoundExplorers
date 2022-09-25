namespace SoundExplorers.Data; 

public interface INotablyNamedEntity : INamedEntity {
  string Notes { get; set; }
}