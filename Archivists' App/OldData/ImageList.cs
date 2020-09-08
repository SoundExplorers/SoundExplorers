using System;

namespace SoundExplorers.OldData {
  /// <summary>
  ///   A list of Images.
  /// </summary>
  internal class ImageList : EntityList<Image> {
    /// <overloads>
    ///   Initialises a new instance of the <see cref="ImageList" /> class,
    ///   populating its list
    ///   with all the Image records on the database.
    /// </overloads>
    /// <summary>
    ///   Initialises a new instance of the <see cref="ImageList" /> class,
    ///   populating its list
    ///   with all the Image records on the database
    ///   and indicating that a parent entity list is not required.
    /// </summary>
    /// <remarks>
    ///   These two constructors are equivalent.
    ///   But they are both required because this is the top level
    ///   parent table:
    ///   it never needs its own parent
    ///   but when the EntityList contstructor invoked by the child entity list (ArtistInImageList)
    ///   instantiates the parent entity list (ImageList),
    ///   it will do a generic instantiation that will work
    ///   for any parent list and will therefore want to explicity
    ///   pass a null paramenter to the parent EntityList contstructor
    ///   to stop the parent having its own parent,
    ///   as every other potential parent entity list except for this one
    ///   could potentially be a child list.
    /// </remarks>
    public ImageList() : this(null) { }

    /// <summary>
    ///   Initialises a new instance of the <see cref="ImageList" /> class,
    ///   populating its list
    ///   with all the Image records on the database,
    ///   optionally specifying the type of parent entity list
    ///   to include.
    /// </summary>
    /// <param name="parentListType">
    ///   Optionally specifies the type of parent entity list
    ///   to include.  Null if a parent list is not required.
    /// </param>
    public ImageList(Type parentListType = null) { }

    /// <summary>
    ///   An indexer that returns
    ///   the Image at the specified index in the list.
    /// </summary>
    /// <param name="index">
    ///   The zero-based index of the Image to get.
    /// </param>
    /// <returns>
    ///   The Image at the specified index in the list.
    /// </returns>
    public new Image this[int index] => base[index] as Image;
  } //End of class
} //End of namespace