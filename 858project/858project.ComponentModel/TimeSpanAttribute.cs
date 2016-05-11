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
    public sealed class TimeSpanAttribute : ValidationAttribute
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public TimeSpanAttribute()
        {
            this.MinValue = TimeSpan.MinValue.ToString();
            this.MaxValue = TimeSpan.MaxValue.ToString();
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
        /// Definuje minimalnu hdonotu
        /// </summary>
        public String MinValue { get; set; }
        /// <summary>
        /// Definuje maximalnu hodnotu
        /// </summary>
        public String MaxValue { get; set; }
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
            else if (value != null && (value is TimeSpan))
            {
                //overime rozsah
                if (TimeSpan.Parse(this.MinValue) <= ((TimeSpan)value) && TimeSpan.Parse(this.MaxValue) >= ((TimeSpan)value))
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult(String.Format("TimeSpan '{0}' and value '{1}' is not valid! [{2} - {3}]", validationContext.DisplayName, (value == null ? "NULL" : value.ToString()), this.MinValue, this.MaxValue));
        }
        #endregion
    }
}
