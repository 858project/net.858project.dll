using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Project858.Web
{
    /// <summary>
    /// Generator na vytvorenie sitemap
    /// https://github.com/uhaciogullari/SimpleMvcSitemap
    /// </summary>
    public sealed class SitemapGenerator : ISitemapGenerator
    {
        #region - Constants -
        private static readonly XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        private static readonly XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
        private static readonly XNamespace nsImage = "http://www.google.com/schemas/sitemap-image/1.1";
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Vygeneruje Sitemap zo vsetkych poloziek
        /// </summary>
        /// <param name="items">Polozky sitemap</param>
        /// <returns>XML dokument sitemap</returns>
        public XDocument GenerateSiteMap(IEnumerable<ISitemapItem> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            var sitemap = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement(xmlns + "urlset",
                      new XAttribute("xmlns", xmlns),
                      new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                      new XAttribute(XNamespace.Xmlns + "image", nsImage.NamespaceName),
                      new XAttribute(xsi + "schemaLocation", "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd"),
                      from item in items
                        select InternalCreateItemElement(item)
                      )
                 );

            return sitemap;
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// Prida element do sitemap
        /// </summary>
        /// <param name="item">Kolekcia poloziek</param>
        /// <returns>XML element do sitemap</returns>
        private XElement InternalCreateItemElement(ISitemapItem item)
        {
            var itemElement = new XElement(xmlns + "url", new XElement(xmlns + "loc", item.Url.ToLowerInvariant()));

            // all other elements are optional

            if (item.LastModified.HasValue)
                itemElement.Add(new XElement(xmlns + "lastmod", item.LastModified.Value.ToString("yyyy-MM-dd")));

            if (item.ChangeFrequency.HasValue)
                itemElement.Add(new XElement(xmlns + "changefreq", item.ChangeFrequency.Value.ToString().ToLower()));

            if (item.Priority.HasValue)
                itemElement.Add(new XElement(xmlns + "priority", item.Priority.Value.ToString("F1", CultureInfo.InvariantCulture)));

            if (item.Images != null && item.Images.Count > 0)
            {
                foreach (var image in item.Images) 
                {
                    var element = new XElement(nsImage + "image",
                                    new XElement(nsImage + "loc", image.Url));
                    if (!String.IsNullOrWhiteSpace(image.Title)) 
                    {
                        element.Add(new XElement(nsImage + "title", image.Title));
                    }
                    if (!String.IsNullOrWhiteSpace(image.Caption)) 
                    {
                        element.Add(new XElement(nsImage + "caption", image.Caption));
                    }
                    itemElement.Add(element);
                }
            }

            return itemElement;
        }
        #endregion
    }
}
