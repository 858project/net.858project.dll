using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project858.Web
{
    /// <summary>
    /// Handler na spracovanie udalosti uzivatela
    /// </summary>
    public interface IUserBaseHandler
    {
        #region - Public Methods -
        /// <summary>
        /// Vykona prihlasenie uzivatela
        /// </summary>
        /// <param name="name">Meno uzivatela</param>
        /// <param name="password">Heslo uzivatela</param>
        /// <returns>Prihlaseny uzivatel alebo null</returns>
        UserBase Login(String name, String password);
        /// <summary>
        /// Overi spravnost tokenu a to ci je uzivatel stale prihlaseny
        /// </summary>
        /// <param name="token">Token uzivatela</param>
        /// <param name="hostAddress">IP adresa uzivatela odkial sa prihlasil</param>
        /// <returns>Token uzivatela</returns>
        Boolean UserBaseValidateToken(String token, String hostAddress);
        /// <summary>
        /// Vrati id uzivatela z tokenu
        /// </summary>
        /// <param name="token">Token uzivatela</param>
        /// <param name="hostAddress">IP adresa uzivatela odkial sa prihlasil</param>
        /// <returns>Token uzivatela</returns>
        Nullable<Guid> UserBaseGetUserLoginIdFromToken(String token, String hostAddress);
        /// <summary>
        /// Vygeneruje token obsahujuci dolezite data na validaciu uzivatela
        /// </summary>
        /// <param name="userId">Id uzivatela</param>
        /// <param name="hostAddress">IP adresa uzivatela odkial sa prihlasil</param>
        /// <returns>Token uzivatela</returns>
        String UserBaseGetUserToken(Guid userId, String hostAddress);
        /// <summary>
        /// Vrati host adress v pripade specialnych pripadov
        /// </summary>
        /// <returns>Host address alebo null</returns>
        String UserBaseGetHostAddress();
        /// <summary>
        /// Vrati uzivatela na zaklade id prihlasenia a IP adresy z ktorej uzivatel prichadza
        /// </summary>
        /// <param name="token">Id prihlasenia uzivatela</param>
        /// <param name="hostAddress">IP adresa z ktorej uzivatel prichadza</param>
        /// <returns>User alebo null</returns>
        UserBase UserBaseGetUserFromUserLoginId(Guid id, String hostAddress);
        /// <summary>
        /// Vykona odhlasenie uzivatela na zaklade Id prihlasenia
        /// </summary>
        /// <param name="id">Token uzivatela</param>
        void UserBaseUserLogOff(Guid id);
        #endregion
    }
}
