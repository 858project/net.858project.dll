using System;
using System.Collections.Generic;
using System.Text;
using Project858.ComponentModel.Client;
using System.Data;
using System.Data.SQLite;
using System.Net.Mail;
using System.Diagnostics;
using Project858.Diagnostics;
using System.Threading;
using Project858;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Reflection;

namespace Project858.Data.SqlClient
{
    /// <summary>
    /// Klient zabezpecujuci vykonavanie prikazov do SQL databazy 
    /// </summary>
    public class SQLiteClient : SQLiteClientBase
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="builder">Strng builder na vytvorenie SQL connection stringu</param>
        public SQLiteClient(SQLiteConnectionStringBuilder builder)
            : this(builder.DataSource, builder.Password)
        {

        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Vstupny argument nie je inicializovany
        /// </exception>
        /// <param name="database">Databaza do ktorej pristupujeme</param>
        /// <param name="password">Heslo k SQL serveru</param>
        public SQLiteClient(String database, String password)
            : base(database, password)
        {
            this.m_reflectionPropertyCollection = new ReflectionObjectItemCollection();
        }
        #endregion

        #region - Class -
        /// <summary>
        /// Polozka zahrnucjuca reglekciu objektu a jeho property
        /// </summary>
        private sealed class ReflectionPropertyItem
        {
            #region - Constructor -
            /// <summary>
            /// Initialize this class
            /// </summary>
            /// <param name="property">Property ktoru objekt reprezentuje</param>
            public ReflectionPropertyItem(PropertyInfo property)
            {
                if (property == null)
                    throw new ArgumentNullException("property");

                this.m_property = property;
            }
            #endregion

            #region - Properties -
            /// <summary>
            /// Vrati atributy prisluchajucej property
            /// </summary>
            public Object[] Attributes
            {
                get
                {
                    if (this.m_attributes == null)
                    {
                        this.m_attributes = this.m_property.GetCustomAttributes(typeof(ColumnAttribute), true);
                    }
                    return this.m_attributes;
                }
            }
            /// <summary>
            /// Property ktoru objekt reprezentuje
            /// </summary>
            public PropertyInfo Property
            {
                get { return this.m_property; }
            }
            #endregion

            #region - Variable -
            /// <summary>
            /// Atributy prisluchajucej property
            /// </summary>
            private Object[] m_attributes = null;
            /// <summary>
            /// Property
            /// </summary>
            private PropertyInfo m_property = null;
            #endregion
        }
        /// <summary>
        /// Kolekcia property prisluchajuca konkretnemu objektu
        /// </summary>
        private sealed class ReflectionPropertyItemCollection : Dictionary<String, ReflectionPropertyItem>
        {
            #region - Constructor -
            /// <summary>
            /// Initialize this class
            /// </summary>
            /// <param name="type">Typ objektu ktoreho property objekt reprezentuje</param>
            public ReflectionPropertyItemCollection(Type type)
            {
                if (type == null)
                    throw new ArgumentNullException("type");
            }
            #endregion

            #region - Public Methods -
            /// <summary>
            /// Vrati pozadovanu property a jej informacie na zaklade mena
            /// </summary>
            /// <param name="name">Meno property ktoru ziadame</param>
            /// <returns>ReflectionPropertyItem alebo null</returns>
            public ReflectionPropertyItem FindProperty(String name)
            {
                if (this.ContainsKey(name))
                {
                    return this[name];
                }
                return null;
            }
            #endregion

            #region - Private Methods -
            /// <summary>
            /// Inicializuje property objektu
            /// </summary>
            /// <param name="type">Typ objektu ktoreho property objekt reprezentuje</param>
            private void InternalInitializeProperty(Type type)
            {
                PropertyInfo[] properties = type.GetProperties();
                int length = properties.Length;
                for (int i = 0; i < length; i++)
                {
                    PropertyInfo property = properties[i];
                    ReflectionPropertyItem item = new ReflectionPropertyItem(property);
                    this.Add(property.Name, item);
                }
            }
            #endregion
        }
        /// <summary>
        /// Kolekcia objektov definovanych typom obsahujucich informacie o properties
        /// </summary>
        private sealed class ReflectionObjectItemCollection : Dictionary<Type, ReflectionPropertyItemCollection>
        {
            #region - Public Methods -
            /// <summary>
            /// Vrati alebo vytvori a vrati kolekciu property pre pozadovany typ objektu
            /// </summary>
            /// <param name="type">Typ objektu pre ktory chceme informacie vratit</param>
            /// <returns>ReflectionPropertyItemCollection</returns>
            public ReflectionPropertyItemCollection FindPropertyCollection(Type type)
            {
                if (!this.ContainsKey(type))
                {
                    this.InternalCreateType(type);
                }
                return this[type];
            }
            #endregion

            #region - Private Methods -
            /// <summary>
            /// Prida objekt pre pozadovany type
            /// </summary>
            /// <param name="type">Type objektu pre ktory chceme property nacitat</param>
            private void InternalCreateType(Type type)
            {
                ReflectionPropertyItemCollection item = new ReflectionPropertyItemCollection(type);
                this.Add(type, item);
            }
            #endregion
        }
        #endregion

        #region - Variable -
        /// <summary>
        /// Buffer na ukladanie reglekcii objektov
        /// </summary>
        private ReflectionObjectItemCollection m_reflectionPropertyCollection = null;
        #endregion

        #region - Public Method -
        /// <summary>
        /// Vykona vymazanie objektu
        /// </summary>
        /// <param name="item">Objekt ktory chceme vymazat</param>
        /// <returns>True = objekt bol vymazany, inak false</returns>
        public Boolean DeleteObject(Object item)
        {
            try
            {
                return this.InternalDeleteObject(item);
            }
            catch (Exception ex)
            {
                this.InternalTrace(TraceTypes.Error, "Chyba pri vymazavani objektu z SQL {0} [{1}]", ex.Message, item.GetType());
                throw;
            }
        }
        /// <summary>
        /// Vykona select pozadovanych dat
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <returns>Kolekcia nacitanych dat</returns>
        public List<T> Select<T>()
        {
            return this.InternalSelect<T>();
        }
        /// <summary>
        /// Vlozi pozadovany objekt do SQL
        /// </summary>
        /// <param name="item">objekt ktorych chceme vlozit</param>
        /// <returns>True = objekt bol uspesne vlozeny</returns>
        public Boolean TryInsertObject(Object item)
        {
            try
            {
                this.InternalInsertObject(item);
                return true;
            }
            catch (Exception ex)
            {
                this.InternalTrace(TraceTypes.Error, "Chyba pri vkladani objektu do SQL {0} [{1}]", ex.Message, item.GetType());
                throw;
            }
        }
        /// <summary>
        /// Vlozi pozadovany objekt do SQL
        /// </summary>
        /// <param name="item">objekt ktorych chceme vlozit</param>
        public void InsertObject(Object item)
        {
            try
            {
                this.InternalInsertObject(item);
            }
            catch (Exception ex)
            {
                this.InternalTrace(TraceTypes.Error, "Chyba pri vkladani objektu do SQL {0} [{1}]", ex.Message, item.GetType());
                throw;
            }
        }
        /// <summary>
        /// Aktualizuje pozadovany objekt v SQL
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme aktualizovat</typeparam>
        /// <param name="item">objekt ktorych chceme aktualizovat</param>
        /// <returns>True = objekt bol uspesne aktualizovany</returns>
        public Boolean TryUpdateObject(Object item)
        {
            try
            {
                this.InternalUpdateObject(item);
                return true;
            }
            catch (Exception ex)
            {
                this.InternalTrace(TraceTypes.Error, "Chyba pri aktualizacii objektu v SQL {0} [{1}]", ex.Message, item.GetType());
                throw;
            }
        }
        /// <summary>
        /// Aktualizuje pozadovany objekt v SQL
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme aktualizovat</typeparam>
        /// <param name="item">objekt ktorych chceme aktualizovat</param>
        public void UpdateObject(Object item) 
        {
            try
            {
               this.InternalUpdateObject(item);
            }
            catch (Exception ex)
            {
                this.InternalTrace(TraceTypes.Error, "Chyba pri aktualizacii objektu v SQL {0} [{1}]", ex.Message, item.GetType());
                throw;
            }
        }
        /// <summary>
        /// Aktualizuje a nacita pozadovany objekt v / z SQL
        /// </summary>
        /// <param name="item">Objekt ktory chceme aktualizovat</param>
        /// <returns>Aktualizovany objekt</returns>
        public Object UpdateAndReloadObject(Object item)
        {
            try
            {
                return this.InternalUpdateAndReloadObject(item);
            }
            catch (Exception ex)
            {
                this.InternalTrace(TraceTypes.Error, "Chyba pri aktualizacii a obnove objektu v / z SQL {0} [{1}]", ex.Message, item.GetType());
                throw;
            }
        }
        /// <summary>
        /// Nacita objekt z sql readera
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="reader">Reader pomocou ktoreho citame data</param>
        /// <returns>Objekt ktory necitavame</returns>
        public T GetObject<T>(SQLiteDataReader reader)
        {
            return this.InternalGetObject<T>(reader, Activator.CreateInstance<T>());
        }
        /// <summary>
        /// Nacita objekty z pozadovaneho commandu
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="command">SqlCommand</param>
        /// <returns>Kolekcia objektov alebo null</returns>
        public List<T> ReadObjectCollection<T>(SQLiteCommand command)
        {
            List<T> collection = new List<T>();
            using (SQLiteDataReader reader = this.ExecuteReader(command))
            {
                while (reader.Read())
                {
                    T item = this.GetObject<T>(reader);
                    collection.Add(item);
                }
            }
            return collection;
        }
        /// <summary>
        /// Nacita prvy objekt alebo vrati null
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="command">SqlCommand</param>
        /// <returns>Object alebo null</returns>
        public T ReadFirstObject<T>(SQLiteCommand command)
        {
            List<T> collection = new List<T>();
            using (SQLiteDataReader reader = this.ExecuteReader(command))
            {
                while (reader.Read())
                {
                    T item = this.GetObject<T>(reader);
                    return item;
                }
            }
            return default(T);
        }
        /// <summary>
        /// Vrati meno, popis triedy
        /// </summary>
        /// <returns>Meno</returns>
        public override string ToString()
        {
            return String.Format("SqlClient");
        }
        #endregion

        #region - Protected Method -
        /// <summary>
        /// Interne spustenie klienta
        /// </summary>
        /// <returns>True = spustenie klienta bolo uspesne</returns>
        protected override bool InternalStart()
        {
            //vykoname start pripajania k serveru
            return base.InternalStart();
        }
        /// <summary>
        /// Pozastavi funkciu klienta
        /// </summary>
        protected override void InternalPause()
        {
            //ukoncime komunikaciu
            base.InternalPause();
        }
        /// <summary>
        /// Ukonci funkciu klienta
        /// </summary>
        protected override void InternalStop()
        {
            //ukoncime komunikaciu
            base.InternalStop();
        }
        /// <summary>
        /// Vykona pred ukoncenim klienta
        /// </summary>
        protected override void InternalDispose()
        {
            base.InternalDispose();
        }
        #endregion

        #region - Private Method -
        /// <summary>
        /// Aktualizuje a nacita pozadovany objekt v / z SQL
        /// </summary>
        /// <param name="item">Objekt ktory chceme aktualizovat</param>
        /// <returns>Aktualizovany objekt</returns>
        public Object InternalUpdateAndReloadObject(Object item)
        {
            this.InternalUpdateObject(item);
            return this.InternalReloadObject(item);
        }
        /// <summary>
        /// Vykona vymazanie objektu
        /// </summary>
        /// <param name="item">Objekt ktory chceme vymazat</param>
        /// <returns>True = objekt bol vymazany, inak false</returns>
        private Boolean InternalDeleteObject(Object item)
        {
            //objekt musi obsahovat definiciu tabulky
            Type type = item.GetType();
            object[] tableAttributes = type.GetCustomAttributes(typeof(TableAttribute), true);
            if (tableAttributes.Length != 1)
            {
                throw new Exception("Missing attribute TableAttribute");
            }

            //overime primarny kluc
            PropertyInfo[] propertyInfo = type.GetProperties();
            propertyInfo = this.InternalFindPropertiesWithColumnAttribute(propertyInfo);
            PropertyInfo[] primaryKeyPropertyInfo = this.InternalFindPropertiesWithPrimaryKeyState(propertyInfo);
            if (primaryKeyPropertyInfo.Length == 0)
            {
                throw new MissingPrimaryKeyException();
            }

            //vytvorime command
            using (SQLiteCommand command = new SQLiteCommand())
            {
                //vytvorime jednotlive polozky commandu
                command.CommandText = String.Format("DELETE FROM [{0}] WHERE [{1}] = @{1}", ((TableAttribute)tableAttributes[0]).Name,
                    primaryKeyPropertyInfo[0].Name);
                SQLiteParameter primaryKeyParameter = new SQLiteParameter(primaryKeyPropertyInfo[0].Name, ((ColumnAttribute)primaryKeyPropertyInfo[0].GetCustomAttributes(typeof(ColumnAttribute), true)[0]).Type);
                primaryKeyParameter.Value = this.InternalValidateValue(primaryKeyPropertyInfo[0].GetValue(item, null));
                command.Parameters.Add(primaryKeyParameter);

                //vykoname priklad do DB
                int count = this.ExecuteNonQuery(command);
                return count == 1;
            }
        }
        /// <summary>
        /// Vykona aktualizaciu objektu
        /// </summary>
        /// <param name="item">Objekt ktory chceme aktualizovat</param>
        /// <returns>Aktualizovany objekt</returns>
        private Object InternalReloadObject(Object item) 
        {
            //objekt musi obsahovat definiciu tabulky
            Type type = item.GetType();
            object[] tableAttributes = type.GetCustomAttributes(typeof(TableAttribute), true);
            if (tableAttributes.Length != 1)
            {
                throw new Exception("Missing attribute TableAttribute");
            }

            //informacie o properties
            PropertyInfo[] propertyInfo = type.GetProperties();
            propertyInfo = this.InternalFindPropertiesWithColumnAttribute(propertyInfo);
            if (propertyInfo.Length == 0)
            {
                throw new Exception("Missing attribute ColumnAttribute");
            }
            if (propertyInfo.Length == 1)
            {
                throw new Exception("Too little attribute ColumnAttribute");
            }

            //overime primarny kluc
            PropertyInfo[] primaryKeyPropertyInfo = this.InternalFindPropertiesWithPrimaryKeyState(propertyInfo);
            if (primaryKeyPropertyInfo.Length == 0)
            {
                throw new MissingPrimaryKeyException();
            }

            //vytvorime command
            using (SQLiteCommand command = new SQLiteCommand())
            {
                //vytvorime jednotlive polozky commandu
                command.CommandText = String.Format("SELECT * FROM [{0}] WHERE [{1}] = @{1}", ((TableAttribute)tableAttributes[0]).Name,
                    primaryKeyPropertyInfo[0].Name);
                SQLiteParameter primaryKeyParameter = new SQLiteParameter(primaryKeyPropertyInfo[0].Name, ((ColumnAttribute)primaryKeyPropertyInfo[0].GetCustomAttributes(typeof(ColumnAttribute), true)[0]).Type);
                primaryKeyParameter.Value = this.InternalValidateValue(primaryKeyPropertyInfo[0].GetValue(item, null));
                command.Parameters.Add(primaryKeyParameter);

                //nacitame data
                using (SQLiteDataReader reader = this.ExecuteReader(command))
                {
                    while (reader.Read())
                    {
                        return this.InternalGetObject(reader, item);
                    }
                }
            }

            //vratime aktualnu hodnotu
            throw new InvalidOperationException("Object is not available !");
        }
        /// <summary>
        /// Nacita objekt z sql readera
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="reader">Reader pomocou ktoreho citame data</param>
        /// <param name="item">Objekt ktoryc chceme aktualizovat</param>
        /// <returns>Objekt ktory necitavame</returns>
        private T InternalGetObject<T>(SQLiteDataReader reader, T item)
        {
            String name = String.Empty;
            ReflectionPropertyItemCollection properties = this.m_reflectionPropertyCollection.FindPropertyCollection(item.GetType());
            if (properties != null)
            {
                Object value = null;
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    name = reader.GetName(i);
                    ReflectionPropertyItem property = properties.FindProperty(name);
                    if (property != null && property.Property.CanWrite)
                    {
                        if (property.Attributes.Length == 1)
                        {
                            value = reader[name];
                            try
                            {
                                value = ((value == null || value == DBNull.Value) ? null : value);
                                if (value == null && !((ColumnAttribute)property.Attributes[0]).CanBeNull)
                                {
                                    throw new InvalidCastException();
                                }
                                property.Property.SetValue(item, ((value == null || value == DBNull.Value) ? null : value), null);
                            }
                            catch (Exception ex)
                            {
                                this.InternalTrace(TraceTypes.Error, "Chyba pri citani dat z SQL. {0} [{1} - {2}]", ex.Message, property.Property.Name, property.Property.PropertyType);
                                throw;
                            }
                        }
                    }
                }
            }
            return item;
        }
        /// <summary>
        /// Vykona select pozadovanych dat
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <returns>Kolekcia nacitanych dat</returns>
        private List<T> InternalSelect<T>()
        {
            Type type = typeof(T);
            object[] tableAttributes = type.GetCustomAttributes(typeof(TableAttribute), true);
            if (tableAttributes.Length != 1)
            {
                throw new Exception("Missing attribute TableAttribute");
            }

            //spracujeme dommand do SQL
            using (SQLiteCommand command = new SQLiteCommand())
            {
                command.CommandText = String.Format("SELECT * FROM [{0}]", ((TableAttribute)tableAttributes[0]).Name);
                return this.ReadObjectCollection<T>(command);
            }
        }
        /// <summary>
        /// Aktualizuje pozadovany objekt v SQL
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme aktualizovat</typeparam>
        /// <param name="item">objekt ktorych chceme aktualizovat</param>
        private void InternalInsertObject(Object item)
        {
            //objekt musi byt zadany
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            //objekt musi obsahovat definiciu tabulky
            Type type = item.GetType();
            object[] tableAttributes = type.GetCustomAttributes(typeof(TableAttribute), true);
            if (tableAttributes.Length != 1)
            {
                throw new Exception("Missing attribute TableAttribute");
            }

            //informacie o properties
            PropertyInfo[] propertyInfo = type.GetProperties();
            propertyInfo = this.InternalFindPropertiesWithColumnAttribute(propertyInfo);
            if (propertyInfo.Length == 0)
            {
                throw new Exception("Missing attribute ColumnAttribute");
            }

            //vytvorime command
            List<SQLiteParameter> parameterCollection = new List<SQLiteParameter>();
            StringBuilder builder = new StringBuilder();
            StringBuilder values = new StringBuilder();
            builder.AppendFormat("INSERT INTO [{0}] (", ((TableAttribute)tableAttributes[0]).Name);
            for (int i = 0; i < propertyInfo.Length; i++)
            {
                PropertyInfo info = propertyInfo[i];
                ColumnAttribute attribude = (ColumnAttribute)info.GetCustomAttributes(typeof(ColumnAttribute), true)[0];
                if (!attribude.IsDbGenerated)
                {
                    builder.AppendFormat("{0}, ", info.Name);
                    values.AppendFormat("@{0}, ", info.Name);
                    SQLiteParameter parameter = new SQLiteParameter(info.Name, attribude.Type);
                    parameter.Value = this.InternalValidateValue(info.GetValue(item, null));
                    parameterCollection.Add(parameter);
                }
            }
            builder.Remove(builder.Length - 2, 2);
            values.Remove(values.Length - 2, 2);
            builder.AppendFormat(") VALUES ({0})", values.ToString());

            //spracujeme dommand do SQL
            using (SQLiteCommand command = new SQLiteCommand())
            {
                command.CommandText = builder.ToString();
                command.Parameters.AddRange(parameterCollection.ToArray());
                this.ExecuteNonQuery(command);
            }
        }
        /// <summary>
        /// Aktualizuje pozadovany objekt v SQL
        /// </summary>
        /// <param name="item">objekt ktorych chceme aktualizovat</param>
        private void InternalUpdateObject(Object item)
        {
            //objekt musi byt zadany
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            //objekt musi obsahovat definiciu tabulky
            Type type = item.GetType();
            object[] tableAttributes = type.GetCustomAttributes(typeof(TableAttribute), true);
            if (tableAttributes.Length != 1) 
            {
                throw new Exception("Missing attribute TableAttribute");
            }

            //informacie o properties
            PropertyInfo[] propertyInfo = type.GetProperties();
            propertyInfo = this.InternalFindPropertiesWithColumnAttribute(propertyInfo);
            if (propertyInfo.Length == 0)
            {
                throw new Exception("Missing attribute ColumnAttribute");
            }
            if (propertyInfo.Length == 1) 
            {
                throw new Exception("Too little attribute ColumnAttribute");
            }

            //overime primarny kluc
            PropertyInfo[] primaryKeyPropertyInfo = this.InternalFindPropertiesWithPrimaryKeyState(propertyInfo);
            if (primaryKeyPropertyInfo.Length == 0)
            {
                throw new MissingPrimaryKeyException();
            }
            if (primaryKeyPropertyInfo.Length > 1)
            {
                throw new Exception("Duplicate PrimaryKey state");
            }

            //vytvorime command
            List<SQLiteParameter> parameterCollection = new List<SQLiteParameter>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("UPDATE [{0}] SET ", ((TableAttribute)tableAttributes[0]).Name);
            for (int i = 0; i < propertyInfo.Length; i++)
            {
                PropertyInfo info = propertyInfo[i];
                if (!primaryKeyPropertyInfo[0].Equals(info))
                {
                    ColumnAttribute attribude = (ColumnAttribute)info.GetCustomAttributes(typeof(ColumnAttribute), true)[0];
                    if (!attribude.IsDbGenerated)
                    {
                        builder.AppendFormat("[{0}] = @{0}, ", info.Name);
                        SQLiteParameter parameter = new SQLiteParameter(info.Name, attribude.Type);
                        parameter.Value = this.InternalValidateValue(info.GetValue(item, null));
                        parameterCollection.Add(parameter);
                    }
                }
            }
            builder.Remove(builder.Length - 2, 2);
            builder.AppendFormat(" WHERE [{0}] = @{0}", primaryKeyPropertyInfo[0].Name);
            SQLiteParameter primaryKeyParameter = new SQLiteParameter(primaryKeyPropertyInfo[0].Name, ((ColumnAttribute)primaryKeyPropertyInfo[0].GetCustomAttributes(typeof(ColumnAttribute), true)[0]).Type);
            primaryKeyParameter.Value = primaryKeyPropertyInfo[0].GetValue(item, null);
            parameterCollection.Add(primaryKeyParameter);

            //vykoname command
            using (SQLiteCommand command = new SQLiteCommand())
            {
                command.CommandText = builder.ToString();
                command.Parameters.AddRange(parameterCollection.ToArray());
                this.ExecuteNonQuery(command);
            }
        }
        /// <summary>
        /// Overi hodnotu a vrati jej spravny format
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme overit</param>
        /// <returns>Hodnota</returns>
        private Object InternalValidateValue(Object value)
        {
            return value == null ? DBNull.Value : value;
        }
        /// <summary>
        /// Vyhlada property ktore su nastavene ako primarny kluc objektu
        /// </summary>
        /// <param name="propertyInfo">Kolekcia v ktorej chceme vyhladat</param>
        /// <returns>Property ktore obsahuju nastaveny stav primarneho kluca</returns>
        private PropertyInfo[] InternalFindPropertiesWithPrimaryKeyState(PropertyInfo[] propertyInfo)
        {
            List<PropertyInfo> collection = new List<PropertyInfo>();
            for (int i = 0; i < propertyInfo.Length; i++)
            {
                PropertyInfo info = propertyInfo[i];
                Object[] attributes = info.GetCustomAttributes(typeof(ColumnAttribute), true);
                if (attributes.Length == 1 && ((ColumnAttribute)attributes[0]).IsPrimaryKey)
                {
                    collection.Add(info);
                }
            }
            return collection.ToArray();
        }
        /// <summary>
        /// Vyhlada property ktore obsahuju pozadovany typ atributu
        /// </summary>
        /// <param name="propertyInfo">Kolekcia v ktorej chceme vyhladat</param>
        /// <returns>Pole najdenych property ktore obsahuju pozadovany atribut</returns>
        private PropertyInfo[] InternalFindPropertiesWithColumnAttribute(PropertyInfo[] propertyInfo)
        {
            List<PropertyInfo> collection = new List<PropertyInfo>();
            for (int i = 0; i < propertyInfo.Length; i++) 
            {
                PropertyInfo info = propertyInfo[i];
                if (info.GetCustomAttributes(typeof(ColumnAttribute), true).Length == 1)
                {
                    collection.Add(info);
                }
            }
            return collection.ToArray();
        }
        #endregion
    }
}
