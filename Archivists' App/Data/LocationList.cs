namespace SoundExplorers.Data {
    /// <summary>
    ///   A list of Locations.
    /// </summary>
    internal class LocationList : EntityList<Location> {
        /// <summary>
        ///   An indexer that returns
        ///   the Location at the specified index in the list.
        /// </summary>
        /// <param name="index">
        ///   The zero-based index of the Location to get.
        /// </param>
        /// <returns>
        ///   The Location at the specified index in the list.
        /// </returns>
        public new Location this[int index] => base[index] as Location;
  } //End of class
} //End of namespace