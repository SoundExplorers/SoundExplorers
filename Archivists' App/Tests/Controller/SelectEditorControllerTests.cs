using NUnit.Framework;
using SoundExplorers.Controller;
using SoundExplorers.Model;

namespace SoundExplorers.Tests.Controller {
  [TestFixture]
  public class SelectEditorControllerTests {
    [Test]
    public void InitiallySelectedTable() {
      const string initiallySelectedTableName = "Genre";
      var view = new MockView<SelectEditorController>();
      var controller = new SelectEditorController(view, initiallySelectedTableName);
      Assert.AreSame(controller, view.Controller, "view.Controller");
      Assert.AreEqual(typeof(GenreList), controller.SelectedEntityListType,
        "SelectedEntityListType");
      Assert.AreEqual(initiallySelectedTableName, controller.SelectedTableName,
        "SelectedTableName");
    }
    
    [Test]
    public void NoInitiallySelectedTable() {
      var view = new MockView<SelectEditorController>();
      var controller = new SelectEditorController(view, "unrecognised");
      Assert.IsNull(controller.SelectedEntityListType, "SelectedEntityListType");
      Assert.IsEmpty(controller.SelectedTableName, "SelectedTableName");
    }
  }
}