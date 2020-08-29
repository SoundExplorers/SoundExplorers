namespace SoundExplorers.Data {
  /// <summary>
  ///   A list of Serieses.
  /// </summary>
  internal class SeriesList : EntityList<Series> {
    /// <summary>
    ///   An indexer that returns
    ///   the Series at the specified index in the list.
    /// </summary>
    /// <param name="index">
    ///   The zero-based index of the Series to get.
    /// </param>
    /// <returns>
    ///   The Series at the specified index in the list.
    /// </returns>
    public new Series this[int index] => base[index] as Series;
  } //End of class
} //End of namespace