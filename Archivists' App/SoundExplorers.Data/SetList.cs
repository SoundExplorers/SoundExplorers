using JetBrains.Annotations;

namespace SoundExplorers.Data {
  /// <summary>
  ///   A list of Sets.
  /// </summary>
  public class SetList : EntityList<Set> {
    /// <overloads>
    ///   Initialises a new instance of the <see cref="SetList" /> class,
    ///   populating its list
    ///   with all the Set records on the database,
    /// </overloads>
    /// <summary>
    ///   Initialises a new instance of the <see cref="SetList" /> class,
    ///   populating its list
    ///   with all the Set records on the database.
    /// </summary>
    [UsedImplicitly]
    public SetList()
      : base(typeof(PerformanceList)) { }

    /// <summary>
    ///   An indexer that returns
    ///   the Set at the specified index in the list.
    /// </summary>
    /// <param name="index">
    ///   The zero-based index of the Set to get.
    /// </param>
    /// <returns>
    ///   The Set at the specified index in the list.
    /// </returns>
    public new Set this[int index] => base[index] as Set;
  } //End of class
} //End of namespace