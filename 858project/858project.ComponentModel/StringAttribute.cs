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
    public sealed class StringAttribute : ValidationAttribute
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public StringAttribute()
        {
            this.MinLength = 0;
            this.MaxLength = Int32.MaxValue;
            this.Type = StringTypes.Text;
        }
        #endregion

        #region - Public Enum -
        /// <summary>
        /// Defnije typy textu
        /// </summary>
        public enum StringTypes
        {
            /// <summary>
            /// text
            /// </summary>
            Text,
            /// <summary>
            /// text zlozeny z alpha znakov
            /// </summary>
            Alpha,
            /// <summary>
            /// text zlozeny z number znakov
            /// </summary>
            Numeric,
            /// <summary>
            /// text zlozeny z alpha alebo numeric znakov
            /// </summary>
            AlphaNumeric,
            /// <summary>
            /// text vo formate e-mailu
            /// </summary>
            Email
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Nazov metody ktora ma validovat uvedenu polozku
        /// </summary>
        public String ClientValidationHandler { get; set; }
        /// <summary>
        /// Place holder na text
        /// </summary>
        public String PlaceHolder { get; set; }
        /// <summary>
        /// Typ stringu
        /// </summary>
        public StringTypes Type { get; set; }
        /// <summary>
        /// Definuje ci je hodnota vyzadovana alebo nie
        /// </summary>
        public Boolean Required { get; set; }
        /// <summary>
        /// Definuje minimalnu dlzku textu ktoru musi hodnota splnat
        /// </summary>
        public Int32 MinLength { get; set; }
        /// <summary>
        /// Definuje maximalnu dlzku textu ktoru musi hodnota splnat
        /// </summary>
        public Int32 MaxLength { get; set; }
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
            else if (value != null && (value is String) && !String.IsNullOrWhiteSpace(value as String))
            {
                //overime minimalnu hodnotu
                if (this.MinLength < 0)
                {
                    this.MinLength = 0;
                }
                //overime rozsah
                if (this.MinLength > this.MaxLength)
                {
                    Int32 tempValue = this.MinLength;
                    this.MinLength = this.MaxLength;
                    this.MaxLength = tempValue;
                }
                //overime string
                Boolean flag = (value as String).Validate(this.MinLength, this.MaxLength);
                if (flag)
                {
                    //overime typ stringu
                    flag = this.InternalValidateStringType((value as String), this.Type);
                    if (flag)
                    {
                        return ValidationResult.Success;
                    }
                }
            }
            return new ValidationResult(String.Format("String '{0}' and value '{1}' is not valid!", validationContext.DisplayName, (value == null ? String.Empty : value.ToString())));
        }
        /// <summary>
        /// Overi spravnost dat
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type">Typ validacie ktoru chceme vykonat</param>
        /// <returns>True = text je spravneho typu, inak false</returns>
        private Boolean InternalValidateStringType(String value, StringTypes type)
        {
            switch (type)
            {
                case StringTypes.Alpha:
                    return value.IsAlpha();
                case StringTypes.AlphaNumeric:
                    return value.IsAlphaNumeric();
                case StringTypes.Numeric:
                    return value.IsNumeric();
                case StringTypes.Email:
                    return Utility.ValidateMailAddress(value);
                default:
                    return true;
            }
        }
        #endregion
    }
}
