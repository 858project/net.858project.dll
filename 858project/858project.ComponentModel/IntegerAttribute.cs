using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Project858.ComponentModel
{
    /// <summary>
    /// Validacia textu
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class IntegerAttribute : ValidationAttribute
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public IntegerAttribute()
        {
            this.MinValue = Int32.MinValue;
            this.MaxValue = Int32.MaxValue;
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Nazov metody ktora ma validovat uvedenu polozku
        /// </summary>
        public String ClientValidationHandler { get; set; }
        /// <summary>
        /// Definuje ci je hodnota vyzadovana alebo nie
        /// </summary>
        public Boolean Required { get; set; }
        /// <summary>
        /// Definuje minimalnu dlzku textu ktoru musi hodnota splnat
        /// </summary>
        public Int32 MinValue { get; set; }
        /// <summary>
        /// Definuje maximalnu dlzku textu ktoru musi hodnota splnat
        /// </summary>
        public Int32 MaxValue { get; set; }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// Overi platnost hodnoty podla nastavenych kriterii
        /// </summary>
        /// <param name="value">Hodnota ktoru validujeme</param>
        /// <param name="validationContext">ValidationContext</param>
        /// <returns>ValidationResult</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //hodnota moze byt null
            if (value == null && this.Required == false)
            {
                return ValidationResult.Success;
            }
            //ak treba hodnotu validovat
            else if (value != null && (value is Int32))
            {
                //overime rozsah
                if (this.MinValue <= ((Int32)value) && this.MaxValue >= ((Int32)value))
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult(String.Format("Int32 '{0}' and value '{1}' is not valid! [{2} - {3}]", validationContext.DisplayName, (value == null ? "NULL" : value.ToString()), this.MinValue, this.MaxValue));
        }
        #endregion
    }
}
