using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Project858.Web
{
    /// <summary>
    /// Interface generatora sitemap
    /// </summary>
    public interface ISitemapGenerator
    {
        #region - Public Methods -
        /// <summary>
        /// Vygeneruje XML dokument z poloziek sitemap
        /// </summary>
        /// <param name="items">Kolekcia sitemap poloziek</param>
        /// <returns>XML dokument</returns>
        XDocument GenerateSiteMap(IEnumerable<ISitemapItem> items);
        #endregion
    }
}
