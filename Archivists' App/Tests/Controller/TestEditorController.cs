using System;
using JetBrains.Annotations;
using SoundExplorers.Controller;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Model;
using VelocityDb.Session;

namespace SoundExplorers.Tests.Controller {
  internal class TestEditorController<TEntity, TBindingItem> : EditorController
    where TEntity : EntityBase, new()
    where TBindingItem : BindingItemBase<TEntity, TBindingItem>, new() {
    public TestEditorController(
      [NotNull] Type mainListType, [NotNull] TestMainGridController mainGridController,
      [NotNull] QueryHelper queryHelper, [NotNull] SessionBase session) :
      base(mainGridController.MockEditorView, mainListType,
        new TestMainController(new MockView<MainController>(), queryHelper, session)) {
      MainGridController = mainGridController;
      QueryHelper = queryHelper;
      Session = session;
    }

    [NotNull]
    public TypedBindingList<TEntity, TBindingItem> TypedBindingList =>
      ((EntityListBase<TEntity, TBindingItem>)MainList).TypedBindingList;

    // public new MainController MainController => base.MainController;
    // public new IEntityList MainList => base.MainList;
    [NotNull] private MockEditorView MockEditorView => MainGridController.MockEditorView;
    [NotNull] private TestMainGridController MainGridController { get; }
    [NotNull] private QueryHelper QueryHelper { get; }
    [NotNull] private SessionBase Session { get; }

    public void CreateAndGoToInsertionRow() {
      TypedBindingList.AddNew();
      MockEditorView.MainGridController.OnRowEnter(TypedBindingList.Count - 1);
    }

    protected override IEntityList CreateEntityList(Type type) {
      var result = base.CreateEntityList(type);
      result.Session = Session;
      return result;
    }

    protected override Option CreateOption(string name) {
      return new TestOption(QueryHelper, Session, name);
    }

    public void SetComboBoxCellValue(
      int rowIndex, [NotNull] string columnName, [NotNull] object value) {
      var comboBoxCellController =
        CreateComboBoxCellControllerWithItems(columnName);
      TypedBindingList[rowIndex].SetPropertyValue(columnName, value);
      comboBoxCellController.OnCellValueChanged(0, value);
    }

    [NotNull]
    private ComboBoxCellController CreateComboBoxCellControllerWithItems(
      [NotNull] string columnName) {
      var comboBoxCell = new MockView<ComboBoxCellController>();
      var comboBoxCellController =
        new ComboBoxCellController(comboBoxCell, MainGridController, columnName);
      comboBoxCellController.GetItems();
      return comboBoxCellController;
    }
  }
}