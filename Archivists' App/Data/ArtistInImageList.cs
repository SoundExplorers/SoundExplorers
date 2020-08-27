using System;

namespace SoundExplorers.Data {
  /// <summary>
  ///   A list of ArtistInImages.
  /// </summary>
  internal class ArtistInImageList : EntityList<ArtistInImage> {
    /// <overloads>
    ///   Initialises a new instance of the <see cref="ArtistInImageList" /> class,
    ///   populating its list
    ///   with all the ArtistInImage records on the database.
    /// </overloads>
    /// <summary>
    ///   Initialises a new instance of the <see cref="ArtistInImageList" /> class,
    ///   populating its list
    ///   with all the ArtistInImage records on the database
    ///   and including all the Image records as a parent list.
    /// </summary>
    public ArtistInImageList()
      : this(typeof(ImageList)) { }

    /// <summary>
    ///   Initialises a new instance of the <see cref="ArtistInImageList" /> class,
    ///   populating its list
    ///   with all the ArtistInImage records on the database,
    ///   optionally specifying the type of parent entity list
    ///   to include.
    /// </summary>
    /// <param name="parentListType">
    ///   Optionally specifies the type of parent entity list
    ///   to include.  Null if a parent list is not required.
    /// </param>
    public ArtistInImageList(Type parentListType = null)
      : base(parentListType) { }

    /// <summary>
    ///   An indexer that returns
    ///   the ArtistInImage at the specified index in the list.
    /// </summary>
    /// <param name="index">
    ///   The zero-based index of the ArtistInImage to get.
    /// </param>
    /// <returns>
    ///   The ArtistInImage at the specified index in the list.
    /// </returns>
    public new ArtistInImage this[int index] => base[index] as ArtistInImage;
  } //End of class
} //End of namespace