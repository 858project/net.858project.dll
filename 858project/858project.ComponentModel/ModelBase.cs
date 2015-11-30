using Newtonsoft.Json;
using Project858.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Project858.ComponentModel
{
    /// <summary>
    /// Model base pre vytvaranie modelov
    /// </summary>
    public abstract class ModelBase : IModel
    {
        #region - Properties -
        /// <summary>
        /// Datum vytvorenia zaznamu
        /// </summary>
        [JsonIgnore]
        [ScriptIgnore]
        [ColumnAttribute(Type = SqlDbType.DateTimeOffset)]
        public DateTimeOffset CreateDate { get; set; }
        /// <summary>
        /// Datum poslednej aktualizacie zaznamu
        /// </summary>
        [JsonIgnore]
        [ScriptIgnore]
        [ColumnAttribute(Type = SqlDbType.DateTimeOffset, IsRequiredWhenUpdating = true)]
        public DateTimeOffset UpdateDate { get; set; }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Overi platnost modelu
        /// </summary>
        /// <returns>True = model je plany, inak false</returns>
        public abstract bool Validate();
        #endregion
    }
}
