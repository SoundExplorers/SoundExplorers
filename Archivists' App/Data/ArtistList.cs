namespace SoundExplorers.Data {
    /// <summary>
    ///   A list of Artists.
    /// </summary>
    internal class ArtistList : EntityList<Artist> {
        /// <summary>
        ///   An indexer that returns
        ///   the Artist at the specified index in the list.
        /// </summary>
        /// <param name="index">
        ///   The zero-based index of the Artist to get.
        /// </param>
        /// <returns>
        ///   The Artist at the specified index in the list.
        /// </returns>
        public new Artist this[int index] => base[index] as Artist;
  } //End of class
} //End of namespace