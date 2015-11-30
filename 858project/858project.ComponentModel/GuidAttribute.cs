using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Project858.ComponentModel
{
    /// <summary>
    /// Validacia guid
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class GuidAttribute : ValidationAttribute
    {
        #region - Properties -
        /// <summary>
        /// Nazov metody ktora ma validovat uvedenu polozku
        /// </summary>
        public String ClientValidationHandler { get; set; }
        /// <summary>
        /// Definuje ci je hodnota vyzadovana alebo nie
        /// </summary>
        public Boolean Required { get; set; }
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
            //ak ide o klasicky guid
            if (value is Guid)
            {
                //ak hodnota nie je vyzadovana, resp. je ale nie je empty
                if (!this.Required || (this.Required && !((Guid)value).IsEmpty()))
                {
                    
                    return ValidationResult.Success;
                }
            }
            //ak ide o guid s moznostou null
            else if (value == null || value is Nullable<Guid>)
            {
                //ak hodnota nie je vyzadovana a je null alebo ak je hodnota vyzadovana a zaroven nie je Empty
                if ((!this.Required && value == null) || (this.Required && !((Nullable<Guid>)value).Value.IsEmpty())) 
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult(String.Format("Guid '{0}' and value '{1}' is not valid! [Required: {2}]", validationContext.DisplayName, (value == null ? String.Empty : value.ToString()), this.Required));
        }
        #endregion
    }
}
