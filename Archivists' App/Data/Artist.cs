namespace SoundExplorers.Data {
  /// <summary>
  ///   Artist entity.
  /// </summary>
  internal class Artist : PieceOwningMediaEntity<Artist> {
    [Field(3)] public string Comments { get; set; }
    [UniqueKeyField(1)] public string Forename { get; set; }

    [PrimaryKeyField]
    [HiddenField]
    public string Name =>
      // ltrim(rtrim(coalesce(@Forename, '') || ' ' || coalesce(@Surname, ''))),
      ((Forename != null ? Forename : string.Empty)
       + " "
       + (Surname != null ? Surname : string.Empty)).Trim();

    [UniqueKeyField(2)] public string Surname { get; set; }
  } //End of class
} //End of namespace