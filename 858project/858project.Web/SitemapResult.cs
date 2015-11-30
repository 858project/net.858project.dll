using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;

namespace Project858.Web
{
    public sealed class SitemapResult : ActionResult
    {
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="items">Kolekcia poloziek do sitemap</param>
        public SitemapResult(IEnumerable<ISitemapItem> items)
            : this(items, new SitemapGenerator())
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="items">Kolekcia poloziek do sitemap</param>
        /// <param name="generator">Generator na vytvorenie</param>
        public SitemapResult(IEnumerable<ISitemapItem> items, ISitemapGenerator generator)
        {
            if (items == null) 
                throw new ArgumentNullException("items");
             if (generator == null) 
                throw new ArgumentNullException("generator");

            this.m_items = items;
            this.m_generator = generator;
        }

        #region - Variables -
        /// <summary>
        /// Kolekcia itemov ktore chceme pridat do sitemap
        /// </summary>
        private IEnumerable<ISitemapItem> m_items = null;
        /// <summary>
        /// Generator na vytvorenie sitemap
        /// </summary>
        private readonly ISitemapGenerator m_generator = null;
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Spracuje result pre HTTP odpoved
        /// </summary>
        /// <param name="context">ControllerContext</param>
        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;

            response.ContentType = "text/xml";
            response.ContentEncoding = Encoding.UTF8;

            using (var writer = new XmlTextWriter(response.Output))
            {
                writer.Formatting = Formatting.Indented;
                var sitemap = m_generator.GenerateSiteMap(m_items);
                sitemap.WriteTo(writer);
            }
        }
        #endregion
    }
}
