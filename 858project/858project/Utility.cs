/*
The MIT License
Copyright 2012-2015 (c) 858 Project s.r.o. <info@858project.com>

Permission is hereby granted, free of charge, to any person obtaining a
copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to permit
persons to whom the Software is furnished to do so, subject to the
following conditions:

The above copyright notice and this permission notice shall be included
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Security.Cryptography;
using Project858.Diagnostics;
using System.Diagnostics;
using System.Data;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using Microsoft.Win32;

namespace Project858
{
    /// <summary>
    /// Trieda implementujuca pomocne metody.
    /// </summary>
    public static class Utility
    {
        #region - Constant -
        /// <summary>
        /// Regex na overenie stringu ci obsahuje len alpha znaky.
        /// Je nutne urobit Replace("LENGTH", "pozadovana dlzka") alebo vyuzit Utlity.GetRegexPattern(regexConstant, length)
        /// </summary>
        public const String REGEX_ALPHA = @"^[a-zA-Z]{LENGTH,}$";
        /// <summary>
        /// Regex na overenie dlzky stringu
        /// Je nutne urobit Replace("LENGTH", "pozadovana dlzka") alebo vyuzit Utlity.GetRegexPattern(regexConstant, length)
        /// </summary>
        public const String REGEX_LENGTH = @"^.{LENGTH,}$";
        /// <summary>
        /// Regex na overenie stringu ci obsahuje len ciselne znaky.
        /// Je nutne urobit Replace("LENGTH", "pozadovana dlzka") alebo vyuzit Utlity.GetRegexPattern(regexConstant, length)
        /// </summary>
        public const String REGEX_NUMERIC = @"^[0-9]{LENGTH,}$";
        /// <summary>
        /// Regex na overenie stringu ci obsahuje len alpha numericke znaky
        /// Je nutne urobit Replace("LENGTH", "pozadovana dlzka") alebo vyuzit Utlity.GetRegexPattern(regexConstant, length)
        /// </summary>
        public const String REGEX_ALPHANUMERIC = @"^[0-9a-zA-Z]{LENGTH,}$";
        /// <summary>
        /// Regex na overenie stringu ci obsahuje len alpha numericke znaky, pricom alphanumericke su len male
        /// Je nutne urobit Replace("LENGTH", "pozadovana dlzka") alebo vyuzit Utlity.GetRegexPattern(regexConstant, length)
        /// </summary>
        public const String REGEX_ALPHANUMERIC_SMALL = @"^[0-9a-z]{LENGTH,}$";
        /// <summary>
        /// Kryptovacie heslo na ulozenie konfiguracie
        /// </summary>
        public const String CONFIGURATION_PASSWORD = "39dare54m48ndgdf543rf684t540fjetr4n444f9fds9";
        /// <summary>
        /// Kryptovacie heslo na ulozenie konfiguracie sluzby
        /// </summary>
        public const String WINDOWS_SERVICE_CONFIGURATION_PASSWORD = "390fmn40fm48nd943fh48450fje8434nf944f94f9";
        #endregion

        #region - Public Static Method -
        /// <summary>
        /// This function returns bit value from position
        /// </summary>
        /// <param name="data">byte</param>
        /// <param name="position">Position</param>
        /// <returns>True = bit = 1, False = bit = 0</returns>
        public static Boolean GetBitValue(Byte data, int position)
        {
            if ((((data) >> (position)) & 1) == 0)
                return false;
            else
                return true;
        }
        /// <summary>
        /// Vrati extension z mime typu
        /// </summary>
        /// <param name="type">Mime type z ktoreho chceme nacitat extension</param>
        /// <returns>Extension alebo null</returns>
        public static String GetFileExtensionFromMimeTypes(String type)
        {
            RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + type, false);
            Object value = key != null ? key.GetValue("Extension", null) : null;
            return value != null ? value.ToString() : string.Empty;
        }
        /// <summary>
        /// Zaloguje sql parametre
        /// </summary>
        /// <param name="command">Command ktory chceme zalogovat</param>
        public static String GetTraceString(SqlCommand command)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("SQL Command: '{0}'", command.CommandText);
            if (command.Parameters != null && command.Parameters.Count > 0)
            {
                foreach (SqlParameter parameter in command.Parameters)
                {
                    builder.Append(Environment.NewLine);
                    Object value = parameter.Value == null ? DBNull.Value : parameter.Value;
                    builder.AppendFormat("SQL Parameter: {0} = '{1}' [{2}]", parameter.ParameterName, value, parameter.SqlDbType);
                }
            }
            return builder.ToString();
        }
        /// <summary>
        /// Prekonvertuje objekt do json stringu
        /// </summary>
        /// <param name="value">Objekt ktory chceme konverotvat</param>
        /// <returns>Json text alebo null</returns>
        public static String ConvertObjectToJson(Object value)
        {
            if (value != null)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(value);
            }
            return null;
        }
        /// <summary>
        /// Dekoduje vstupnu hodnotu na zaklade vstupneho kluca a algoritmu
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Vstupna hodnota je null alebo empty
        /// </exception>
        /// <param name="key">Klud na dekodovanie</param>
        /// <param name="value">Hodnota ktoru chceme dekodovat</param>
        /// <returns>Dekodovana hodnota alebo null</returns>
        public static unsafe String DecodeValue(String key, String value)
        {
            if (String.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }
            if (String.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException("value");
            }
            byte[] bValue = Convert.FromBase64String(value);
            byte[] bKey = Encoding.UTF8.GetBytes(Utility.ReverseString(key));
            int valueLength = bValue.Length;
            int keyLength = bKey.Length;
            byte[] tempValue = new byte[valueLength - 1];
            Buffer.BlockCopy(bValue, 1, tempValue, 0, valueLength - 1);
            for (int i = 0; i < valueLength - 1; i++)
            {
                tempValue[i] = (byte)(tempValue[i] ^ bKey[(i * 2) % keyLength] ^ bValue[0]);
            }
            return Encoding.UTF8.GetString(tempValue);
        }
        /// <summary>
        /// Enkoduje vstupnu hodnotu na zaklade vstupneho kluca a algoritmu
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Vstupna hodnota je null alebo empty
        /// </exception>
        /// <param name="key">Klud na enkodovanie</param>
        /// <param name="value">Hodnota ktoru chceme enkodovat</param>
        /// <returns>Enkodovana hodnota alebo null</returns>
        public static unsafe String EncodeValue(String key, String value)
        {
            if (String.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }
            if (String.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException("value");
            }
            byte[] bValue = Encoding.UTF8.GetBytes(value);
            byte[] bKey = Encoding.UTF8.GetBytes(Utility.ReverseString(key));
            int valueLength = bValue.Length;
            int keyLength = bKey.Length;
            byte[] tempValue = new byte[valueLength + 1];
            Random random = new Random();
            tempValue[0] = (byte)random.Next();
            Buffer.BlockCopy(bValue, 0, tempValue, 1, valueLength);
            bValue = tempValue;
            for (int i = 0; i < valueLength; i++)
            {
                bValue[i + 1] = (byte)(bValue[i + 1] ^ bKey[(i * 2) % keyLength] ^ bValue[0]);
            }
            return Convert.ToBase64String(bValue);
        }
        /// <summary>
        /// Reverzne string
        /// </summary>
        /// <param name="value">String ktory chceme reverznut</param>
        /// <returns>Reverznuty string alebo null</returns>
        public static String ReverseString(String value)
        {
            if (!String.IsNullOrWhiteSpace(value))
            {
                char[] array = value.ToCharArray();
                Array.Reverse(array);
                return new String(array);
            }
            return null;
        }
        /// <summary>
        /// Overi ci je guid empty
        /// </summary>
        /// <param name="value">Guid ktory chceme overit</param>
        /// <returns>True = guid je empty, inak false</returns>
        public static Boolean IsEmpty(Guid value)
        {
            return Guid.Empty.CompareTo(value) == 0;
        }
        /// <summary>
        /// Overi ci je hodnota typu alpha
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme overit</param>
        /// <param name="length">Dlzka stringu ktory chceme overit</param>
        /// <returns>True hodnota je alpha, inak false</returns>
        public static Boolean IsAlpha(string value, int length)
        {
            if (String.IsNullOrEmpty(value))
            {
                return false;
            }
            return Regex.IsMatch(value, @"^[a-zA-Z]{" + length + ",}$");
        }
        /// <summary>
        /// Overi ci je hodnota typu anumeric
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme overit</param>
        /// <param name="length">Dlzka stringu ktory chceme overit</param>
        /// <returns>True hodnota je numeric, inak false</returns>
        public static Boolean IsNumeric(string value, int length)
        {
            if (String.IsNullOrEmpty(value))
            {
                return false;
            }
            return Regex.IsMatch(value, @"^[0-9]{" + length + ",}$");
        }
        /// <summary>
        /// Overi ci je hodnota typu alphanumeric
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme overit</param>
        /// <param name="length">Dlzka stringu ktory chceme overit</param>
        /// <returns>True hodnota je alphanumeric, inak false</returns>
        public static Boolean IsAlphaNumeric(string value, int length)
        {
            if (String.IsNullOrEmpty(value))
            {
                return false;
            }
            return Regex.IsMatch(value, @"^[0-9a-zA-Z]{"+ length + ",}$");
        }
        /// <summary>
        /// Zacriptuje string do SHA1
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme zacryptovat</param>
        /// <returns>Zacryptovany string</returns>
        public static String EncryptoSHA1(String value)
        {
            return Utility.EncryptoSHA1(value, Encoding.UTF8);
        }
        /// <summary>
        /// Zacriptuje string do SHA1
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme zacryptovat</param>
        /// <param name="encoding">Encoding v akom chceme kodovat</param>
        /// <returns>Zacryptovany string</returns>
        public static String EncryptoSHA1(String value, Encoding encoding)
        {
            if (!String.IsNullOrWhiteSpace(value))
            {
                using (SHA1CryptoServiceProvider provider = new SHA1CryptoServiceProvider())
                {
                    if (encoding == null)
                    {
                        encoding = Encoding.UTF8;
                    }
                    var result = BitConverter.ToString(provider.ComputeHash(encoding.GetBytes(value))).Replace("-", "");
                    return result.ToLower();
                }
            }
            return String.Empty;
        }
        /// <summary>
        /// Vyparsuje guid z textu ktory neobsahuje pomlcky
        /// </summary>
        /// <param name="guid">String ktory obsahuje guid bez pomlciek</param>
        /// <returns>Guid alebo null</returns>
        public static Nullable<Guid> ParseGuidWithoutDash(String guid)
        {
            //overie string
            if (String.IsNullOrWhiteSpace(guid) || guid.Length != 32)
            {
                return null;
            }

            //pridame pomlcky
            guid = guid.Insert(8, "-").Insert(13, "-").Insert(18, "-").Insert(23, "-");

            //overime guid
            if (!Utility.ValidateGuid(guid))
            {
                return null;
            }

            //vyparsujeme quid
            return Utility.ParseGuid(guid);
        }
        /// <summary>
        /// Vyparsuje jedinecny identifikator
        /// </summary>
        /// <param name="guid">String-ova hodnota v ktorej sa ma Guid nachadzat</param>
        /// <returns>Vyparsovany Guid alebo Guid.Empty</returns>
        public static Nullable<Guid> ParseGuid(String guid)
        {
            if (!String.IsNullOrWhiteSpace(guid))
            {
                Guid key = Guid.Empty;
                if (Utility.ValidateGuid(guid))
                {
                    if (!Guid.TryParse(guid, out key))
                    {
                        return null;
                    }
                }
                return key;
            }
            return null;
        }
        /// <summary>
        /// Overi spravnost stringu ako guid
        /// </summary>
        /// <param name="guid">Guid hodnota v stringu</param>
        /// <returns>True = hodnota je guid, inak false</returns>
        public static Boolean ValidateGuid(String guid)
        {
            if (String.IsNullOrWhiteSpace(guid))
                return false;

            return Regex.IsMatch(guid, @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$");
        }
        /// <summary>
        /// Overi ci je objekt ciselneho typu
        /// </summary>
        /// <param name="value">Objekt ktory chceme overit</param>
        /// <returns>True = objekt je ciselneho typu, inak false</returns>
        public static Boolean IsNumbericType(Object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return value is int || value is uint || value is float || value is decimal || value is double;
        }
        /// <summary>
        /// Odstrani zo stringu biele znaky
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Ak string neobsahuje ine znaky ako biele, pripadne je null
        /// </exception>
        /// <param name="str">String ktory chceme upravit</param>
        /// <returns>Upraveny string</returns>
        public static String StringTrim(String str)
        { 
            if (String.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentNullException("str");
            }

            //inicializujeme
            Regex regex = new Regex(@"\s");

            //odstranime znaky
            return regex.Replace(str, String.Empty);
        }
        /// <summary>
        /// Overi spravnost telefonneho cisla. Minimalna dlzka cislic je 8
        /// </summary>
        /// <param name="phone">Cislo ktore chceme overit</param>
        /// <returns>True = cislo je spravne</returns>
        public static Boolean ValidatePhoneNumber(String phone)
        {
            // Check for exactly 10 numbers left over
            return Regex.IsMatch(phone, @"^\+?\d{3,15}$"); 
        }
        /// <summary>
        /// Vrati Regex string s upravou na pozadovanu dlzku pri kontrole
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Argument mimo rozsah. Min value is 1
        /// </exception>
        /// <param name="regexConstant">Konstanta z Utility.REGEX ktoru chceme modifikovat</param>
        /// <param name="length">Pozadovana dlzka pri kontrole</param>
        /// <returns>Regex string</returns>
        public static String GetRegexPattern(String regexConstant, int length)
        {
            if (length < 1)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            return regexConstant.Replace("LENGTH", length.ToString());
        }
        /// <summary>
        /// Overi meno serioveho portu
        /// </summary>
        /// <param name="time">Cas ktory chceme overit</param>
        /// <returns>True = cas je ok, inak false</returns>
        public static Boolean ValidateTime(String time)
        {
            //osetrenies
            if (String.IsNullOrEmpty(time))
                return false;

            //regularny vyraz na overnie mena portu
            String pattern = @"^([01]?[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]$";

            //overime meno
            return Regex.IsMatch(time, pattern);
        }
        /// <summary>
        /// method for validating a url with regular expressions
        /// </summary>
        /// <param name="url">url we're validating</param>
        /// <returns>true if valid, otherwise false</returns>
        public static Boolean ValidateUrl(String url)
        {
            //pattern na validaciu
            String pattern = @"^(((http|https|ftp)\://)|(www\.))[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}$";
            
            //nastavenie regex
            Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            //validacia
            return reg.IsMatch(url);
        }
        /// <summary>
        /// Overi meno serioveho portu
        /// </summary>
        /// <param name="portName">Meno portu ktore chceme voerit</param>
        /// <returns>True = meno portu je spravne, inak false</returns>
        public static Boolean ValidatePortName(String portName)
        {
            //osetrenies
            if (String.IsNullOrEmpty(portName))
                return false;

            //regularny vyraz na overnie mena portu
            String pattern = @"^[Cc][Oo][Mm][1-9]\d?\d?$";

            //overime meno
            return Regex.IsMatch(portName, pattern);
        }
        /// <summary>
        /// Vrati cas vytvorenia Buildu z verzie ktora bola vytvorena sposobom [X.X.*]
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Ak nebol vstupny argument inicializovany
        /// </exception>
        /// <param name="version">Verzia assembly</param>
        /// <returns>Datum a cas vytvorenia buildu</returns>
        public static DateTime GetDateTimeFromVersion(Version version)
        {
            //osetrenie
            if (version == null)
                throw new ArgumentNullException("version");

            //inicializacia
            DateTime dateTime = new DateTime(2000, 1, 1, 0, 0, 0);

            //Build - pocet dni od zaciatku roka 2000
            //Revision - pocet dvoj sekundoviek aktualneho dna, podla UTC casu
            dateTime = dateTime.AddDays(version.Build).AddSeconds(version.Revision * 2);

            //vratime datum
            return dateTime;
        }
        /// <summary>
        /// Overi spravnost zadanej emailovej adresy
        /// </summary>
        /// <param name="address">Adresa ktoru chceme overit</param>
        /// <returns>True = adresa je spravna</returns>
        public static Boolean ValidateMailAddress(String address)
        {
            //osetrenies
            if (String.IsNullOrEmpty(address))
                return false;

            //regularny vyraz na overenie addresy
            String pattern = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|" +
                             @"(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
           
            //overenei adresy
            return Regex.IsMatch(address, pattern);
        }
        /// <summary>
        /// method to validate an IP address
        /// using regular expressions. The pattern
        /// being used will validate an ip address
        /// with the range of 1.0.0.0 to 255.255.255.255
        /// </summary>
        /// <param name="address">Address to validate</param>
        /// <returns></returns>
        public static Boolean ValidateIpAddress(String address)
        {
            //create our match pattern
            string pattern = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
            //create our Regular Expression object
            Regex check = new Regex(pattern);
            //boolean variable to hold the status
            bool valid = false;
            //check to make sure an ip address was provided
            if (String.IsNullOrEmpty(address))
            {
                //no address provided so return false
                valid = false;
            }
            else
            {
                //address provided so use the IsMatch Method
                //of the Regular Expression object
                valid = check.IsMatch(address, 0);
            }
            //return the results
            return valid;
        }
        /// <summary>
        /// Zacryptuje vstupny string na zaklade vstupneho hesla
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException">
        /// Prazdny vstupny argument
        /// </exception>
        /// 
        /// <param name="str">String ktory chceme Crytpovat</param>
        /// <param name="password">Cryptovacie heslo</param>
        /// <param name="data">Zacryptovane data</param>
        /// <returns>True = kryptovanie bolo uspesne</returns>
        public static Boolean TryStringEncrypt(String str, String password, out String data)
        {
            //osetrime vstupne argumenty
            if (String.IsNullOrEmpty(str))
                throw new ArgumentNullException("str");
            if (String.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            //zabezpecime out
            data = String.Empty;

            try
            {
                //zacryptujeme data
                data = Utility.StringEncrypt(str, password);

                //kryptovanie bolo uspesne
                return true;
            }
            catch (Exception)
            {
                //ziadne data
                data = String.Empty;

                //ukoncenie s chybou
                return false;
            }
        }
        /// <summary>
        /// Zacryptuje vstupny string na zaklade vstupneho hesla
        /// </summary>
        /// 
        /// <exception cref="Exception">
        /// Chyba decryptovania
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Prazdny vstupny argument
        /// </exception>
        /// 
        /// <param name="str">String ktory chceme krytpovat</param>
        /// <param name="password">Cryptovacie heslo</param>
        /// <returns>Zacryptovany string</returns>
        public static String StringEncrypt(String str, String password)
        {
            //osetrime vstupne argumenty
            if (String.IsNullOrEmpty(str))
                throw new ArgumentNullException("str");
            if (String.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            //inicializacia
            MemoryStream memoryStream = null;
            CryptoStream cryptoStream = null;

            try
            {
                RijndaelManaged RijndaelCipher = new RijndaelManaged();
                RijndaelCipher.Padding = PaddingMode.PKCS7;
                byte[] PlainText = System.Text.Encoding.Unicode.GetBytes(str);
                byte[] Salt = Encoding.ASCII.GetBytes(password.Length.ToString());

                PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(password, Salt);
                //Creates a symmetric encryptor object.
                ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
                memoryStream = new MemoryStream();
                //Defines a stream that links data streams to cryptographic transformations
                cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);
                cryptoStream.Write(PlainText, 0, PlainText.Length);
                //Writes the final state and clears the buffer
                cryptoStream.FlushFinalBlock();
                byte[] CipherBytes = memoryStream.ToArray();

                //vratime zacryptovany string
                return Convert.ToBase64String(CipherBytes);
            }
            catch (Exception ex)
            {
                //zalogujeme
                ConsoleLogger.WriteLine(ex);
                //preposleme vynimku
                throw;
            }
            finally
            {
                //ukoncime
                if (memoryStream != null)
                {
                    memoryStream.Close();
                    memoryStream.Dispose();
                }
                //ukoncime
                if (cryptoStream != null)
                {
                    cryptoStream.Close();
                    cryptoStream.Dispose();
                }
            }
        }
        /// <summary>
        /// Odkryptuje string na zaklade vstupneho hesla
        /// </summary>
        /// <param name="str">String ktory chcem odkryptovat</param>
        /// <param name="password">Dekryptovacie heslo</param>
        /// <param name="data">Dekryptovane data</param>
        /// <returns>True = dekryptovanie bolo uspesne</returns>
        public static Boolean TryStringDecrypt(String str, String password, out String data)
        {
            //osetrime vstupne argumenty
            if (String.IsNullOrEmpty(str))
                throw new ArgumentNullException("str");
            if (String.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            //zabezpecime out
            data = String.Empty;

            try
            {
                //odcryptujeme data
                data = Utility.StringDecrypt(str, password);

                //dekryptovanie bolo uspesne
                return true;
            }
            catch (Exception ex)
            {
                //zalogujeme
                ConsoleLogger.WriteLine(ex);
#if DEBUG
                Debug.WriteLine(ex);
#endif

                //ukoncenie s chybou
                return false;
            }
        }
        /// <summary>
        /// Odcryptuje string na zaklade vstupneho hesla
        /// </summary>
        /// <param name="str">String ktory chcem odcryptovat</param>
        /// <param name="password">Decryptovacie heslo</param>
        /// <returns>Odcryptovane data</returns>
        public static String StringDecrypt(String str, String password)
        {
            //osetrime vstupne argumenty
            if (String.IsNullOrEmpty(str))
                throw new ArgumentNullException("str");
            if (String.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            //inicializacia
            MemoryStream memoryStream = null;
            CryptoStream cryptoStream = null;

            try
            {
                RijndaelManaged RijndaelCipher = new RijndaelManaged();
                RijndaelCipher.Padding = PaddingMode.PKCS7;
                byte[] EncryptedData = Convert.FromBase64String(str);
                byte[] Salt = Encoding.ASCII.GetBytes(password.Length.ToString());
                //Making of the key for decryption
                PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(password, Salt);
                //Creates a symmetric Rijndael decryptor object.
                ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
                memoryStream = new MemoryStream(EncryptedData);
                //Defines the cryptographics stream for decryption.THe stream contains decrpted data
                cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);
                byte[] PlainText = new byte[EncryptedData.Length];
                int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);

                //Converting to string
                return Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);
            }
            catch (Exception ex)
            {
                //zalogujeme
                ConsoleLogger.WriteLine(ex.Message);
#if DEBUG
                Debug.WriteLine(ex);
#endif
                //preposleme vynimku
                throw;
            }
            finally
            {
                //ukoncime
                if (memoryStream != null)
                {
                    memoryStream.Close();
                    memoryStream.Dispose();
                }
                //ukoncime
                if (cryptoStream != null)
                {
                    cryptoStream.Close();
                    cryptoStream.Dispose();
                }
            }
        }
        /// <summary>
        /// Na zaklade vstupneho DateTime vrati UnixTime
        /// </summary>
        /// <param name="date">Vstupny datum z ktoreho ratame unixTime</param>
        /// <returns>UnixTime ohraniceny na processingTimeout</returns>
        public static Int32 ConvertDateTimeToUnixTime(DateTime date)
        {
            //vypocitame unix time
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return (Int32)(diff.TotalSeconds);
        }
        /// <summary>
        /// Prepocita UnixTime na DateTime foramt
        /// </summary>
        /// <param name="Seconds">UnixTime</param>
        /// <returns>DateTime</returns>
        public static DateTime ConvertUnixTimeToDateTime(Int32 Seconds)
        {
            //vypocitame unix time
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            origin = origin.AddSeconds(Seconds);
            return origin;
        }
        /// <summary>
        /// Prekonvertuje tabulku do stringu predstavujuceho CSV format Excelu
        /// </summary>
        /// <param name="table">Tabulka ktoru chceme prekonvertovat</param>
        /// <returns>String predstavujuci obsah CSV suboru</returns>
        public static String ConvertDataTableToCsvFile(DataTable table)
        {
            //osetrime vstup
            if (table == null)
                throw new ArgumentNullException("table");
            if (table.Columns.Count == 0)
                throw new ArgumentException("table is not valid !");

            //builder na pridavanie stringov
            StringBuilder builder = new StringBuilder();

            //pridame stlpce
            foreach (DataColumn column in table.Columns)
            {
                //pridame 
                builder.Append(String.Format("{0};", column.ColumnName));
            }

            //odstranime poslednu ciarku
            builder.Remove(builder.Length - 1, 1);

            //pridame data
            foreach (DataRow row in table.Rows)
            {
                //dalsi riadok
                builder.Append(Environment.NewLine);

                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    //overime ci ide o DT typ
                    if (row[i].GetType() == typeof(DateTime))
                    {
                        //format zobrazovanej polozky
                        String format = (((DateTime)row[i]).Millisecond != 0) ? "yyyy-MM-dd HH:mm:ss.fff" : "yyyy-MM-dd HH:mm:ss";

                        //pridame data
                        builder.Append(String.Format("{0};", ((DateTime)row[i]).ToString(format)));
                    }
                    else
                    {
                        //pridame 
                        builder.Append(String.Format("{0};", row[i]));
                    }
                }

                //odstranime poslednu ciarku
                builder.Remove(builder.Length - 1, 1);
            }

            //vratime vytvorene data
            return builder.ToString();
        }
        #endregion
    }
}
