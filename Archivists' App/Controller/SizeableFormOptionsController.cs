using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Controller {
  public class SizeableFormOptionsController {
    public SizeableFormOptionsController(
      [NotNull] IView<SizeableFormOptionsController> view, [NotNull] string formName,
      bool isPositionPersisted) {
      HeightOption = new Option(formName + ".Height");
      WidthOption = new Option(formName + ".Width");
      // Window state will default to Normal (0).
      WindowStateOption = new Option(formName + ".WindowState");
      if (isPositionPersisted) {
        LeftOption = new Option(formName + ".Left");
        TopOption = new Option(formName + ".Top");
      }
      view.SetController(this);
    }

    public int Height {
      get => HeightOption.Int32Value;
      set => HeightOption.Int32Value = value;
    }

    private Option HeightOption { get; }

    public int Left {
      get => LeftOption.Int32Value;
      set => LeftOption.Int32Value = value;
    }

    private Option LeftOption { get; }

    public int Top {
      get => TopOption.Int32Value;
      set => TopOption.Int32Value = value;
    }

    private Option TopOption { get; }

    public int Width {
      get => WidthOption.Int32Value;
      set => WidthOption.Int32Value = value;
    }

    private Option WidthOption { get; }

    public int WindowState {
      get => WindowStateOption.Int32Value;
      set => WindowStateOption.Int32Value = value;
    }

    private Option WindowStateOption { get; }
  }
}