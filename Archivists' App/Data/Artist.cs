namespace SoundExplorers.Data {
  /// <summary>
  ///   Artist entity.
  /// </summary>
  internal class Artist : PieceOwningMediaEntity<Artist> {
    [Field] public string Comments { get; set; }
    [UniqueKeyField] public string Forename { get; set; }

    [PrimaryKeyField]
    [HiddenField]
    public string Name =>
      // ltrim(rtrim(coalesce(@Forename, '') || ' ' || coalesce(@Surname, ''))),
      ((Forename != null ? Forename : string.Empty)
       + " "
       + (Surname != null ? Surname : string.Empty)).Trim();

    [UniqueKeyField] public string Surname { get; set; }

    //#region Private Fields
    //private string _name;
    //private string _forename;
    //private string _surname;
    //#endregion Private Fields
  } //End of class
} //End of namespace