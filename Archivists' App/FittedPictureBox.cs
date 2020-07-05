using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace SoundExplorers {

    /// <summary>
    /// A picture box control for displaying an image,
    /// which will be resized to fit the picture box 
    /// while retaining image's original aspect ratio.
    /// </summary>
    internal class FittedPictureBox : PictureBox {

        #region Private Fields
        private string _imageLocation;
        #endregion Private Fields

        #region Public Properties
        /// <summary>
        /// Gets or sets the path of the image that is to be displayed 
        /// in the <see cref="FittedPictureBox"/>
        /// and displays the image indicated.
        /// </summary>
        /// <exception cref="ApplicationException">
        /// The specified file is not an image file.
        /// </exception>
        public new virtual string ImageLocation {
            get {
                return _imageLocation;
            }
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
        #endregion Public Properties

        #region Private Properties
        private Bitmap Original;
        #endregion Private Properties

        #region Constructors
        /// <summary>
        /// Initialises a new instance of the 
        /// <see cref="FittedPictureBox"/> class.
        /// </summary>
        public FittedPictureBox() {
        }
        #endregion Constructors

        #region Public Methods
        /// <summary>
        /// Sets the <see cref="ImageLocation"/> to the specified path
        /// and displays the image indicated.
        /// </summary>
        /// <param name="path">
        /// The path of the image that is to be displayed 
        /// in the <see cref="FittedPictureBox"/>.
        /// </param>
        /// <exception cref="ApplicationException">
        /// The specified file is not an image file.
        /// </exception>
        public new virtual void Load(string path) {
            ImageLocation = path;
        }
        #endregion Public Methods

        #region Protected Methods
        /// <summary>
        /// Raises the <see cref="Control.Resize"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            if (Image != null) {
                Image = ResizeImage(Original);
            }
        }
        #endregion Protected Methods

        #region Private Methods
        /// <summary>
        /// Resizes the sepcified image to the required
        /// size.
        /// </summary>
        /// <param name="original">The image to be resized.</param>
        /// <returns>
        /// </returns>
        /// A bitmap containing a resized version of the specified image.
        /// <remarks>
        /// Based on 
        /// "Resizing a Photographic image with GDI+ for .NET"
        /// by Joel Neubeck:
        /// http://www.codeproject.com/KB/GDI-plus/imageresize.aspx
        /// </remarks>
        private Bitmap ResizeImage(Image original) {
            int sourceWidth = original.Width;
            int sourceHeight = original.Height;
            if (original is Bitmap 
            && sourceWidth == ClientSize.Width
            && sourceHeight == ClientSize.Height) {
                return original as Bitmap;
            }
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;
            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            nPercentW = ((float)ClientSize.Width / (float)sourceWidth);
            nPercentH = ((float)ClientSize.Height / (float)sourceHeight);
            if (nPercentH < nPercentW) {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((ClientSize.Width -
                              (sourceWidth * nPercent)) / 2);
            } else {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((ClientSize.Height -
                              (sourceHeight * nPercent)) / 2);
            }
            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);
            Bitmap newBitmap = new Bitmap(
                ClientSize.Width, 
                ClientSize.Height,
                PixelFormat.Format32bppArgb);
            newBitmap.SetResolution(original.HorizontalResolution,
                             original.VerticalResolution);
            Graphics graphics = Graphics.FromImage(newBitmap);
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
        #endregion Private Methods
    }//End of class
}//End of namespace