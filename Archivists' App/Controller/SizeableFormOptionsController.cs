using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  public class SizeableFormOptionsController {
    private Option _heightOption;
    private Option _leftOption;
    private Option _topOption;
    private Option _widthOption;
    private Option _windowStateOption;

    public SizeableFormOptionsController(
      [NotNull] IView<SizeableFormOptionsController> view, [NotNull] string formName) {
      FormName = formName;
      view.SetController(this);
    }

    [NotNull] private string FormName { get; }

    public int Height {
      get => HeightOption.Int32Value;
      set => HeightOption.Int32Value = value;
    }

    [NotNull]
    private Option HeightOption =>
      _heightOption ?? (_heightOption = CreateOption(FormName + ".Height"));

    public int Left {
      get => LeftOption.Int32Value;
      set => LeftOption.Int32Value = value;
    }

    [NotNull]
    private Option LeftOption =>
      _leftOption ?? (_leftOption = CreateOption(FormName + ".Left"));

    public int Top {
      get => TopOption.Int32Value;
      set => TopOption.Int32Value = value;
    }

    [NotNull]
    private Option TopOption =>
      _topOption ?? (_topOption = CreateOption(FormName + ".Top"));

    public int Width {
      get => WidthOption.Int32Value;
      set => WidthOption.Int32Value = value;
    }

    [NotNull]
    private Option WidthOption =>
      _widthOption ?? (_widthOption = CreateOption(FormName + ".Width"));

    public int WindowState {
      get => WindowStateOption.Int32Value;
      set => WindowStateOption.Int32Value = value;
    }

    /// <summary>
    ///   Window state will default to Normal (0).
    /// </summary>
    [NotNull]
    private Option WindowStateOption => _windowStateOption ??
                                        (_windowStateOption =
                                          CreateOption(FormName + ".WindowState"));

    [ExcludeFromCodeCoverage]
    [NotNull]
    protected virtual Option CreateOption([NotNull] string name) {
      return new Option(name);
    }
  }
}