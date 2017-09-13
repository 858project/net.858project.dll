using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Project858.Reflection
{
    /// <summary>
    /// Reflekcia typu
    /// </summary>
    public class ReflectionType
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="type">Typ objektu</param>
        public ReflectionType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            this.Type = type;
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Datovy typ ktoreho sa reflekcia tyka
        /// </summary>
        public Type Type { get; private set; }
        /// <summary>
        /// Kolekcia property objektu
        /// </summary>
        public Dictionary<String, ReflectionProperty> PropertyCollection
        {
            get
            {
                if (this.m_propertyCollection == null)
                {
                    this.m_propertyCollection = this.InternalInitializePropertyCollection();
                }
                return this.m_propertyCollection;
            }
        }
        #endregion

        #region - Variables -
        /// <summary>
        /// Kolekcia property objektu
        /// </summary>
        public Dictionary<String, ReflectionProperty> m_propertyCollection = null;
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Vrati property z datoveho typu podla mena
        /// </summary>
        /// <param name="type">Typ objektu z ktoreho chceme vratit property</param>
        /// <param name="name">Meno property ktoru chceme nacitat</param>
        /// <returns>PropertyInfo alebo null</returns>
        public ReflectionProperty GetReflectionProperty(String name)
        {
            //ziskame property
            var propertyCollection = this.PropertyCollection;

            //prejdeme vsetky property
            foreach (var property in propertyCollection)
            {
                if (property.Key.CompareTo(name) == 0)
                {
                    return property.Value;
                }
            }
            return null;
        }
        /// <summary>
        /// Vrati property z datoveho typu podla mena
        /// </summary>
        /// <param name="type">Typ objektu z ktoreho chceme vratit property</param>
        /// <param name="name">Meno property ktoru chceme nacitat</param>
        /// <returns>PropertyInfo alebo null</returns>
        public PropertyInfo GetProperty(String name)
        {
            //ziskame property
            var propertyCollection = this.PropertyCollection;

            //meno je vzdy malym
            name = name.ToLower();

            //overime ci property existuje
            if (propertyCollection.ContainsKey(name))
            {
                return propertyCollection[name].Property;
            }
            return null;
        }
        /// <summary>
        /// Vrati hodnotu pozadovanej property
        /// </summary>
        /// <param name="instance">Instancia objektu z ktoreho hodnotu chceme ziskat</param>
        /// <param name="name">Meno property ktorej hodnotu chceme ziskat</param>
        /// <returns>Hodnota property alebo null</returns>
        public Object GetPropertyValue(Object instance, String name)
        {
            PropertyInfo info = this.GetProperty(name);
            if (info != null)
            {
                return info.GetValue(instance, null);
            }
            return null;
        }
        /// <summary>
        /// Nastavi pozadovanu hodnotu property
        /// </summary>
        /// <param name="instance">Instancia objektu do ktoreho chceme hodnotu nastavit</param>
        /// <param name="name">Meno property ktorej chceme hodnotu nastavit</param>
        /// <param name="value">Hodnota ktoru chceme nastavit</param>
        /// <param name="changeType">Definuje ci si zelame zmenit typ na typ property</param>
        public void SetPropertyValue(Object instance, String name, Object value, Boolean changeType = false)
        {
            PropertyInfo info = this.GetProperty(name);
            if (info != null)
            {
                Object internalValue = value != null && changeType ? Convert.ChangeType(value, info.PropertyType) : value;
                info.SetValue(instance, internalValue, null);
            }
        }
        /// <summary>
        /// This function return first custom attribute
        /// </summary>
        /// <typeparam name="T">Type of attribute</typeparam>
        /// <returns>Attribute or null</returns>
        public T GetCustomAttribute<T>()
        {
            //get all attribute
            Object[] collection = this.Type.GetCustomAttributes(typeof(T), true);

            //check
            if (collection != null && collection.Count() > 0)
            {
                return (T)collection[0];
            }
            return default(T);
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// Inicializuje kolekciu property pre aktualny typ
        /// </summary>
        /// <returns></returns>
        private Dictionary<String, ReflectionProperty> InternalInitializePropertyCollection()
        {
            //inicializujeme kolekciu dat
            Dictionary<String, ReflectionProperty> collection = new Dictionary<String, ReflectionProperty>();

            //ziskame vsetky property z objektu
            var properties = this.Type.GetProperties();

            //prejdeme vsetky polozky
            foreach (var property in properties)
            {
                collection.Add(property.Name.ToLower(), new ReflectionProperty(property));
            }

            //vratime kolekciu property objektov
            return collection;
        }
        #endregion
    }
}
