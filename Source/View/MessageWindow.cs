// Written by Simon O'Rorke, June 2004.

using System.Windows.Forms;
using JetBrains.Annotations;

namespace SoundExplorers.View; 

/// <summary>
///   A window for showing a message,
///   automatically sized to fit the message text and
///   allowing the message text to be copied to the clipboard.
/// </summary>
/// <remarks>
///   You cannot create a new instance of the <b>MessageWindow</b> class.
///   To show the window, call the <b>static</b> method
///   <see cref="O:SoundExplorers.View.MessageWindow.Show">Show</see>.
/// </remarks>
public static class MessageWindow {
	/// <overloads>
	///   Shows a message in a window that is sized to fit the message.
	/// </overloads>
	/// <summary>
	///   Shows a the specified message in a window that is sized to fit the message,
	///   optionally specifying the title to be shown in the windows's title bar.
	/// </summary>
	/// <param name="message">
	///   The message to be shown.
	/// </param>
	/// <param name="title">
	///   Optionally specifies the title to be shown in the window's title bar.
	///   Default: the application product name will be shown in the title bar.
	/// </param>
	/// <remarks>
	///   The window will be shown centred on the primary screen's working area.
	/// </remarks>
	[PublicAPI]
	public static void Show(string message, string? title = null) {
		Show(null, message, title);
	}

	/// <summary>
	///   Shows a the specified message in a window that is sized to fit the message,
	///   optionally specifying owning window
	///   and the title to be shown in the windows's title bar.
	/// </summary>
	/// <param name="owner">
	///   Optionally specifies the window that owns the message window.
	///   Specifying the owner ensures that the message form will be shown centred
	///   on the working area of the screen on which the owning window is shown.
	///   If null, the message window will be shown centred
	///   on the primary screen's working area.
	/// </param>
	/// <param name="message">
	///   The message to be shown.
	/// </param>
	/// <param name="title">
	///   Optionally specifies the title to be shown in the window's title bar.
	///   Default: the application product name will be shown in the title bar.
	/// </param>
	public static void Show(Form? owner, string message,
		string? title = null) {
		var messageForm = new MessageView(message, title ?? Application.ProductName)
			{Owner = owner};
		messageForm.ShowDialog();
	}
} //End of class 
//End of namespace