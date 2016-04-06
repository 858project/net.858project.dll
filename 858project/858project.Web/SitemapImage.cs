using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project858.Web
{
    /// <summary>
    /// Sitemap image item
    /// </summary>
    public sealed class SitemapImage
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="url">The URL of the image.</param>
        /// <param name="title">The title of the image.</param>
        /// <param name="caption">The caption of the image.</param>
        /// <exception cref="System.ArgumentNullException">If the <paramref name="url"/> is null or empty.</exception>
        public SitemapImage(String url, String title = null, String caption = null)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            this.Url = url;
            this.Title = title;
            this.Caption = caption;
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// The URL of the image.
        /// </summary>
        public String Url { get; protected set; }
        /// <summary>
        /// The caption of the image.
        /// </summary>
        public String Caption { get; protected set; }
        /// <summary>
        /// The title of the image.
        /// </summary>
        public String Title { get; protected set; }
        #endregion
    }
}
