namespace SoundExplorers.Controller {
	/// <summary>
	///   A simple interface for objects to set message text.
	/// </summary>
	public interface IMessageUpdater {
		/// <summary>
		///   Sets the message text.
		/// </summary>
		/// <param name="message">The new message to display.</param>
		void SetMessage(string message);
  } // end class
} // end namespace