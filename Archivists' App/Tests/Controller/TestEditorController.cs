using System;
using System.Data.Linq;
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
    private TestEditor<TEntity, TBindingItem> _editor;

    public TestEditorController([NotNull] IEditorView view, [NotNull] Type mainListType,
      [NotNull] QueryHelper queryHelper, [NotNull] SessionBase session) :
      base(view, mainListType) {
      QueryHelper = queryHelper;
      Session = session;
    }

    [NotNull]
    public TestEditor<TEntity, TBindingItem> Editor {
      get => _editor ?? throw new NullReferenceException(
        "TestEditorController.Editor is null");
      set => _editor = value;
    }

    public bool AutoValidate { get; set; }

    [NotNull] private QueryHelper QueryHelper { get; }
    [NotNull] private SessionBase Session { get; }

    protected override ChangeAction LastChangeAction => TestUnsupportedLastChangeAction
      ? ChangeAction.None
      : base.LastChangeAction;

    public bool TestUnsupportedLastChangeAction { get; set; }

    public void CreateAndGoToInsertionRow() {
      Editor.AddNew();
      OnMainGridRowEnter(Editor.Count - 1);
    }

    protected override IEntityList CreateEntityList(Type type) {
      var result = base.CreateEntityList(type);
      result.Session = Session;
      return result;
    }

    protected override Option CreateOption(string name) {
      return new TestOption(QueryHelper, Session, name);
    }

    public IEntityList GetMainList() {
      return MainList;
    }

    public override void OnMainGridRowEnter(int rowIndex) {
      if (AutoValidate) {
        if (MainList.HasRowBeenEdited) {
          OnMainGridRowValidated(CurrentRowIndex);
        }
        if (rowIndex == MainList.BindingList.Count) { // New row
          MainList.BindingList.AddNew();
        }
      }
      base.OnMainGridRowEnter(rowIndex);
    }

    public void SetComboBoxCellValue(
      int rowIndex, [NotNull] string columnName, [NotNull] object value) {
      var comboBoxCellController =
        CreateComboBoxCellControllerWithItems(columnName);
      Editor[rowIndex].SetPropertyValue(columnName, value);
      comboBoxCellController.OnCellValueChanged(0, value);
    }

    [NotNull]
    private ComboBoxCellController CreateComboBoxCellControllerWithItems(
      [NotNull] string columnName) {
      var comboBoxCell = new MockView<ComboBoxCellController>();
      var comboBoxCellController =
        new ComboBoxCellController(comboBoxCell, this, columnName);
      comboBoxCellController.GetItems();
      return comboBoxCellController;
    }
  }
}