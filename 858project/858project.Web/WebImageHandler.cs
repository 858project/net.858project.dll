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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Project858;
using Project858.Web;
using System.Drawing.Drawing2D;

namespace Project858.Web
{
    /// <summary>
    /// Handler na spracovanie obrazkov
    /// </summary>
    public sealed class WebImageHandler : IHttpHandler
    {
        #region - Constructors -
        /// <summary>
        /// Static initialize this class
        /// </summary>
        static WebImageHandler()
        {
            WebImageHandler.m_regex = new Regex(@"^((?<subType>\w+)/)?(?<guid>\w{32})(_(?<width>\d+)(x(?<height>\d+))?)?[.](png|jpg|bmp|gif)$");
            WebImageHandler.m_directoryPath = "~/App_Data/Temp/Images/";
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Definuje ci je mozne objekt opakovane pouzit
        /// </summary>
        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region - Private Static Variables -
        /// <summary>
        /// Cesta k temp directory
        /// </summary>
        private static String m_directoryPath = null;
        /// <summary>
        /// Regex na parsovanie dat z obrazku
        /// </summary>
        private static Regex m_regex = null;
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Spracuje request poziadavku na nacitanie obrazku
        /// </summary>
        /// <param name="context">HttpContext</param>
        public void ProcessRequest(HttpContext context)
        {
            //nazov suboru
            String baseUrl = context.Request.RawUrl;
            String file = baseUrl.Replace(WebConfiguration.ImageConfiguration.BasUrl, String.Empty);
            
            Nullable<Guid> key = null;
            Nullable<int> width = null;
            Nullable<int> height = null;
            if (this.internalParseImageUrl(ref file, out key, out width, out height))
            {
                //update file name
                file = !width.HasValue && !height.HasValue ?
                    String.Format("{0}.data", key.Value.ToStringWithoutDash()) :
                    String.Format("{0}_{1}_{2}.data", key.Value.ToStringWithoutDash(), (width.HasValue ? width.Value : 0), (height.HasValue ? height.Value : 0));

                //ziskame format obrazku
                ImageFormat format = this.InternalGetContentType(baseUrl);
                String directory = context.Server.MapPath(WebImageHandler.m_directoryPath);
                file = context.Server.MapPath(String.Format("{0}{1}", WebImageHandler.m_directoryPath, file));
                WebImage image = null;
                if (File.Exists(file))
                {
                    image = new WebImage(System.IO.File.ReadAllBytes(file), format);
                }
                else
                {
                    if (WebConfiguration.OnImageLoadDelegate != null)
                    {
                        image = WebConfiguration.OnImageLoadDelegate(key.Value, format);
                        if (image != null)
                        {
                            if (width.HasValue)
                            {
                                image.Image = image.Image.CropOrResizeImage(width, height, format);
                            }
                            try
                            {
                                if (!Directory.Exists(directory))
                                {
                                    Directory.CreateDirectory(directory);
                                }
                                File.WriteAllBytes(file, image.ImageBlob);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                            }
                        }
                    }
                }
                if (image != null)
                {
                    context.Response.Cache.SetCacheability(HttpCacheability.Public);
                    context.Response.Cache.SetMaxAge(new TimeSpan(2, 0, 0, 0));
                    context.Response.ContentType = image.ContentType;
                    context.Response.BinaryWrite(image.ImageBlob);
                    return;
                }
            }

            context.Response.StatusCode = 404;
            context.Response.StatusDescription = "Image not found !";
        }
        /// <summary>
        /// Resize obrazok na pozadovanu vysku a sirku. Vyska a sirka sa dosiahne bud zmenou velkosti alebo orezanim
        /// </summary>
        /// <param name="image">Image ktory chceme resiznut</param>
        /// <param name="width">Pozadovana sirka obrazku</param>
        /// <param name="height">Pozadovana vyska obrazku</param>
        /// <param name="needToFill">Definuje ci je potrebne obrazok roztiahnut</param>
        /// <returns>Upraveny obrazok</returns>
        public static Image CropOrResizeImage(Image image, Nullable<int> width, Nullable<int> height, bool needToFill)
        {
            int sourceWidth = image.Width;
            int sourceHeight = image.Height;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            //ak nie je vyska zadana
            nPercentW = ((float)width.Value / (float)sourceWidth);
            if (!height.HasValue)
            {
                height = (int)(nPercentW * (float)sourceHeight);
            }
            nPercentH = ((float)height.Value / (float)sourceHeight);

            if (!needToFill)
            {
                if (nPercentH < nPercentW)
                {
                    nPercent = nPercentH;
                }
                else
                {
                    nPercent = nPercentW;
                }
            }
            else
            {
                if (nPercentH > nPercentW)
                {
                    nPercent = nPercentH;
                    destX = (int)Math.Round((width.Value - (sourceWidth * nPercent)) / 2);
                }
                else
                {
                    nPercent = nPercentW;
                    destY = (int)Math.Round((height.Value - (sourceHeight * nPercent)) / 2);
                }
            }

            if (nPercent > 1)
                nPercent = 1;

            int destWidth = (int)Math.Round(sourceWidth * nPercent);
            int destHeight = (int)Math.Round(sourceHeight * nPercent);
            Bitmap bmPhoto = new Bitmap(
                destWidth <= width.Value ? destWidth : width.Value,
                destHeight < height.Value ? destHeight : height.Value,
                PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(bmPhoto))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.DrawImage(image, new Rectangle(destX, destY, destWidth, destHeight), new Rectangle(0, 0, sourceWidth, sourceHeight), GraphicsUnit.Pixel);
            }

            return bmPhoto;
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// Vrati format obrazku
        /// </summary>
        /// <param name="path">Cesta k suboru</param>
        /// <returns>Format obrazku</returns>
        private ImageFormat InternalGetContentType(String path)
        {
            switch (Path.GetExtension(path))
            {
                case ".gif": return ImageFormat.Gif;
                case ".jpg": return ImageFormat.Jpeg;
                case ".png": return ImageFormat.Png;
                default: return ImageFormat.Bmp;
            }
        }
        /// <summary>
        /// Rozparsuje vstupne data z ktorych vyberie guid, pripadne vysku a sirku obrazku
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme parsovat</param>
        /// <param name="key">Jedinecny identifikator obrazku alebo null</param>
        /// <param name="width">sirka obrazku alebo null</param>
        /// <param name="height">Vyska obrazku alebo null;</param>
        /// <returns>True = parsovanie bolo uspesne, inak false</returns>
        private Boolean internalParseImageUrl(ref String value, out Nullable<Guid> key, out Nullable<int> width, out Nullable<int> height)
        {
            //inicialize out
            key = null;
            width = null;
            height = null;
            //small/0ac068e9ae704ffd8c58088714784ffd.jpg
            //vyparsujeme data
            Match match = WebImageHandler.m_regex.Match(value);
            if (match.Success)
            {
                try
                {
                    key = match.Groups["guid"].Value.ToGuidWithoutDash();
                    int int_width = 0;
                    int int_height = 0;
                    if (int.TryParse(match.Groups["width"].Value, out int_width))
                    {
                        width = int_width;
                    }

                    if (int.TryParse(match.Groups["height"].Value, out int_height))
                    {
                        height = int_height;
                    }
                    //ak nie je zadana presna velkost
                    if ((!width.HasValue || width == 0) && (!height.HasValue || height == 0))
                    {
                        String subType = match.Groups["subType"].Value;
                        if (!String.IsNullOrWhiteSpace(subType))
                        {
                            //odstranime subtype
                            value = value.Replace(String.Format("{0}/", subType), String.Empty);
                        }
                        //ak existuje konfiguracia
                        if (WebConfiguration.ImageConfiguration.ContainsKey(subType)) 
                        {                         
                            Size size = WebConfiguration.ImageConfiguration[subType];
                            if (!size.IsEmpty)
                            {
                                width = size.Width;
                                height = size.Height;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    WebUtility.Trace(ex);
                    return false;
                }
                //ak ide o poziadavku na velky obrazok
                if (width == null || width > 1000)
                {
                    width = null;
                }
                if (height == null || height > 1000)
                {
                    height = null;
                }
                if (key != null && !key.Value.IsEmpty())
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
