/*
The MIT License
Copyright 2012-2015 (c) 858 Project s.r.o. <info@858project.com>

Permission is hereby granted, free of charge, to any person obtaining a
copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to permit
persons to whom the Software is furnished to do so, subject to the
following conditions:

The above copyright notice and this permission notice shall be included
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

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
                //Encoder myEncoder = Encoder.Quality;
                //EncoderParameters parameters = new EncoderParameters(1);
                //EncoderParameter parameter = new EncoderParameter(myEncoder, 80L);
                //parameters.Param[0] = parameter;
                //image.Save(stream, this.InternalGetEncoder(this.ImageFormat), parameters);
                image.Save(stream, this.ImageFormat);
                return stream.ToArray();
            }
        }
        /// <summary>
        /// Vrati code info pre encoder obrazku
        /// </summary>
        /// <param name="format">Format z ktreho chceme ziskat code info</param>
        /// <returns>ImageCodecInfo alebo null</returns>
        private ImageCodecInfo InternalGetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        #endregion
    }
}