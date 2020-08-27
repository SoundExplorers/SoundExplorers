using System;

namespace SoundExplorers.Data {
  /// <summary>
  ///   A list of Sets.
  /// </summary>
  internal class SetList : EntityList<Set> {
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
    public SetList()
      : this(typeof(PerformanceList)) { }

    /// <summary>
    ///   Initialises a new instance of the <see cref="SetList" /> class,
    ///   populating its list
    ///   with all the Set records on the database,
    ///   optionally specifying the type of parent entity list
    ///   to include.
    /// </summary>
    /// <param name="parentListType">
    ///   Optionally specifies the type of parent entity list
    ///   to include.  Null if a parent list is not required.
    /// </param>
    public SetList(Type parentListType = null)
      : base(parentListType) { }

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