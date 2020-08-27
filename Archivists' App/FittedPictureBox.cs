using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace SoundExplorers {
  /// <summary>
  ///   A picture box control for displaying an image,
  ///   which will be resized to fit the picture box
  ///   while retaining image's original aspect ratio.
  /// </summary>
  internal class FittedPictureBox : PictureBox {
    private string _imageLocation;
    private Bitmap Original;

    /// <summary>
    ///   Gets or sets the path of the image that is to be displayed
    ///   in the <see cref="FittedPictureBox" />
    ///   and displays the image indicated.
    /// </summary>
    /// <exception cref="ApplicationException">
    ///   The specified file is not an image file.
    /// </exception>
    public new virtual string ImageLocation {
      get => _imageLocation;
      set {
        _imageLocation = value;
        try {
          Original = new Bitmap(_imageLocation);
        } catch (ArgumentException) {
          throw new ApplicationException(
            "\"" + _imageLocation + "\" is not an image file.");
        }
        Image = ResizeImage(Original);
      }
    }

    /// <summary>
    ///   Sets the <see cref="ImageLocation" /> to the specified path
    ///   and displays the image indicated.
    /// </summary>
    /// <param name="path">
    ///   The path of the image that is to be displayed
    ///   in the <see cref="FittedPictureBox" />.
    /// </param>
    /// <exception cref="ApplicationException">
    ///   The specified file is not an image file.
    /// </exception>
    public new virtual void Load(string path) {
      ImageLocation = path;
    }

    /// <summary>
    ///   Raises the <see cref="Control.Resize" /> event.
    /// </summary>
    /// <param name="e">
    ///   An <see cref="EventArgs" /> that contains the event data.
    /// </param>
    protected override void OnResize(EventArgs e) {
      base.OnResize(e);
      if (Image != null) {
        Image = ResizeImage(Original);
      }
    }

    /// <summary>
    ///   Resizes the sepcified image to the required
    ///   size.
    /// </summary>
    /// <param name="original">The image to be resized.</param>
    /// <returns>
    /// </returns>
    /// A bitmap containing a resized version of the specified image.
    /// <remarks>
    ///   Based on
    ///   "Resizing a Photographic image with GDI+ for .NET"
    ///   by Joel Neubeck:
    ///   http://www.codeproject.com/KB/GDI-plus/imageresize.aspx
    /// </remarks>
    private Bitmap ResizeImage(Image original) {
      int sourceWidth = original.Width;
      int sourceHeight = original.Height;
      if (original is Bitmap
          && sourceWidth == ClientSize.Width
          && sourceHeight == ClientSize.Height) {
        return original as Bitmap;
      }
      var sourceX = 0;
      var sourceY = 0;
      var destX = 0;
      var destY = 0;
      float nPercent = 0;
      float nPercentW = 0;
      float nPercentH = 0;
      nPercentW = ClientSize.Width / (float)sourceWidth;
      nPercentH = ClientSize.Height / (float)sourceHeight;
      if (nPercentH < nPercentW) {
        nPercent = nPercentH;
        destX = Convert.ToInt16((ClientSize.Width -
                                 sourceWidth * nPercent) / 2);
      } else {
        nPercent = nPercentW;
        destY = Convert.ToInt16((ClientSize.Height -
                                 sourceHeight * nPercent) / 2);
      }
      var destWidth = (int)(sourceWidth * nPercent);
      var destHeight = (int)(sourceHeight * nPercent);
      var newBitmap = new Bitmap(
        ClientSize.Width,
        ClientSize.Height,
        PixelFormat.Format32bppArgb);
      newBitmap.SetResolution(original.HorizontalResolution,
        original.VerticalResolution);
      var graphics = Graphics.FromImage(newBitmap);
      graphics.Clear(Color.Black);
      graphics.InterpolationMode =
        InterpolationMode.HighQualityBicubic;
      graphics.DrawImage(
        original,
        new Rectangle(destX, destY, destWidth, destHeight),
        new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
        GraphicsUnit.Pixel);
      graphics.Dispose();
      return newBitmap;
    }
  } //End of class
} //End of namespace