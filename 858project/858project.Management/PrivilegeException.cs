using System;
using System.Collections.Generic;
using System.Text;

namespace Project858.Management
{
    /// <summary>
    /// The exception that is thrown when an error occures when requesting a specific privilege.
    /// </summary>
    public class PrivilegeException : Exception
    {
        #region - Constructor -
        /// <summary>
        /// Initializes a new instance of the PrivilegeException class.
        /// </summary>
        public PrivilegeException() : base() { }
        /// <summary>
        /// Initializes a new instance of the PrivilegeException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public PrivilegeException(string message) : base(message) { }
        #endregion
    }
}
