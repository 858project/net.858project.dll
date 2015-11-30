using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project858.Web
{
    /// <summary>
    /// Definuje dostupne typy jazykov
    /// </summary>
    public enum LanguageTypes : byte
    {
        /// <summary>
        /// Neznamy jazyk
        /// </summary>
        Unknow = 0,
        /// <summary>
        /// Slovencina
        /// </summary>
        [LanguageAttribute(Culture = "sk-SK", Name = "Slovak", IsoCode = "sk")]
        SK = 1,
        /// <summary>
        /// Cestia
        /// </summary>
        [LanguageAttribute(Culture = "cs-CZ", Name = "Czech", IsoCode = "cs")]
        CS = 2,
        /// <summary>
        /// Anglictina US
        /// </summary>
        [LanguageAttribute(Culture = "en-US", Name = "English US", IsoCode = "us")]
        EN_US = 3,
        /// <summary>
        /// Anglictina GB
        /// </summary>
        [LanguageAttribute(Culture = "en-GB", Name = "English GB", IsoCode = "gb")]
        EN_GB = 4,
        /// <summary>
        /// Polstina
        /// </summary>
        [LanguageAttribute(Culture = "pl-PL", Name = "Poland", IsoCode = "pl")]
        PL = 5,
        /// <summary>
        /// Nemcina
        /// </summary>
        [LanguageAttribute(Culture = "de-DE", Name = "German", IsoCode = "de")]
        DE = 6,
        /// <summary>
        /// Madarcina
        /// </summary>
        [LanguageAttribute(Culture = "hu-HU", Name = "Hungarian", IsoCode = "hu")]
        HU = 7
    }
}
