using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Project858.Web
{
    /// <summary>
    /// Represents a sitemap item.
    /// </summary>
    public sealed class SitemapItem : ISitemapItem
    {
        #region - Constructor -
        /// <summary>
        /// Creates a new instance of <see cref="SitemapItem"/>
        /// </summary>
        /// <param name="url">URL of the page. Optional.</param>
        /// <param name="lastModified">The date of last modification of the file. Optional.</param>
        /// <param name="changeFrequency">How frequently the page is likely to change. Optional.</param>
        /// <param name="priority">The priority of this URL relative to other URLs on your site. Valid values range from 0.0 to 1.0. Optional.</param>
        /// <param name="images"></param>
        /// <exception cref="System.ArgumentNullException">If the <paramref name="url"/> is null or empty.</exception>
        public SitemapItem(String url, DateTime? lastModified = null, Nullable<SitemapChangeFrequencyTypes> changeFrequency = null, Nullable<Double> priority = null, List<SitemapImage> images = null)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            this.Url = url;
            this.LastModified = lastModified;
            this.ChangeFrequency = changeFrequency;
            this.Priority = priority;
            this.Images = images == null ? null : images.AsReadOnly();
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// URL of the page.
        /// </summary>
        public String Url { get; private set; }
        /// <summary>
        /// The date of last modification of the file.
        /// </summary>
        public DateTime? LastModified { get; private set; }
        /// <summary>
        /// How frequently the page is likely to change.
        /// </summary>
        public Nullable<SitemapChangeFrequencyTypes> ChangeFrequency { get; private set; }
        /// <summary>
        /// The priority of this URL relative to other URLs on your site. Valid values range from 0.0 to 1.0.
        /// </summary>
        public Nullable<Double> Priority { get; private set; }
        /// <summary>
        /// Images collection
        /// </summary>
        public ReadOnlyCollection<SitemapImage> Images { get; private set; }
        #endregion
    }
}
