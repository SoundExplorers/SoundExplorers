namespace SoundExplorers.View {
  public class ParentGrid : GridBase {
    public ParentGrid() {
      AllowUserToAddRows = false;
      AllowUserToDeleteRows = false;
      MultiSelect = false;
      ReadOnly = true;
    }
  }
}