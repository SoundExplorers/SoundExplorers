namespace SoundExplorers.Data {
  /// <summary>
  ///   A list of Newsletters.
  /// </summary>
  internal class NewsletterList : EntityList<Newsletter> {
    /// <summary>
    ///   An indexer that returns
    ///   the Newsletter at the specified index in the list.
    /// </summary>
    /// <param name="index">
    ///   The zero-based index of the Newsletter to get.
    /// </param>
    /// <returns>
    ///   The Newsletter at the specified index in the list.
    /// </returns>
    public new Newsletter this[int index] => base[index] as Newsletter;
  } //End of class
} //End of namespace