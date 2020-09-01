using System;
using System.Data;
using Devart.Data.PostgreSql;

namespace SoundExplorers.Data {
  /// <summary>
  ///   A list of Performances.
  /// </summary>
  public class PerformanceList : EntityList<Performance> {
    /// <overloads>
    ///   Initialises a new instance of the <see cref="PerformanceList" /> class,
    ///   populating its list
    ///   with all the Performance records on the database.
    /// </overloads>
    /// <summary>
    ///   Initialises a new instance of the <see cref="PerformanceList" /> class,
    ///   populating its list
    ///   with all the Performance records on the database
    ///   and indicating that a parent entity list is not required.
    /// </summary>
    /// <remarks>
    ///   These two constructors are equivalent.
    ///   But they are both required because this is the top level
    ///   parent table:
    ///   it never needs its own parent
    ///   but when the EntityList contstructor invoked by the child entity list (SetList)
    ///   instantiates the parent entity list (PerformanceList),
    ///   it will do a generic instantiation that will work
    ///   for any parent list and will therefore want to explicity
    ///   pass a null paramenter to the parent EntityList contstructor
    ///   to stop the parent having its own parent,
    ///   as every other potential parent entity list except for this one
    ///   could potentially be a child list.
    /// </remarks>
    public PerformanceList() : this(null) { }

    /// <summary>
    ///   Initialises a new instance of the <see cref="PerformanceList" /> class,
    ///   populating its list
    ///   with all the Performance records on the database,
    ///   optionally specifying the type of parent entity list
    ///   to include.
    /// </summary>
    /// <param name="parentListType">
    ///   Optionally specifies the type of parent entity list
    ///   to include.  Null if a parent list is not required.
    /// </param>
    public PerformanceList(Type parentListType = null) { }

    public PerformanceList(
      Type parentListType = null, string location = null)
      : base(selectCommand: CreateSelectCommand(location)) { }

    /// <summary>
    ///   An indexer that returns
    ///   the Performance at the specified index in the list.
    /// </summary>
    /// <param name="index">
    ///   The zero-based index of the Performance to get.
    /// </param>
    /// <returns>
    ///   The Performance at the specified index in the list.
    /// </returns>
    public new Performance this[int index] => base[index] as Performance;

    private static SelectCommand<Performance> CreateSelectCommand(
      string location) {
      var selectCommand = new SelectCommand<Performance>();
      if (location != null) {
        selectCommand.CommandText =
          SqlHelper.GetSql("Select Performances for Location.sql");
        var parameter = new PgSqlParameter();
        parameter.ParameterName = "@Location";
        parameter.Value = location;
        parameter.DbType = DbType.String;
        selectCommand.Parameters.Add(parameter);
      }
      return selectCommand;
    }
  } //End of class
} //End of namespace