using System;
using System.Data;
using SoundExplorers.Data;
using VelocityDb.Session;

namespace SoundExplorers.Model {
  /// <summary>
  ///   Accesses an option/preference for the current user,
  ///   as held on the UserOption table.
  /// </summary>
  public class Option {
    private QueryHelper _queryHelper;
    private SessionBase _session;

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
    ///   The <see cref="Name" /> property will be set to the value of the
    ///   <paramref name="name" /> parameter.
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
    public Option(string name, object defaultValue = null) {
      DefaultValue = defaultValue;
      var temp = new UserOption {UserId = Environment.UserName, OptionName = name};
      UserOption = QueryHelper.Find<UserOption>(temp.SimpleKey, Session);
      if (UserOption == null) {
        UserOption = temp;
        if (DefaultValue != null && DefaultValue.ToString() != string.Empty) {
          UserOption.OptionValue = DefaultValue.ToString();
        } else {
          UserOption.OptionValue = string.Empty;
        }
      }
    }

    internal QueryHelper QueryHelper {
      get => _queryHelper ?? (_queryHelper = Global.QueryHelper);
      set => _queryHelper = value;
    }

    internal SessionBase Session {
      get => _session ?? (_session = Global.Session);
      set => _session = value;
    }

    /// <summary>
    ///   Gets or sets the entity that represents the
    ///   data for the UserOption database record.
    /// </summary>
    private UserOption UserOption { get; }

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
        try {
          return bool.Parse(StringValue);
        } catch (FormatException) {
          if (DefaultValue != null) {
            return (bool)DefaultValue;
          }
          return false;
        }
      }
      set => StringValue = value.ToString();
    }

    /// <summary>
    ///   Default value.
    /// </summary>
    private object DefaultValue { get; }

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
        try {
          return int.Parse(StringValue);
        } catch (FormatException) {
          if (DefaultValue != null) {
            return (int)DefaultValue;
          }
          return 0;
        }
      }
      set => StringValue = value.ToString();
    }

    /// <summary>
    ///   Gets the name that identifies the option relative to the current user.
    /// </summary>
    /// <remarks>
    ///   This is specified via the constructor.
    ///   It represents value to which the <b>OptionName</b> field of the
    ///   <b>UserOption</b> record on the database will be set.
    /// </remarks>
    public string Name => UserOption.OptionName;

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
        if (value == UserOption.OptionValue) {
          return;
        }
        if (!string.IsNullOrEmpty(value)) {
          UserOption.OptionValue = value;
          if (!UserOption.IsPersistent) {
            Session.Persist(UserOption);
          }
        } else if (UserOption.IsPersistent) {
          Session.Unpersist(UserOption);
          UserOption.OptionValue = value;
        }
      }
    }
  } //End of class
} //End of namespace