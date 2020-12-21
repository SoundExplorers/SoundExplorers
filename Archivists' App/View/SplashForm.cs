namespace SoundExplorers.View {
  
  public partial class SplashForm : SplashFormBase {
    public SplashForm() {
      // When migrating the application to .Net 5.0,
      // the picture was removed from the splash window to avoid a new behaviour
      // where, instead of the picture, there would sometimes momentarily be 
      // a hole in the window on loading.
      //
      // Required for Windows Form Designer support
      InitializeComponent();
      // Add any constructor code after InitializeComponent call
    }
  } //End of class
} //End of namespace