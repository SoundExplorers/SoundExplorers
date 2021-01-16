namespace SoundExplorers.Controller {
  public interface IParentGrid : IGrid, IView<ParentGridController> {
    ParentGridController Controller { get; }
  }
}