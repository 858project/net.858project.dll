using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project858.Data.SqlClient;
using Project858.Web;
using Project858.ComponentModel;
using System.Web.Script.Serialization;

namespace Project858.ComponentModel
{
    /// <summary>
    /// Model reprezentujuci task
    /// </summary>
    public abstract class FileModelBase : ModelBase
    {
        #region - Properties -
        /// <summary>
        /// Id tasku
        /// </summary>
        [ColumnAttribute(IsPrimaryKey = true, Type = SqlDbType.UniqueIdentifier, IsRequiredWhenInserting = true, IsRequiredWhenUpdating = false)]
        public Guid IdFile { get; set; }
        /// <summary>
        /// Meno suboru
        /// </summary>
        [ColumnAttribute(Type = SqlDbType.VarChar, IsRequiredWhenInserting = true, IsRequiredWhenUpdating = true)]
        public String Name { get; set; }
        /// <summary>
        /// Meno suboru
        /// </summary>
        [ColumnAttribute(Type = SqlDbType.VarChar, IsRequiredWhenInserting = true, IsRequiredWhenUpdating = true)]
        public String ContentType { get; set; }
        /// <summary>
        /// Id uzivatela ktory ulohu zadal
        /// </summary>
        [ScriptIgnore]
        [ColumnAttribute(Type = SqlDbType.VarBinary, IsRequiredWhenInserting = true, IsRequiredWhenUpdating = true)]
        public Byte[] Data { get; set; }
        #endregion
    }
}