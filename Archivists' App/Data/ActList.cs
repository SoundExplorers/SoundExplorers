namespace SoundExplorers.Data {
    /// <summary>
    ///   A list of Acts.
    /// </summary>
    internal class ActList : EntityList<Act> {
        /// <summary>
        ///   An indexer that returns
        ///   the Act at the specified index in the list.
        /// </summary>
        /// <param name="index">
        ///   The zero-based index of the Act to get.
        /// </param>
        /// <returns>
        ///   The Act at the specified index in the list.
        /// </returns>
        public new Act this[int index] => base[index] as Act;
  } //End of class
} //End of namespace