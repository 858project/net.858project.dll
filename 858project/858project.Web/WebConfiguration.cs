using Project858.ComponentModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace Project858.Web
{
    /// <summary>
    /// Konfiguracia web sluzby a jej dodatocnych funkcii
    /// </summary>
    public static class WebConfiguration
    {
        #region - Constructors -
        /// <summary>
        /// Static initialize this class
        /// </summary>
        static WebConfiguration()
        {
            WebConfiguration.ImageConfiguration = new WebImageConfiguration()
            {
                BasUrl = "/images/"
            };
        }
        #endregion

        #region - Static Class -
        /// <summary>
        /// Konfiguracia obrazkov
        /// </summary>
        public class WebImageConfiguration : Dictionary<String, Size>
        {
            #region - Properties -
            /// <summary>
            /// zakladna url na pristup k obrazkom
            /// </summary>
            public String BasUrl { get; set; }
            #endregion
        }
        #endregion

        #region - Public Delegate -
        /// <summary>
        /// Vrati spravu popisujucu response stav
        /// </summary>
        /// <param name="type">Response stav</param>
        /// <returns>Sprava, popis stavu</returns>
        public delegate String ResponseMessageDelegate(ResponseTypes type);
        /// <summary>
        /// Delegat na nacitanie obrazku
        /// </summary>
        /// <param name="id">Id obrazku ktory chceme nacitat</param>
        /// <param name="format">Format obrazku</param>
        /// <returns>Obrazok alebo null</returns>
        public delegate WebImage ImageLoadDelegate(Guid id, ImageFormat format);
        /// <summary>
        /// Delegat na nacitanie suboru
        /// </summary>
        /// <param name="id">Id suboru ktory chceme nacitat</param>
        /// <returns>Suboru alebo null</returns>
        public delegate FileModelBase FileLoadDelegate(Guid id);
        #endregion

        #region - Static Properties -
        /// <summary>
        /// Konfiguracia obrazkov
        /// </summary>
        public static WebImageConfiguration ImageConfiguration { get; set; }
        /// <summary>
        /// Delegat na nacitanie spravy, popisu response stavu
        /// </summary>
        public static WebConfiguration.ResponseMessageDelegate OnResponseMessageDelegate { get; set; }
        /// <summary>
        /// Delegat na nacitanie obrazku
        /// </summary>
        public static WebConfiguration.ImageLoadDelegate OnImageLoadDelegate { get; set; }
        /// <summary>
        /// Delegat na nacitanie suboru
        /// </summary>
        public static WebConfiguration.FileLoadDelegate OnFileLoadDelegate { get; set; }
        #endregion
    }
}
