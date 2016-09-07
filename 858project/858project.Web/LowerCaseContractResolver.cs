using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project858.Web
{
    /// <summary>
    /// Resolver na upravu json vystupu
    /// </summary>
    public sealed class LowerCaseContractResolver : DefaultContractResolver
    {
        #region - Private Methods -
        /// <summary>
        /// Update property name
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <returns>Updated property name</returns>
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
        }
        #endregion
    }
}

