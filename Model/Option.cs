using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using SoundExplorers.Data;
using VelocityDb.Session;

namespace SoundExplorers.Model {
  /// <summary>
  ///   Accesses an option/preference for the current user,
  ///   as held on the UserOption table.
  /// </summary>
  public class Option {
    /// <summary>
    ///   Initialises a new instance of the Option class,
    ///   fetching the corresponding UserOption record if it already exists.
    /// </summary>
    /// <param name="name">
    ///   The name that identifies the option relative to the current user.
    /// </param>
    /// <param name="defaultValue">
    ///   Default value for the option if not found on the database.
    ///   If not specified, the default string value will be an empty string,
    ///   the default boolean value will be False
    ///   and the default integer value will be zero.
    /// </param>
    /// <remarks>
    ///   The <see cref="StringValue" /> property will be set to
    ///   the current value of the <b>OptionValue</b> field of the
    ///   <b>UserOption</b> record, if found.
    ///   Otherwise the <see cref="StringValue" /> property will be set to
    ///   to the value of the <paramref name="defaultValue" /> parameter,
    ///   if specified or, failing that, to an empty string.
    /// </remarks>
    /// <exception cref="DataException">
    ///   Thrown if
    ///   there is an error on attempting to access the database.
    /// </exception>
    [ExcludeFromCodeCoverage]
    public Option(string name, object? defaultValue = null) : this(
      QueryHelper.Instance, Global.Session, name, defaultValue) { }

    /// <summary>
    ///   Should only need to be called directly for testing.
    /// </summary>
    protected Option(QueryHelper queryHelper, SessionBase session,
      string name, object? defaultValue = null) {
      QueryHelper = queryHelper;
      OptionName = name;
      Session = session;
      DefaultValue = defaultValue;
      UserOption = FetchUserOption();
    }

    private object? DefaultValue { get; }
    private string OptionName { get; }
    private QueryHelper QueryHelper { get; }
    private SessionBase Session { get; }

    /// <summary>
    ///   Gets or sets the entity that represents the
    ///   data for the UserOption database record.
    /// </summary>
    private UserOption UserOption { get; set; }

    /// <summary>
    ///   Gets or sets the current value of the option as a boolean.
    /// </summary>
    /// <remarks>
    ///   This is initially value of the <b>OptionValue</b> field of the
    ///   corresponding <b>UserOption</b> record, if it exists
    ///   and <b>OptionValue</b> contains a valid integer.
    ///   Otherwise it will initially be False or the default value
    ///   that can optionally be set in the constructor.
    ///   <para>
    ///     When set, the database will be updated unless the there's
    ///     no actual change to the previous value.
    ///     The corresponding <b>UserOption</b> record will be updated if found
    ///     or inserted if not.
    ///   </para>
    /// </remarks>
    public bool BooleanValue {
      get {
        bool unused = bool.TryParse(StringValue, out bool result);
        return result;
      }
      set => StringValue = value.ToString();
    }

    /// <summary>
    ///   Gets or sets the current value of the option as an integer.
    /// </summary>
    /// <remarks>
    ///   This is initially value of the <b>OptionValue</b> field of the
    ///   corresponding <b>UserOption</b> record, if it exists
    ///   and <b>OptionValue</b> contains a valid integer.
    ///   Otherwise it will initially be zero or the default value
    ///   that can optionally be set in the constructor.
    ///   <para>
    ///     When set, the database will be updated unless the there's
    ///     no actual change to the previous value.
    ///     The corresponding <b>UserOption</b> record will be updated if found
    ///     or inserted if not.
    ///   </para>
    /// </remarks>
    public int Int32Value {
      get {
        bool unused = int.TryParse(StringValue, out int result);
        return result;
      }
      set => StringValue = value.ToString();
    }

    /// <summary>
    ///   Gets or sets the current value of the option as a string.
    /// </summary>
    /// <remarks>
    ///   This is initially value of the <b>OptionValue</b> field of the
    ///   corresponding <b>UserOption</b> record, if it exists.
    ///   Otherwise it will initially be blank or the default value
    ///   that can optionally be set in the constructor.
    ///   <para>
    ///     When set, the database will be updated unless the there's
    ///     no actual change to the previous value.
    ///     When set to a value other than a null reference or an empty string,
    ///     the corresponding <b>UserOption</b> record will be updated if found
    ///     or inserted if not.
    ///     When set to a null reference or an empty string,
    ///     the the corresponding <b>UserOption</b> record will be
    ///     will be deleted if found.
    ///   </para>
    /// </remarks>
    public string StringValue {
      get => UserOption.OptionValue;
      set {
        // We need to try to fetch the UserOption from the database again,
        // as it might have been persisted, updated deleted on the database
        // by another procedure.
        // This happens when two concurrently open Editor windows save
        // their positions on being closed.
        UserOption = FetchUserOption();
        if (value == UserOption.OptionValue) {
          return;
        }
        Session.BeginUpdate();
        if (!string.IsNullOrWhiteSpace(value)) {
          UserOption.OptionValue = value;
          if (!UserOption.IsPersistent) {
            Session.Persist(UserOption);
          }
        } else if (UserOption.IsPersistent) {
          Session.Unpersist(UserOption);
          UserOption.OptionValue = value;
        }
        Session.Commit();
      }
    }

    /// <summary>
    ///   Returns the required UserOption from the database or, if not found,
    ///   a new UserOption with the required key and defaulted value.
    /// </summary>
    private UserOption FetchUserOption() {
      var temp = new UserOption {UserId = Environment.UserName, OptionName = OptionName};
      Session.BeginUpdate();
      var result = QueryHelper.Find<UserOption>(temp.SimpleKey, Session);
      if (result == null) {
        result = temp;
        result.OptionValue = !string.IsNullOrWhiteSpace(DefaultValue?.ToString())
          ? DefaultValue.ToString()!
          : string.Empty;
      }
      Session.Commit();
      return result;
    }
  } //End of class
} //End of namespace