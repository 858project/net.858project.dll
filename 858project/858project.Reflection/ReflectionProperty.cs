﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Project858.Reflection
{
    /// <summary>
    /// Reflekcia property objektu
    /// </summary>
    public class ReflectionProperty
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="property">Property</param>
        public ReflectionProperty(PropertyInfo property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }
            this.Property = property;
            this.Name = property.Name;
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Meno property
        /// </summary>
        public String Name { get; private set; }
        /// <summary>
        /// Property ktoru reprezentuje typ
        /// </summary>
        public PropertyInfo Property { get; private set; }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Vrati pozadovany vlastny atribut propety
        /// </summary>
        /// <param name="type">Typ atributu ktory ziadame</param>
        /// <returns>Atribut alebo null</returns>
        public Object[] GetCustomAttributes(Type type)
        {
            return this.Property.GetCustomAttributes(type, true);
        }
        #endregion
    }
}