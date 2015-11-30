using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Project858.Data.SqlClient
{
    /// <summary>
    /// Nastavenie strlca tabulky
    /// </summary>
    public sealed class ColumnAttribute : Attribute
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public ColumnAttribute()
        {
            this.IsRequiredWhenInserting = false;
            this.IsRequiredWhenUpdating = false;
            this.CanBeNull = false;
            this.Size = int.MaxValue;
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Definuje dlzku contextu
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// Definuje ci hodnotu potrebne pripojit do Insert
        /// </summary>
        public Boolean IsRequiredWhenInserting { get; set; }
        /// <summary>
        /// Definuje ci hodnotu potrebne pripojit do Update
        /// </summary>
        public Boolean IsRequiredWhenUpdating { get; set; }
        /// <summary>
        /// Meno typu. Vyuzivane hlavne pri SqlDbType.Udt
        /// </summary>
        public String TypeName { get; set; }
        /// <summary>
        /// Definuje ci moze byt hodnota null
        /// </summary>
        public bool CanBeNull { get; set; }
        /// <summary>
        /// Typ objektu v databaze
        /// </summary>
        public SqlDbType Type { get; set; }
        /// <summary>
        /// Definuje ci je hodnota generovana databazou
        /// </summary>
        public bool IsDbGenerated { get; set; }
        /// <summary>
        /// Definuje ci je hodnota primernym klucom
        /// </summary>
        public bool IsPrimaryKey { get; set; }
        #endregion
    }
}
