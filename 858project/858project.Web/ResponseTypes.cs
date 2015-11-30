using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project858.Web
{
    /// <summary>
    /// Definuje navratove hodnoty servera
    /// </summary>
    public enum ResponseTypes : byte
    {
        Successfully = 0,
        RedirectToAction = 1,
        InternalError = 2,
        ModelIsNotValidError = 3,
        IdenticalPasswordError = 4,
        EmailExistError = 5,
        WrongUsernameOrPasswordError = 6,
        EmailNotExistError = 7,
        TimeoutError = 8,
        FileIsNotValid = 9,
        DataIsNotValidError = 10,
        UserIsBlocked = 11,
        UserIsPermanentlyBlocked = 12,
        UserIsNotConfirmed = 13,
        UserIsNotLogged = 14,
        Rejected = 15
    }
}
