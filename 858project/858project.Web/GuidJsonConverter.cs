using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project858.Web
{
    /// <summary>
    /// Guid convertor pre JsonNetResult
    /// </summary>
    public class GuidJsonConverter : JsonConverter
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public GuidJsonConverter()
        {
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Urcuje ci je mozne json citat
        /// </summary>
        public override bool CanRead
        {
            get { return false; }
        }
        /// <summary>
        /// Overi ci je mozne objekt konvertovat podla typu
        /// </summary>
        /// <param name="objectType">Typ objektu</param>
        /// <returns>True = konvertovanie je mozne | false</returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(Guid) == objectType || typeof(Nullable<Guid>) == objectType;
        }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Zapise objekt do json
        /// </summary>
        /// <param name="writer">Writer na zapis objektu</param>
        /// <param name="value">Hodnota ktoru chceme zapisat</param>
        /// <param name="serializer">Serializer</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value == null ? value : ((Guid)value).ToStringWithoutDash());
        }
        /// <summary>
        /// Precita hodnotu z json
        /// </summary>
        /// <param name="reader">Reader na citanie</param>
        /// <param name="objectType">Typ objektu</param>
        /// <param name="existingValue">Aktualna hodnota</param>
        /// <param name="serializer">Serializer</param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
        }
        #endregion
    }
}
