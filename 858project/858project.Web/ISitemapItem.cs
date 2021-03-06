﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Project858.Web
{
    /// <summary>
    /// An interface for sitemap items
    /// </summary>
    public interface ISitemapItem
    {
        /// <summary>
        /// URL of the page.
        /// </summary>
        String Url { get; }
        /// <summary>
        /// The date of last modification of the file.
        /// </summary>
        DateTime? LastModified { get; }
        /// <summary>
        /// How frequently the page is likely to change.
        /// </summary>
        Nullable<SitemapChangeFrequencyTypes> ChangeFrequency { get; }
        /// <summary>
        /// The priority of this URL relative to other URLs on your site. Valid values range from 0.0 to 1.0.
        /// </summary>
        Nullable<Double> Priority { get; }
        /// <summary>
        /// Images collection
        /// </summary>
        ReadOnlyCollection<SitemapImage> Images { get; }
    }
}
