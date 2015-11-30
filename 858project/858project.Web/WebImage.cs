using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Project858.Web
{
    /// <summary>
    /// Web obrazok reprezentovany datami
    /// </summary>
    public sealed class WebImage
    {
        #region - Constructor -
        /// <summary>
        /// Initializet this class
        /// </summary>
        /// <param name="data"></param>
        /// <param name="format">Format obrazku</param>
        public WebImage(Byte[] data, ImageFormat format)
        {
            this.ImageBlob = data;
            this.ImageFormat = format;
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Obrazok
        /// </summary>
        public Image Image
        {
            get { return this.getImage(); }
            set { this.ImageBlob = this.setImage(value); }
        }
        /// <summary>
        /// Jedinecne id obrazku
        /// </summary>
        public Guid IdImage { get; set; }
        /// <summary>
        /// Definuje format obrazku
        /// </summary>
        public ImageFormat ImageFormat
        {
            get;
            private set;
        }
        /// <summary>
        /// Content type obrazku
        /// </summary>
        public String ContentType
        {
            get { return this.ImageFormat.GetMimeType(); }
        }
        /// <summary>
        /// Pole data reprezentujuce obrazok
        /// </summary>
        public byte[] ImageBlob { get; private set; }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// Vrati image
        /// </summary>
        /// <returns>Image alebo null</returns>
        private Image getImage()
        {
            using (MemoryStream stream = new MemoryStream(this.ImageBlob))
            {
                return Image.FromStream(stream);
            }
        }
        /// <summary>
        /// Prekonvertuje obrazok na pole bytov
        /// </summary>
        /// <param name="image">Obrazok ktory chceme konvertovat</param>
        /// <returns>Pole dat</returns>
        private byte[] setImage(Image image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, this.ImageFormat);
                return stream.ToArray();
            }
        }
        #endregion
    }
}