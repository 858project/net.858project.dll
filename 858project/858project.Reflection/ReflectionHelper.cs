using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Project858.Reflection
{
    /// <summary>
    /// Helper na refaktoring objektu a jeho properties
    /// </summary>
    public class ReflectionHelper
    {
        #region - Constructors -
        /// <summary>
        /// Initialize static class
        /// </summary>
        static ReflectionHelper()
        {
            ReflectionHelper.m_collection = new Dictionary<Type, ReflectionType>();
        }
        #endregion

        #region - Variables -
        /// <summary>
        /// Kolekcia ulozenych a nacitanych typov
        /// </summary>
        private static Dictionary<Type, ReflectionType> m_collection = null;
        #endregion

        #region - Public Static Methods -
        /// <summary>
        /// Vrati property z datoveho typu podla mena
        /// </summary>
        /// <param name="type">Typ objektu z ktoreho chceme vratit property</param>
        /// <param name="name">Meno property ktoru chceme nacitat</param>
        /// <returns>PropertyInfo alebo null</returns>
        public static ReflectionProperty GetReflectionProperty(Type type, String name)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name is not valid!");
            }
            //ziskame reflekciu
            var reflectionType = ReflectionHelper.InternalGetType(type);
            return reflectionType.GetReflectionProperty(name);
        }
        /// <summary>
        /// Vrati property z datoveho typu podla mena
        /// </summary>
        /// <param name="type">Typ objektu z ktoreho chceme vratit property</param>
        /// <param name="name">Meno property ktoru chceme nacitat</param>
        /// <returns>PropertyInfo alebo null</returns>
        public static PropertyInfo GetProperty(Type type, String name)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name is not valid!");
            }
            //ziskame reflekciu
            var reflectionType = ReflectionHelper.InternalGetType(type);
            return reflectionType.GetProperty(name);
        }
        /// <summary>
        /// Vrati hodnotu pozadovanej property
        /// </summary>
        /// <param name="instance">Instancia objektu z ktoreho hodnotu chceme ziskat</param>
        /// <param name="name">Meno property ktorej hodnotu chceme ziskat</param>
        /// <returns>Hodnota property alebo null</returns>
        /// <exception cref="ArgumentNullException">
        /// Neinicializovany argument instance
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Nezadane meno property
        /// </exception>
        public static Object GetPropertyValue(Object instance, String name)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name is not valid!");
            }
            //ziskame reflekciu
            var reflectionType = ReflectionHelper.InternalGetType(instance.GetType());
            return reflectionType.GetPropertyValue(instance, name);
        }
        /// <summary>
        /// Nastavi pozadovanu hodnotu property
        /// </summary>
        /// <param name="instance">Instancia objektu do ktoreho chceme hodnotu nastavit</param>
        /// <param name="name">Meno property ktorej chceme hodnotu nastavit</param>
        /// <param name="value">Hodnota ktoru chceme nastavit</param>
        /// <param name="changeType">Definuje ci si zelame zmenit typ na typ property</param>
        /// <exception cref="ArgumentNullException">
        /// Neinicializovany argument instance
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Nezadane meno property
        /// </exception>
        public static void SetPropertyValue(Object instance, String name, Object value, Boolean changeType = false)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name is not valid!");
            }
            //ziskame reflekciu
            var reflectionType = ReflectionHelper.InternalGetType(instance.GetType());
            reflectionType.SetPropertyValue(instance, name, value, changeType);
        }
        #endregion

        #region - Private Static Methods -
        /// <summary>
        /// Vrati reflekciu pre pozadovany typ
        /// </summary>
        /// <param name="type">Typ ktoreho reflekciu pozadujeme</param>
        /// <returns>Reflekcia alebo null</returns>
        private static ReflectionType InternalGetType(Type type)
        {
            if (ReflectionHelper.m_collection.ContainsKey(type))
            {
                return ReflectionHelper.m_collection[type];
            }
            var reflectionType = new ReflectionType(type);
            ReflectionHelper.m_collection.Add(type, reflectionType);
            return reflectionType;
        }
        #endregion
    }
}
