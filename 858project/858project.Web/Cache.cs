using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project858.Web
{
    /// <summary>
    /// Buffer na buffrovanie dat
    /// </summary>
    public sealed class Cache : Dictionary<String, CacheItem>
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public Cache()
        {
            this.Timeout = new TimeSpan(0, 10, 0);
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Interval platnosti polozky
        /// </summary>
        public TimeSpan Timeout { get; set; }
        #endregion

        #region - Variable -
        /// <summary>
        /// Synchronizacny objekt na pristup k datam
        /// </summary>
        private readonly Object m_lockObject = new Object();
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Prida data do buffra
        /// </summary>
        /// <param name="key">Kluc ktory chceme pridat</param>
        /// <param name="data">Data </param>
        public void Add(String key, Object data)
        {
            if (data != null)
            {
                this.Add(key, data, null);
            }
        }
        /// <summary>
        /// Prida data do buffra
        /// </summary>
        /// <param name="key">Kluc ktory chceme pridat</param>
        /// <param name="data">Data </param>
        /// <param name="timeout">Timeout platnosti dat </param>
        public void Add(String key, Object data, Nullable<TimeSpan> timeout)
        {
            this.Add(key, new CacheItem()
            {
                Id = key,
                Date = DateTime.Now,
                Data = data,
                Timeout = null
            });
        }
        /// <summary>
        /// Prida polozku do buffra dat
        /// </summary>
        /// <param name="key">Jedinecny identifikator objektu</param>
        /// <param name="item">Polozka ktoru buffrujeme</param>
        public new void Add(String key, CacheItem item)
        {
            if (String.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            lock (m_lockObject)
            {
                //zmazeme stare data
                this.InternalClearOldItem();

                //pridame data ak neexistuju
                if (!base.ContainsKey(key))
                {
                    //pridame nove data
                    base.Add(key, item);
                }
                else
                {
                    //nahradime objekt
                    base[key] = item;
                }
            }
        }
        /// <summary>
        /// Vymaze vsetky data
        /// </summary>
        public new void Clear()
        {
            lock (m_lockObject)
            {
                base.Clear();
            }
        }
        /// <summary>
        /// Overi ci v buffry existuje pozadovana hodnota
        /// </summary>
        /// <param name="key">Kluc ktory hladame</param>
        /// <returns>True = data existuju, inak false</returns>
        public new bool ContainsKey(String key)
        {
            lock (m_lockObject)
            {
                //zmazeme stare data
                this.InternalClearOldItem();

                //overime ci data existuju
                return base.ContainsKey(key);
            }
        }
        /// <summary>
        /// Overi ci pozadovana hodnota existuje, ak ano vrati jej data
        /// </summary>
        /// <param name="key">Kluc ktory hladame</param>
        /// <returns>Pozadovane data alebo null</returns>
        public CacheItem GetValue(String key)
        {
            if (this.ContainsKey(key))
            {
                CacheItem item = null;
                base.TryGetValue(key, out item);
                return item;
            }
            return null;
        }
        /// <summary>
        /// Vrati hodnotu z cache
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory ocakavame</typeparam>
        /// <param name="key">Jedinecny identifikator objektu</param>
        /// <returns>Object alebo jeho default</returns>
        public T GetValue<T>(String key)
        {
            if (this.ContainsKey(key))
            {
                CacheItem item = null;
                if (base.TryGetValue(key, out item))
                {
                    if (item.Data.GetType() == typeof(T))
                    {
                        return (T)item.Data;
                    }
                }
            }
            return default(T);
        }
         /// <summary>
        /// Vrati hodnotu z cache
        /// </summary>
        /// <typeparam name="T">Typ objektu ktory ocakavame</typeparam>
        /// <param name="key">Jedinecny identifikator objektu</param>
        /// <param name="method">Metoda na nacitanie dat</param>
        /// <returns>Object alebo jeho default</returns>
        public T GetValue<T>(String key, Func<T> method)
        {
            if (this.ContainsKey(key))
            {
                CacheItem item = null;
                if (base.TryGetValue(key, out item))
                {
                    if (item.Data.GetType() == typeof(T))
                    {
                        return (T)item.Data;
                    }
                }
            }
            T value = method();
            if (value != null)
            {
                this.Add(key, value);
            }
            return value;
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// Vymaze data ktorym vyprsal interval platnosti dat
        /// </summary>
        private void InternalClearOldItem()
        {
            List<String> collection = new List<String>();
            foreach (KeyValuePair<String, CacheItem> item in this)
            {
                if ((DateTime.Now - item.Value.Date) > (item.Value.Timeout == null ? this.Timeout : item.Value.Timeout))
                {
                    collection.Add(item.Key);
                }
            }
            foreach (String key in collection)
            {
                base.Remove(key);
            }
        }
        #endregion
    }
}
