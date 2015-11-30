using System;
using System.Collections.Generic;
using System.Text;

namespace Project858
{
    /// <summary>
    /// Definuje typ inputu ktory sa aplikuje na overenie zadaneho textu
    /// </summary>
    public enum UserInputTypes
    {
        /// <summary>
        /// Ziadnay typ. Ocakava sa len pozadovana dlzka
        /// </summary>
        None,
        /// <summary>
        /// Iba alpha znaky
        /// </summary>
        Alpha,
        /// <summary>
        /// Iba ciselne znaky
        /// </summary>
        Numeric,
        /// <summary>
        /// Alpha numericke znaky. Alphanumericke aj velke aj male znaky
        /// </summary>
        Alphanumeric,
        /// <summary>
        /// Alpha numericke znaky. Alphanumericke iba male znaky
        /// </summary>
        AlphanumericSmall
    }
}
