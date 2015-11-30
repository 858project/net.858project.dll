using System;
using System.Collections.Generic;
using System.Text;

namespace Project858
{
    /// <summary>
    /// Standardne casto vyuzivane konstanty
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Regex na overenie stringy ci obsahuje len alpha znaky.
        /// Je nutne urobit Replace("LENGTH", "pozadovana dlzka")
        /// </summary>
        public const String REGEX_ALPHA = @"^[a-zA-Z]{LENGTH,}$";
        /// <summary>
        /// Regex na overenie dlzky stringu
        /// Je nutne urobit Replace("LENGTH", "pozadovana dlzka")
        /// </summary>
        public const String REGEX_LENGTH = @"^.{LENGTH,}$";
        /// <summary>
        /// Regex na overenie stringy ci obsahuje len ciselne znaky.
        /// Je nutne urobit Replace("LENGTH", "pozadovana dlzka")
        /// </summary>
        public const String REGEX_NUMERIC = @"^[0-9]{LENGTH,}$";
        /// <summary>
        /// Regex na overenie stringy ci obsahuje len alpha numericke znaky
        /// Je nutne urobit Replace("LENGTH", "pozadovana dlzka")
        /// </summary>
        public const String REGEX_ALPHANUMERIC = @"^[0-9a-zA-Z]{LENGTH,}$";
        /// <summary>
        /// Regex na overenie stringy ci obsahuje len alpha numericke znaky, pricom alphanumericke su len male
        /// Je nutne urobit Replace("LENGTH", "pozadovana dlzka")
        /// </summary>
        public const String REGEX_ALPHANUMERIC_SMALL = @"^[0-9a-z]{LENGTH,}$";
    }
}
