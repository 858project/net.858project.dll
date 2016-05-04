using System;
using System.Collections.Generic;
using System.Text;
using Project858.ComponentModel.Client;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Diagnostics;
using Project858.Diagnostics;
using System.Threading;
using Project858;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Reflection;
using System.Dynamic;

namespace Project858.Data.SqlClient
{
    /// <summary>
    /// Klient zabezpecujuci vykonavanie prikazov do SQL databazy 
    /// </summary>
    public class SqlClient : SqlClientBase
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Vstupny argument nie je inicializovany
        /// </exception>
        /// <param name="sqlServer">Meno SQL servera na pristup k datam</param>
        /// <param name="sqlDatabase">Databaza do ktorej pristupujeme</param>
        /// <param name="sqlLogin">Login k SQL serveru</param>
        /// <param name="sqlPassword">Heslo k SQL serveru</param>
        public SqlClient(String sqlServer, String sqlDatabase, String sqlLogin, String sqlPassword)
            : base(sqlServer, sqlDatabase, sqlLogin, sqlPassword)
        {
            this.m_lockObj = new Object();
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
            public ColumnAttribute ColumnAttribute
            {
                get
                {
                    if (this.m_columnAttributes == null)
                    {
                        Object[] attributes = this.Property.GetCustomAttributes(typeof(ColumnAttribute), true);
                        if (attributes != null && attributes.Length == 1)
                        {
                            this.m_columnAttributes = attributes[0] as ColumnAttribute;
                        }
                    }
                    return this.m_columnAttributes;
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
            private ColumnAttribute m_columnAttributes = null;
            /// <summary>
            /// Property
            /// </summary>
            private PropertyInfo m_property = null;
            #endregion
        }
        /// <summary>
        /// Kolekcia property prisluchajuca konkretnemu objektu
        /// </summary>
        private sealed class ReflectionObjectItem : Dictionary<String, ReflectionPropertyItem>
        {
            #region - Constructor -
            /// <summary>
            /// Initialize this class
            /// </summary>
            /// <param name="type">Typ objektu ktoreho property objekt reprezentuje</param>
            public ReflectionObjectItem(Type type)
            {
                if (type == null)
                    throw new ArgumentNullException("type");

                this.Type = type;
                this.InternalInitializeProperty(type);
            }
            #endregion

            #region - Properties -
            /// <summary>
            /// Typ objektu
            /// </summary>
            public Type Type { get; set; }
            /// <summary>
            /// Vrati meno tabulky alebo view ktore je mozne pouzit pre SELECT command
            /// </summary>
            public String TableOrViewName
            {
                get
                {
                    if (this.ViewAttribute != null)
                    {
                        return this.ViewAttribute.Name;
                    }
                    if (this.TableAttribute != null)
                    {
                        return this.TableAttribute.Name;
                    }
                    return String.Empty;
                }
            }
            /// <summary>
            /// Atribut definujuci informacie o view prisluchajucemu k objektu
            /// </summary>
            public ViewAttribute ViewAttribute
            {
                get
                {
                    if (this.m_viewAttribute == null)
                    {
                        Object[] attributes = this.Type.GetCustomAttributes(typeof(ViewAttribute), true);
                        if (attributes != null && attributes.Length == 1)
                        {
                            this.m_viewAttribute = attributes[0] as ViewAttribute;
                        }
                    }
                    return this.m_viewAttribute;
                }
            }
            /// <summary>
            /// Atribut definujuci informacie o tabulke prisluchajucej k objekte
            /// </summary>
            public TableAttribute TableAttribute
            {
                get
                {
                    if (this.m_tableAttribute == null)
                    {
                        Object[] attributes = this.Type.GetCustomAttributes(typeof(TableAttribute), true);
                        if (attributes != null && attributes.Length == 1)
                        {
                            this.m_tableAttribute = attributes[0] as TableAttribute;
                        }
                    }
                    return this.m_tableAttribute;
                }
            }
            #endregion

            #region - Variable -
            /// <summary>
            /// Atribut definujuci informacie o view prisluchajucemu k objektu
            /// </summary>
            private ViewAttribute m_viewAttribute = null;
            /// <summary>
            /// Atribut definujuci informacie o tabulke prisluchajucej k objekte
            /// </summary>
            private TableAttribute m_tableAttribute = null;
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
        private sealed class ReflectionObjectItemCollection : Dictionary<Type, ReflectionObjectItem>
        {
            #region - Public Methods -
            /// <summary>
            /// Vrati alebo vytvori a vrati kolekciu property pre pozadovany typ objektu
            /// </summary>
            /// <param name="type">Typ objektu pre ktory chceme informacie vratit</param>
            /// <returns>ReflectionPropertyItemCollection</returns>
            public ReflectionObjectItem FindPropertyCollection(Type type)
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
                ReflectionObjectItem item = new ReflectionObjectItem(type);
                this.Add(type, item);
            }
            #endregion
        }
        #endregion

        #region - Variable -
        /// <summary>
        /// Pomocny synchronizacny objekt na pristup k pripojeniu
        /// </summary>
        private readonly Object m_lockObj = null;
        /// <summary>
        /// Buffer na ukladanie reglekcii objektov
        /// </summary>
        private ReflectionObjectItemCollection m_reflectionPropertyCollection = null;
        #endregion

        #region - Public Method -
        /// <summary>
        /// Vykona pozadovany query prikaz
        /// </summary>
        /// <param name="query">Query prikaz</param>
        /// <returns>Pocet ovplyvnenych riadkov</returns>
        public int ExecuteNonQuery(String query)
        {
            return this.InternalExecuteNonQuery(query);
        }
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
                this.InternalTrace(TraceTypes.Error, "Chyba pri vymazavani objektu z SQL {0} [{1} : {2}]", ex.Message, item.GetType(), item.ToJsonString());
                throw;
            }
        }
        /// <summary>
        /// Vykona select pozadovanych dat
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="whereClause">Where podmienka</param>
        /// <param name="orderClause">Order klauzula</param>
        /// <param name="limit">Urcuje maximalne mnozstvo poloziek ktore chceme nacitat</param>
        /// <param name="page">Stranka dat</param>
        /// <returns>Kolekcia nacitanych dat</returns>
        public List<T> Select<T>(String whereClause = null, String orderClause = null, Nullable<UInt32> limit = null, Nullable<UInt32> page = null)
        {
            if (limit != null && limit.Value < 1)
            {
                throw new ArgumentException("Value 'limit' cannot be less than the minimum value 1");
            }
            return this.InternalSelect<T>(whereClause, orderClause, limit, page);
        }
        /// <summary>
        /// Nacita pocet dat
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="whereClause">Where podmienka</param>
        /// <returns>Pocet dat</returns>
        public Int32 SelectCount<T>(String whereClause = null)
        {
            return this.InternalSelectCount<T>(whereClause);
        }
        /// <summary>
        /// Vykona select s nacitanim dat do dynamic objektu
        /// </summary>
        /// <param name="query">Query na vykonanie selectu</param>
        /// <returns>Kolekcia dynamic objektov</returns>
        public List<dynamic> SelectDynamicFromQuery(String query)
        {
            return this.InternalSelectDynamicFromQuery(query);
        }
        /// <summary>
        /// Vykona select pozadovanych dat
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="query">Query ktorym chceme nacitat data</param>
        /// <returns>Kolekcia nacitanych dat</returns>
        public List<T> SelectFromQuery<T>(String query)
        {
            return this.InternalSelectFromQuery<T>(query);
        }
        /// <summary>
        /// Vykona select pozadovanych dat
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="whereClause">Where podmienka</param>
        /// <param name="orderClause">Order klauzula</param>
        /// <returns>Kolekcia nacitanych dat</returns>
        public T SelectFirstObject<T>(String whereClause = null, String orderClause = null)
        {
            return this.InternalSelectFirstObject<T>(whereClause, orderClause);
        }
        /// <summary>
        /// Vykona select pozadovanych dat
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="query">Query ktorym chceme nacitat data</param>
        /// <returns>Kolekcia nacitanych dat</returns>
        public T SelectFirstObjectFromQuery<T>(String query)
        {
            return this.InternalSelectFirstObjectFromQuery<T>(query);
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
                this.InternalTrace(TraceTypes.Error, "Chyba pri vkladani objektu do SQL. {0} [{1} : {2}]", ex.Message, item.GetType(), item.ToJsonString());
                this.InternalException(ex);
                return false;
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
                this.InternalTrace(TraceTypes.Error, "Chyba pri vkladani objektu do SQL {0} [{1} : {2}]", ex.Message, item.GetType(), item.ToJsonString());
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
                this.InternalTrace(TraceTypes.Error, "Chyba pri aktualizacii objektu v SQL {0} [{1}: {2}]", ex.Message, item.GetType(), item.ToJsonString());
                this.InternalException(ex);
                return false;
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
                this.InternalTrace(TraceTypes.Error, "Chyba pri aktualizacii objektu v SQL {0} [{1} : {2}]", ex.Message, item.GetType(), item.ToJsonString());
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
                this.InternalTrace(TraceTypes.Error, "Chyba pri aktualizacii a obnove objektu v / z SQL {0} [{1} : {2}]", ex.Message, item.GetType(), item.ToJsonString());
                throw;
            }
        }
        /// <summary>
        /// Nacita dynamic z sql readera
        /// </summary>
        /// <param name="reader">Reader pomocou ktoreho citame data</param>
        /// <returns>Dynamic ktory nacitame</returns>
        public dynamic GetDynamic(SqlDataReader reader)
        {
            return this.InternalGetDynamic(reader, new ExpandoObject());
        }
        /// <summary>
        /// Nacita objekt z sql readera
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="reader">Reader pomocou ktoreho citame data</param>
        /// <returns>Objekt ktory necitavame</returns>
        public T GetObject<T>(SqlDataReader reader)
        {
            return this.InternalGetObject<T>(reader, Activator.CreateInstance<T>());
        }
        /// <summary>
        /// Nacita kolekciu dynamic objektov
        /// </summary>
        /// <param name="command">SqlCommand</param>
        /// <returns>Kolekcia objektov alebo null</returns>
        public List<dynamic> ReadDynamicCollection(SqlCommand command)
        {
            //inicializujeme
            List<dynamic> collection = new List<dynamic>();

            //synchronizacia
            lock (this.m_lockObj)
            {
                using (SqlDataReader reader = this.ExecuteReader(command))
                {
                    while (reader.Read())
                    {
                        dynamic item = this.GetDynamic(reader);
                        collection.Add(item);
                    }
                }
            }

            //vratime nacitanu kolekciu
            return collection;
        }
        /// <summary>
        /// Nacita objekty z pozadovaneho commandu
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="command">SqlCommand</param>
        /// <returns>Kolekcia objektov alebo null</returns>
        public List<T> ReadObjectCollection<T>(SqlCommand command)
        {
            //inicializujeme
            List<T> collection = new List<T>();

            //synchronizacia
            lock (this.m_lockObj)
            {
                using (SqlDataReader reader = this.ExecuteReader(command))
                {
                    while (reader.Read())
                    {
                        T item = this.GetObject<T>(reader);
                        collection.Add(item);
                    }
                }
            }

            //vratime nacitanu kolekciu
            return collection;
        }
        /// <summary>
        /// Nacita prvy objekt alebo vrati null
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="command">SqlCommand</param>
        /// <returns>Object alebo null</returns>
        public T ReadFirstObject<T>(SqlCommand command)
        {
            //inicializujeme
            List<T> collection = new List<T>();

            //synchronizacia
            lock (this.m_lockObj)
            {
                using (SqlDataReader reader = this.ExecuteReader(command))
                {
                    while (reader.Read())
                    {
                        T item = this.GetObject<T>(reader);
                        return item;
                    }
                }
            }

            //vratime default
            return default(T);
        }
        /// <summary>
        /// Vykona aktualizaciu objektu
        /// </summary>
        /// <param name="command">Command na zaklade ktoreho chceme ziskat dat</param>
        /// <returns>Aktualizovana kolekcia objektov</returns>
        public List<SqlObject> ReadSqlObjectCollection(SqlCommand command)
        {
            return this.InternalReadSqlObjectCollection(command);
        }
        /// <summary>
        /// Vykona aktualizaciu objektu
        /// </summary>
        /// <param name="command">Command na zaklade ktoreho chceme ziskat dat</param>
        /// <returns>Aktualizovany objekt</returns>
        public SqlObject ReadSqlObject(SqlCommand command)
        {
            return this.InternalReadSqlObject(command);
        }
        /// <summary>
        /// Vrati pocet data
        /// </summary>
        /// <param name="whereClause">Podmienka pre ziskanie poctu</param>
        /// <returns>Pocet dat</returns>
        public int GetCount<T>(String whereClause = null)
        {
            return this.InternalGetCount<T>(whereClause);
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
        /// Vrati pocet data
        /// </summary>
        /// <param name="whereClause">Podmienka pre ziskanie poctu</param>
        /// <returns>Pocet dat</returns>
        public int InternalGetCount<T>(String whereClause = null)
        {
            //premapujeme datovy objekt
            ReflectionObjectItem properties = this.m_reflectionPropertyCollection.FindPropertyCollection(typeof(T));

            //objekt musi obsahovat definiciu tabulky
            if (properties.TableAttribute == null)
            {
                throw new Exception("Missing attribute TableAttribute");
            }

            //ziskame meno
            String name = properties.ViewAttribute != null ? properties.ViewAttribute.Name : properties.TableAttribute.Name;

            //vytvorime command
            String query = String.Format("SELECT COUNT(*) FROM [{0}]{1}", name,
                 (String.IsNullOrWhiteSpace(whereClause) ? String.Empty : String.Format(" WHERE {0}", whereClause)));

            //spracujeme command do SQL
            using (SqlCommand command = new SqlCommand())
            {
                //vykoname prikaz
                command.CommandText = query;
                Object value = this.ExecuteScalar(command);
                if (value is int)
                {
                    return (int)value;
                }
                return 0;
            }
        }
        /// <summary>
        /// Vykona pozadovany query prikaz
        /// </summary>
        /// <param name="query">Query prikaz</param>
        /// <returns>Pocet ovplyvnenych riadkov</returns>
        public int InternalExecuteNonQuery(String query)
        {
            //spracujeme dommand do SQL
            using (SqlCommand command = new SqlCommand())
            {
                //vykoname prikad
                command.CommandText = query;
                return this.ExecuteNonQuery(command);
            }
        }
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
            //najdeme informacie o datovom type
            ReflectionObjectItem properties = this.m_reflectionPropertyCollection.FindPropertyCollection(item.GetType());

            //objekt musi obsahovat definiciu tabulky
            if (properties.TableAttribute == null)
            {
                throw new Exception("Missing attribute TableAttribute");
            }

            //overime primarny kluc
            List<ReflectionPropertyItem> primaryKeyPropertyInfo = this.InternalFindPropertiesWithPrimaryKeyState(properties);
            if (primaryKeyPropertyInfo.Count == 0)
            {
                throw new MissingPrimaryKeyException();
            }
            if (primaryKeyPropertyInfo.Count > 1)
            {
                throw new Exception("Duplicate PrimaryKey state");
            }

            //vytvorime command
            using (SqlCommand command = new SqlCommand())
            {
                //vytvorime jednotlive polozky commandu
                command.CommandText = String.Format("DELETE FROM [{0}] WHERE [{1}] = @{1}", properties.TableAttribute.Name, primaryKeyPropertyInfo[0].Property.Name);
                SqlParameter primaryKeyParameter = new SqlParameter(primaryKeyPropertyInfo[0].Property.Name, primaryKeyPropertyInfo[0].ColumnAttribute.Type);
                primaryKeyParameter.Value = this.InternalPrepareValue(primaryKeyPropertyInfo[0].Property.GetValue(item, null), primaryKeyPropertyInfo[0].ColumnAttribute);
                command.Parameters.Add(primaryKeyParameter);

                //vykoname priklad do DB
                int count = this.ExecuteNonQuery(command);
                return count == 1;
            }
        }
        /// <summary>
        /// Vykona aktualizaciu objektu
        /// </summary>
        /// <param name="command">Command na zaklade ktoreho chceme ziskat dat</param>
        /// <returns>Aktualizovany objekt</returns>
        private List<SqlObject> InternalReadSqlObjectCollection(SqlCommand command)
        {
            //initializujeme kolekciu
            List<SqlObject> collection = new List<SqlObject>();

            //synchronizacia
            lock (this.m_lockObj)
            {
                //nacitame data
                using (SqlDataReader reader = this.ExecuteReader(command))
                {
                    while (reader.Read())
                    {
                        collection.Add(this.InternalGetSqlObject(reader));
                    }
                }
            }

            //vratime data
            return collection;
        }
        /// <summary>
        /// Vykona aktualizaciu objektu
        /// </summary>
        /// <param name="command">Command na zaklade ktoreho chceme ziskat dat</param>
        /// <returns>Aktualizovany objekt</returns>
        private SqlObject InternalReadSqlObject(SqlCommand command)
        {
            //synchronizacia
            lock (this.m_lockObj)
            {
                //nacitame data
                using (SqlDataReader reader = this.ExecuteReader(command))
                {
                    while (reader.Read())
                    {
                        return this.InternalGetSqlObject(reader);
                    }
                }
            }

            //vratime aktualnu hodnotu
            throw new InvalidOperationException("Object is not available !");
        }
        /// <summary>
        /// Vykona aktualizaciu objektu
        /// </summary>
        /// <param name="item">Objekt ktory chceme aktualizovat</param>
        /// <returns>Aktualizovany objekt</returns>
        private Object InternalReloadObject(Object item)
        {
            //najdeme informacie o datovom type
            ReflectionObjectItem properties = this.m_reflectionPropertyCollection.FindPropertyCollection(item.GetType());

            //objekt musi obsahovat definiciu tabulky
            if (properties.TableAttribute == null)
            {
                throw new Exception("Missing attribute TableAttribute");
            }
            //informacie o properties
            if (properties.Count == 0)
            {
                throw new Exception("Missing attribute ColumnAttribute");
            }

            //overime primarny kluc
            List<ReflectionPropertyItem> primaryKeyPropertyInfo = this.InternalFindPropertiesWithPrimaryKeyState(properties);
            if (primaryKeyPropertyInfo.Count == 0)
            {
                throw new MissingPrimaryKeyException();
            }
            if (primaryKeyPropertyInfo.Count > 1)
            {
                throw new Exception("Duplicate PrimaryKey state");
            }

            //synchronizacia
            lock (this.m_lockObj)
            {
                //vytvorime command
                using (SqlCommand command = new SqlCommand())
                {
                    //vytvorime jednotlive polozky commandu
                    command.CommandText = String.Format("SELECT * FROM [{0}] WHERE [{1}] = @{1}", properties.TableOrViewName, primaryKeyPropertyInfo[0].Property.Name);
                    SqlParameter primaryKeyParameter = new SqlParameter(primaryKeyPropertyInfo[0].Property.Name, primaryKeyPropertyInfo[0].ColumnAttribute.Type);
                    primaryKeyParameter.Value = this.InternalPrepareValue(primaryKeyPropertyInfo[0].Property.GetValue(item, null), primaryKeyPropertyInfo[0].ColumnAttribute);
                    command.Parameters.Add(primaryKeyParameter);

                    //nacitame data
                    using (SqlDataReader reader = this.ExecuteReader(command))
                    {
                        while (reader.Read())
                        {
                            return this.InternalGetObject(reader, item);
                        }
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
        private SqlObject InternalGetSqlObject(SqlDataReader reader)
        {
            String name = String.Empty;
            dynamic item = new SqlObject();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                name = reader.GetName(i);
                ((SqlObject)item).SetMember(name, reader[name]);
            }
            return item;
        }
        /// <summary>
        /// Nacita dynamic z sql readera
        /// </summary>
        /// <param name="reader">Reader pomocou ktoreho citame data</param>
        /// <param name="item">Dynamc ktory chceme aktualizovat</param>
        /// <returns>Dynamic ktory nacitame</returns>
        private dynamic InternalGetDynamic(SqlDataReader reader, dynamic item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            var dynamicItem = item as IDictionary<String, Object>;
            if (dynamicItem == null)
            {
                throw new ArgumentNullException("dynamicItem");
            }
            String name = String.Empty;
            Object value = null;

            for (int i = 0; i < reader.FieldCount; i++)
            {
                name = reader.GetName(i);
                value = reader[name];
                try
                {
                    value = ((value == null || value == DBNull.Value) ? null : value);
                    dynamicItem.Add(name, value);
                }
                catch (Exception ex)
                {
                    this.InternalTrace(TraceTypes.Error, "Chyba pri citani dynamic z SQL. {0} [{1}]", ex.Message, name);
                    throw;
                }
            }

            return dynamicItem;
        }
        /// <summary>
        /// Nacita objekt z sql readera
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="reader">Reader pomocou ktoreho citame data</param>
        /// <param name="item">Objekt ktoryc chceme aktualizovat</param>
        /// <returns>Objekt ktory necitavame</returns>
        private T InternalGetObject<T>(SqlDataReader reader, T item)
        {
            String name = String.Empty;
            ReflectionObjectItem properties = this.m_reflectionPropertyCollection.FindPropertyCollection(item.GetType());
            if (properties != null)
            {
                Object value = null;
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    name = reader.GetName(i);
                    ReflectionPropertyItem property = properties.FindProperty(name);
                    if (property != null && property.Property.CanRead)
                    {
                        if (property.ColumnAttribute != null)
                        {
                            value = reader[name];
                            try
                            {
                                value = ((value == null || value == DBNull.Value) ? null : value);
                                if (value == null && !property.ColumnAttribute.CanBeNull)
                                {
                                    throw new InvalidCastException(String.Format("Specified cast is not valid. [{0} - {1} - {2}]", item.GetType().Name, property.Property.Name, property.Property.PropertyType));
                                }
                                value = this.InternalUpdateValue(value);
                                property.Property.SetValue(item, value, null);
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
        /// Aktualizuje hodnotu skor ako dojde k jej spracovaniu
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme aktualizovat</param>
        /// <returns>Aktualizovana hodnota alebo povodna hodnota</returns>
        private Object InternalUpdateValue(Object value)
        {
            if (value != null)
            {
                if (value is String)
                {
                    return ((String)value).Trim();
                }
            }
            return value;
        }
        /// <summary>
        /// Vykona select pozadovanych dat
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="whereClause">Where podmienka</param>
        /// <param name="orderClause">Order klauzula</param>
        /// <returns>Kolekcia nacitanych dat</returns>
        private T InternalSelectFirstObject<T>(String whereClause, String orderClause)
        {
            //ziskame informacie o objekte
            ReflectionObjectItem properties = this.m_reflectionPropertyCollection.FindPropertyCollection(typeof(T));

            //overime whereClause 
            if (!String.IsNullOrWhiteSpace(whereClause))
            {
                //overime obsah
                if (whereClause.IndexOf("ORDER BY", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                    whereClause.IndexOf(" ASC ", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                    whereClause.IndexOf(" DESC ", StringComparison.CurrentCultureIgnoreCase) > 0)
                {
                    throw new Exception(String.Format("Whereclause can't contains order by part. [{0}]", whereClause));
                }
            }
            //overime orderClause 
            if (!String.IsNullOrWhiteSpace(orderClause))
            {
                //overime obsah
                if (orderClause.IndexOf("WHERE", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                    orderClause.IndexOf(">", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                    orderClause.IndexOf("<", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                    orderClause.IndexOf("=", StringComparison.CurrentCultureIgnoreCase) > 0)
                {
                    throw new Exception(String.Format("OrderClause can't contains where part. [{0}]", orderClause));
                }
            }
            //overime povinny atribut pre tuto metodu
            if (properties.TableAttribute == null)
            {
                throw new Exception("Missing attribute TableAttribute");
            }
            //informacie o properties
            if (properties.Count == 0)
            {
                throw new Exception("Missing attribute ColumnAttribute");
            }

            //spracujeme dommand do SQL
            using (SqlCommand command = new SqlCommand())
            {
                //vytvorime command
                String commandText = String.Format("SELECT TOP 1 * FROM [{0}]{1}{2}", properties.TableOrViewName, (String.IsNullOrWhiteSpace(whereClause) ? String.Empty : String.Format(" WHERE {0}", whereClause)), (String.IsNullOrWhiteSpace(orderClause) ? String.Empty : String.Format(" ORDER BY {0}", orderClause)));
                //vykoname prikad
                command.CommandText = commandText;
                return this.ReadFirstObject<T>(command);
            }
        }
        /// <summary>
        /// Vykona select pozadovanych dat
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="query">Query ktorym chceme nacitat data</param>
        /// <returns>Kolekcia nacitanych dat</returns>
        private T InternalSelectFirstObjectFromQuery<T>(String query)
        {
            //overime query
            if (String.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentNullException("query");
            }

            //spracujeme dommand do SQL
            using (SqlCommand command = new SqlCommand())
            {
                //vykoname prikad
                command.CommandText = query;
                return this.ReadFirstObject<T>(command);
            }
        }
        /// <summary>
        /// Vykona select s nacitanim dat do dynamic objektu
        /// </summary>
        /// <param name="query">Query na vykonanie selectu</param>
        /// <returns>Kolekcia dynamic objektov</returns>
        private List<dynamic> InternalSelectDynamicFromQuery(String query)
        {
            //overime query
            if (String.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentNullException("query");
            }

            //spracujeme dommand do SQL
            using (SqlCommand command = new SqlCommand())
            {
                //vykoname prikad
                command.CommandText = query;
                return this.ReadDynamicCollection(command);
            }
        }
        /// <summary>
        /// Vykona select pozadovanych dat
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="query">Query ktorym chceme nacitat data</param>
        /// <returns>Kolekcia nacitanych dat</returns>
        private List<T> InternalSelectFromQuery<T>(String query)
        {
            //overime query
            if (String.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentNullException("query");
            }

            //spracujeme dommand do SQL
            using (SqlCommand command = new SqlCommand())
            {
                //vykoname prikad
                command.CommandText = query;
                return this.ReadObjectCollection<T>(command);
            }
        }
        /// <summary>
        /// Nacita pocet dat
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="whereClause">Where podmienka</param>
        /// <returns>Pocet dat alebo null</returns>
        private Int32 InternalSelectCount<T>(String whereClause)
        {
            //ziskame informacie o objekte
            ReflectionObjectItem properties = this.m_reflectionPropertyCollection.FindPropertyCollection(typeof(T));

            //overime whereClause 
            if (!String.IsNullOrWhiteSpace(whereClause))
            {
                //overime obsah
                if (whereClause.IndexOf("ORDER BY", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                    whereClause.IndexOf(" ASC ", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                    whereClause.IndexOf(" DESC ", StringComparison.CurrentCultureIgnoreCase) > 0)
                {
                    throw new Exception("Whereclause can't contains order by part");
                }
            }

            //overime povinny atribut pre tuto metodu
            if (properties.TableAttribute == null && properties.ViewAttribute == null)
            {
                throw new Exception("Missing attribute TableAttribute or ViewAttribute");
            }

            //spracujeme dommand do SQL
            using (SqlCommand command = new SqlCommand())
            {
                //vytvorime command
                String commandText = String.Format("SELECT COUNT(*) FROM [{0}]{1}", properties.TableOrViewName,
                    (String.IsNullOrWhiteSpace(whereClause) ? String.Empty : String.Format(" WHERE {0}", whereClause)));

                //vykoname prikad
                command.CommandText = commandText;
                return (Int32)this.ExecuteScalar(command);
            }
        }
        /// <summary>
        /// Vykona select pozadovanych dat
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory chceme nacitat</typeparam>
        /// <param name="whereClause">Where podmienka</param>
        /// <param name="orderClause">Order klauzula</param>
        /// <returns>Kolekcia nacitanych dat</returns>
        private List<T> InternalSelect<T>(String whereClause, String orderClause, Nullable<UInt32> limit, Nullable<UInt32> page)
        {
            //ziskame informacie o objekte
            ReflectionObjectItem properties = this.m_reflectionPropertyCollection.FindPropertyCollection(typeof(T));

            //overime whereClause 
            if (!String.IsNullOrWhiteSpace(whereClause))
            {
                //overime obsah
                if (whereClause.IndexOf("ORDER BY", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                    whereClause.IndexOf(" ASC ", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                    whereClause.IndexOf(" DESC ", StringComparison.CurrentCultureIgnoreCase) > 0)
                {
                    throw new Exception("Whereclause can't contains order by part");
                }
            }
            //overime orderClause 
            if (!String.IsNullOrWhiteSpace(orderClause))
            {
                //overime obsah
                if (orderClause.IndexOf("WHERE", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                    orderClause.IndexOf(">", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                    orderClause.IndexOf("<", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                    orderClause.IndexOf("=", StringComparison.CurrentCultureIgnoreCase) > 0)
                {
                    throw new Exception("OrderClause can't contains where part");
                }
            }
            //overime povinny atribut pre tuto metodu
            if (properties.TableAttribute == null && properties.ViewAttribute == null)
            {
                throw new Exception("Missing attribute TableAttribute or ViewAttribute");
            }
            //informacie o properties
            if (properties.Count == 0)
            {
                throw new Exception("Missing attribute ColumnAttribute");
            }

            //spracujeme dommand do SQL
            using (SqlCommand command = new SqlCommand())
            {
                //vykoname prikad
                command.CommandText = this.InternalCreateCommandText(properties, whereClause, orderClause, limit, page);
                return this.ReadObjectCollection<T>(command);
            }
        }
        /// <summary>
        /// Vytvori command pre select podla parametrov
        /// </summary>
        /// <param name="properties">Property reflector na objekt</param>
        /// <param name="whereClause">Podmiebja</param>
        /// <param name="orderClause">Zoradenie</param>
        /// <param name="limit">Limit</param>
        /// <param name="page">Stranka</param>
        /// <returns>Command text</returns>
        private String InternalCreateCommandText(ReflectionObjectItem properties, String whereClause, String orderClause, Nullable<UInt32> limit, Nullable<UInt32> page)
        {
            StringBuilder commandTextBuilder = new StringBuilder();
            commandTextBuilder.Append("SELECT");
            if (!page.HasValue && limit.HasValue)
            {
                commandTextBuilder.AppendFormat(" TOP {0}", limit.Value);
            }
            commandTextBuilder.AppendFormat(" * FROM [{0}]", properties.TableOrViewName);
            if (!String.IsNullOrWhiteSpace(whereClause))
            {
                commandTextBuilder.AppendFormat(" WHERE {0}", whereClause);
            }
            if (!String.IsNullOrWhiteSpace(orderClause))
            {
                commandTextBuilder.AppendFormat(" ORDER BY {0}", orderClause);
            }
            if (page.HasValue && limit.HasValue)
            {
                commandTextBuilder.AppendFormat(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", ((page.Value - 1) * limit.Value), limit.Value);
            }
            return commandTextBuilder.ToString();
        }
        /*
        /// <summary>
        /// Vlozi pozadovany objekt do SQL
        /// </summary>
        /// <param name="item">objekt ktorych chceme vlozit</param>
        /// <returns>Kod navratovej hodnoty</returns>
        private int InternalInsertObjectWithProcedure(Object item)
        {
            //objekt musi byt zadany
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            //najdeme informacie o datovom type
            ReflectionObjectItem properties = this.m_reflectionPropertyCollection.FindPropertyCollection(item.GetType());

            //objekt musi obsahovat definiciu tabulky
            if (properties.TableAttribute == null)
            {
                throw new Exception("Missing attribute TableAttribute");
            }

            //overime nazov procedury
            if (String.IsNullOrWhiteSpace(((TableAttribute)properties.Attributes[0]).InsertProcedureName))
            {
                throw new Exception("InsertProcedureName is empty or null");
            }

            //informacie o properties
            if (properties.Count == 0)
            {
                throw new Exception("Missing attribute ColumnAttribute");
            }
  
            //vytvorime command
            List<SqlParameter> parameterCollection = new List<SqlParameter>();
            foreach (var value in properties.Values)
            {
                if (value.Attributes.Length == 1)
                {
                    ColumnAttribute attribude = (ColumnAttribute)value.Attributes[0];
                    if (!attribude.IsDbGenerated && attribude.IsRequiredWhenInserting)
                    {
                        SqlParameter parameter = new SqlParameter(value.Property.Name, attribude.Type);
                        parameter.Value = this.InternalValidateValue(value.Property.GetValue(item, null));
                        parameterCollection.Add(parameter);
                    }
                }
            }

            //return values
            SqlParameter returnValueParam = new SqlParameter("returnVal", SqlDbType.Int);
            returnValueParam.Direction = ParameterDirection.ReturnValue;
            parameterCollection.Add(returnValueParam);

            //spracujeme dommand do SQL
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = ((TableAttribute)properties.Attributes[0]).InsertProcedureName;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(parameterCollection.ToArray());
                this.ExecuteNonQuery(command);
                return returnValueParam.Value != null ? (int)returnValueParam.Value : -1;
            }
        }
        */
        /// <summary>
        /// Vlozi pozadovany objekt do SQL
        /// </summary>
        /// <param name="item">objekt ktorych chceme vlozit</param>
        private void InternalInsertObject(Object item)
        {
            //objekt musi byt zadany
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            //najdeme informacie o datovom type
            ReflectionObjectItem properties = this.m_reflectionPropertyCollection.FindPropertyCollection(item.GetType());

            //objekt musi obsahovat definiciu tabulky
            if (properties.TableAttribute == null)
            {
                throw new Exception("Missing attribute TableAttribute");
            }

            //najdeme vsetky property
            if (properties == null || properties.Count == 0)
            {
                throw new Exception("Missing attribute ColumnAttribute");
            }

            //vytvorime command
            List<SqlParameter> parameterCollection = new List<SqlParameter>();
            StringBuilder builder = new StringBuilder();
            StringBuilder values = new StringBuilder();
            builder.AppendFormat("INSERT INTO [{0}] (", properties.TableAttribute.Name);
            foreach (var value in properties.Values)
            {
                if (value.ColumnAttribute != null)
                {
                    ColumnAttribute attribude = value.ColumnAttribute;
                    if (!attribude.IsDbGenerated && attribude.IsRequiredWhenInserting)
                    {
                        builder.AppendFormat("[{0}], ", value.Property.Name);
                        values.AppendFormat("@{0}, ", value.Property.Name);
                        SqlParameter parameter = new SqlParameter(value.Property.Name, attribude.Type);
                        parameter.Value = this.InternalPrepareValue(value.Property.GetValue(item, null), attribude);
                        parameterCollection.Add(parameter);
                    }
                }
            }
            builder.Remove(builder.Length - 2, 2);
            values.Remove(values.Length - 2, 2);
            builder.AppendFormat(") VALUES ({0})", values.ToString());

            //spracujeme dommand do SQL
            using (SqlCommand command = new SqlCommand())
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

            //najdeme informacie o datovom type
            ReflectionObjectItem properties = this.m_reflectionPropertyCollection.FindPropertyCollection(item.GetType());

            //objekt musi obsahovat definiciu tabulky
            if (properties.TableAttribute == null)
            {
                throw new Exception("Missing attribute TableAttribute");
            }
            //informacie o properties
            if (properties.Count == 0)
            {
                throw new Exception("Missing attribute ColumnAttribute");
            }
            if (properties.Count == 1)
            {
                throw new Exception("Too little attribute ColumnAttribute");
            }

            //overime primarny kluc
            List<ReflectionPropertyItem> primaryKeyPropertyInfo = this.InternalFindPropertiesWithPrimaryKeyState(properties);
            if (primaryKeyPropertyInfo.Count == 0)
            {
                throw new MissingPrimaryKeyException();
            }
            if (primaryKeyPropertyInfo.Count > 1)
            {
                throw new Exception("Duplicate PrimaryKey state");
            }

            //vytvorime command
            List<SqlParameter> parameterCollection = new List<SqlParameter>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("UPDATE [{0}] SET ", properties.TableAttribute.Name);
            foreach (var property in properties.Values)
            {
                if (property.ColumnAttribute != null)
                {
                    ColumnAttribute attribude = property.ColumnAttribute;
                    if (!attribude.IsDbGenerated && attribude.IsRequiredWhenUpdating && !attribude.IsPrimaryKey)
                    {
                        builder.AppendFormat("[{0}] = @{0}, ", property.Property.Name);
                        SqlParameter parameter = new SqlParameter(property.Property.Name, attribude.Type);
                        parameter.Value = this.InternalPrepareValue(property.Property.GetValue(item, null), attribude);
                        parameterCollection.Add(parameter);
                    }
                }
            }

            //chyba, nie su ziadne SET parametre
            if (parameterCollection == null || parameterCollection.Count == 0)
            {
                throw new Exception("Too little attribute ColumnAttribute with update ");
            }

            builder.Remove(builder.Length - 2, 2);
            builder.AppendFormat(" WHERE [{0}] = @{0}", primaryKeyPropertyInfo[0].Property.Name);
            SqlParameter primaryKeyParameter = new SqlParameter(primaryKeyPropertyInfo[0].Property.Name, primaryKeyPropertyInfo[0].ColumnAttribute.Type);
            primaryKeyParameter.Value = primaryKeyPropertyInfo[0].Property.GetValue(item, null);
            parameterCollection.Add(primaryKeyParameter);

            //vykoname command
            using (SqlCommand command = new SqlCommand())
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
        /// <param name="attribute">Atributy stlpca v tabulke</param>
        /// <returns>Hodnota</returns>
        private Object InternalPrepareValue(Object value, ColumnAttribute attribute)
        {
            value = value == null ? DBNull.Value : value;
            return this.InternalTruncateValue(value, attribute);
        }
        /// <summary>
        /// Vykona skratenie hodnoty ak je dostupna a je to povolene
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme skratit</param>
        /// <param name="attribute">Atributy stlpca v tabulke</param>
        /// <returns>Skratena hodnota alebo povodna</returns>
        public Object InternalTruncateValue(Object value, ColumnAttribute attribute)
        {
            if (value != null && this.TruncateValue && attribute.Size != int.MaxValue)
            {
                if (value is String)
                {
                    value = ((String)value).TruncateLongString(attribute.Size);
                }
            }
            return value;
        }
        /// <summary>
        /// Vyhlada property ktore su nastavene ako primarny kluc objektu
        /// </summary>
        /// <param name="propertyInfo">Kolekcia v ktorej chceme vyhladat</param>
        /// <returns>Property ktore obsahuju nastaveny stav primarneho kluca</returns>
        private List<ReflectionPropertyItem> InternalFindPropertiesWithPrimaryKeyState(ReflectionObjectItem properties)
        {
            List<ReflectionPropertyItem> collection = new List<ReflectionPropertyItem>();
            foreach (var property in properties.Values)
            {
                if (property.ColumnAttribute != null && property.ColumnAttribute.IsPrimaryKey)
                {
                    collection.Add(property);
                }
            }
            return collection;
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
