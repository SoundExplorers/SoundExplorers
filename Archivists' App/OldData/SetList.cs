using JetBrains.Annotations;

namespace SoundExplorers.OldData {
  /// <summary>
  ///   A list of Sets.
  /// </summary>
  public class SetList : EntityList<Set> {
    /// <summary>
    ///   Initialises a new instance of the <see cref="SetList" /> class,
    ///   so that a subsequent call of the Fetch method will populate
    ///   the instance's list with all the Set records on the database.
    /// </summary>
    /// <remarks>
    ///   Used when Set is the main table.
    /// </remarks>
    [UsedImplicitly]
    public SetList()
      : base(typeof(PerformanceList)) { }
    
    /// <summary>
    ///   Initialises a new instance of the <see cref="SetList" /> class,
    ///   so that a subsequent call of the Fetch method will populate
    ///   the instance's list with all the Set records on the database.
    /// </summary>
    /// <remarks>
    ///   Used when Set is the parent table.
    /// </remarks>
    [UsedImplicitly]
    public SetList(object[] dummy = null) { }

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