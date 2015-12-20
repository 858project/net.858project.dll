using System;
using System.Collections.Generic;
using System.Text;
using Project858.ComponentModel.Client;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace Project858.Diagnostics
{
    /// <summary>
    /// Logger na zalogovanie zavaznych chyb mimo rozsahu
    /// </summary>
    public static class TraceLogger
    {
        #region - Constant -
        /// <summary>
        /// Meno subfoldra na ulozenei trace
        /// </summary>
        private const String SUB_FOLDER_NAME = "Trace";
        #endregion

        #region - Delegate -
        /// <summary>
        /// Pomocny delegat na asynchronne volanie metody
        /// </summary>
        /// <param name="time">Cas vzniku logovacej spravy</param>
        /// <param name="_traceType">Typ logovacej spravy</param>
        /// <param name="modulName">Meno modulu ktory spravy vytvoril</param>
        /// <param name="message">Text logovacej spravy</param>
        /// <returns>True = logovanie spravy bolo uspesne</returns>
        private delegate Boolean TraceAsyncDelegate(DateTime time, TraceTypes traceType, String modulName, String message);
        #endregion

        #region - Properties -
        /// <summary>
        /// Folder kde sa data ukladaju
        /// </summary>
        public static String DirectoryPath
        {
            get { return TraceLogger.m_directory; }
            set { TraceLogger.m_directory = value; }
        }
        #endregion

        #region - Variable -
        /// <summary>
        /// Folder kde sa data ukladaju
        /// </summary>
        private static String m_directory = String.Empty;
        /// <summary>
        /// Pomocny synchronizacny objekt na pristup k databaze
        /// </summary>
        private static readonly Object m_lockObj = new Object();
        #endregion

        #region - Public Method -
        /// <summary>
        /// Zaloguje pozadovanu spravu ako ERROR spravu
        /// </summary>
        /// <param name="exception">Chyba ktoru chceme zalogovat</param>
        /// <returns>True = logovanie bolo uspesne inak false</returns>
        public static bool Error(Exception exception)
        {
            return TraceLogger.Error(null, exception.ToString());
        }
        /// <summary>
        /// Zaloguje pozadovanu spravu ako ERROR spravu
        /// </summary>
        /// <param name="message">Sprava ktoru chceme zalogovat</param>
        /// <param name="args">Aegument pre spravu do String.Format()</param>
        /// <returns>True = logovanie bolo uspesne inak false</returns>
        public static bool Error(String message, params Object[] args)
        {
            return TraceLogger.Error(null, String.Format(message, args));
        }
        /// <summary>
        /// Zaloguje pozadovanu spravu ako ERROR spravu
        /// </summary>
        /// <param name="modulName">Meno modulu ktory spravu loguje</param>
        /// <param name="message">Sprava ktoru chceme zalogovat</param>
        /// <param name="args">Aegument pre spravu do String.Format()</param>
        /// <returns>True = logovanie bolo uspesne inak false</returns>
        public static bool Error(String modulName, String message, params Object[] args)
        {
            return TraceLogger.Error(DateTime.Now, modulName, String.Format(message, args));
        }
        /// <summary>
        /// Zaloguje pozadovanu spravu ako ERROR spravu
        /// </summary>
        /// <param name="date">Datum a cas vzniku spravy</param>
        /// <param name="modulName">Meno modulu ktory spravu loguje</param>
        /// <param name="message">Sprava ktoru chceme zalogovat</param>
        /// <param name="args">Aegument pre spravu do String.Format()</param>
        /// <returns>True = logovanie bolo uspesne inak false</returns>
        public static bool Error(DateTime date, String modulName, String message, params Object[] args)
        {
            return TraceLogger.Trace(date, TraceTypes.Error, modulName, String.Format(message, args));
        }
        /// <summary>
        /// Zaloguje pozadovanu spravu ako INFO spravu
        /// </summary>
        /// <param name="message">Sprava ktoru chceme zalogovat</param>
        /// <param name="args">Aegument pre spravu do String.Format()</param>
        /// <returns>True = logovanie bolo uspesne inak false</returns>
        public static bool Info(String message, params Object[] args)
        {
            return TraceLogger.Info(null, String.Format(message, args));
        }
        /// <summary>
        /// Zaloguje pozadovanu spravu ako INFO spravu
        /// </summary>
        /// <param name="modulName">Meno modulu ktory spravu loguje</param>
        /// <param name="message">Sprava ktoru chceme zalogovat</param>
        /// <param name="args">Aegument pre spravu do String.Format()</param>
        /// <returns>True = logovanie bolo uspesne inak false</returns>
        public static bool Info(String modulName, String message, params Object[] args)
        {
            return TraceLogger.Info(DateTime.Now, modulName, String.Format(message, args));
        }
        /// <summary>
        /// Zaloguje pozadovanu spravu ako INFO spravu
        /// </summary>
        /// <param name="date">Datum a cas vzniku spravy</param>
        /// <param name="modulName">Meno modulu ktory spravu loguje</param>
        /// <param name="message">Sprava ktoru chceme zalogovat</param>
        /// <param name="args">Aegument pre spravu do String.Format()</param>
        /// <returns>True = logovanie bolo uspesne inak false</returns>
        public static bool Info(DateTime date, String modulName, String message, params Object[] args)
        {
            return TraceLogger.Trace(date, TraceTypes.Info, modulName, String.Format(message, args));
        }
        /// <summary>
        /// Zaloguje spravu do SQLite databazy
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Argument nie je inicializovany
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Ak klient nie je v stave Start
        /// </exception>
        /// <param name="date">Cas vzniku logovacej spravy</param>
        /// <param name="traceType">Typ logovacej spravy</param>
        /// <param name="modulName">Meno modulu ktory spravy vytvoril</param>
        /// <param name="message">Text logovacej spravy</param>
        /// <returns>True = logovanie spravy bolo uspesne</returns>
        public static bool Trace(DateTime date, TraceTypes traceType, String modulName, String message)
        {
            //overime vstupne data
            if (String.IsNullOrEmpty(modulName))
                throw new ArgumentNullException("modulName");
            if (String.IsNullOrEmpty(message))
                throw new ArgumentNullException("message");

            //zalogujeme spravu
            return TraceToFile(date, traceType, modulName, message);
        }
        /// <summary>
        /// Zaloguje spravu do SQLite databazy asynchronnym volanim
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Argument nie je inicializovany
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Ak klient nie je v stave Start
        /// </exception>
        /// <param name="date">Cas vzniku logovacej spravy</param>
        /// <param name="traceType">Typ logovacej spravy</param>
        /// <param name="modulName">Meno modulu ktory spravy vytvoril</param>
        /// <param name="message">Text logovacej spravy</param>
        public static void TraceAsync(DateTime date, TraceTypes traceType, String modulName, String message)
        {
            //overime vstupne data
            if (String.IsNullOrEmpty(modulName))
                throw new ArgumentNullException("modulName");
            if (String.IsNullOrEmpty(message))
                throw new ArgumentNullException("message");

            //pomocny delegat
            TraceAsyncDelegate asyncDelegate = new TraceAsyncDelegate(TraceToFile);
            asyncDelegate.BeginInvoke(date, traceType, modulName, message, null, null);
        }
        #endregion

        #region - Private Method -
        /// <summary>
        /// Zapise pozadovane data do SQLite databazy
        /// </summary>
        /// <param name="date">Cas log zaznamu</param>
        /// <param name="traceType">Typ logovacej spravy</param>
        /// <param name="modulName">Meno modulu ktory spravy vytvoril</param>
        /// <param name="message">Sprava s informaciou</param>
        /// <returns>True = zapis logu bol uspesny</returns>
        private static Boolean TraceToFile(DateTime date, TraceTypes traceType, String modulName, String message)
        {
            //zapisovac do suboru
            TextWriter textWriter = null;

            try
            {
                lock (m_lockObj)
                {
                    //ak nie je zadany folder na logovanie
                    if (String.IsNullOrWhiteSpace(TraceLogger.m_directory))
                    {
                        //vytvorime cestu k priecinku na ulozenie Trace
                        TraceLogger.m_directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "Trace";
                    }

                    //zistime ci existuje subfolder ak nie tak ho pridame...
                    if (!Directory.Exists(TraceLogger.m_directory))
                        Directory.CreateDirectory(TraceLogger.m_directory);

                    //vytvorime plnu cestu k suboru
                    String fileName = TraceLogger.m_directory + Path.DirectorySeparatorChar + DateTime.Now.Date.ToString("yyyy_MM_dd") + ".log";

                    //inicializacia
                    textWriter = TextWriter.Synchronized(File.AppendText(fileName));

                    //meno modulu
                    modulName = String.IsNullOrWhiteSpace(modulName) ? String.Empty : String.Format(" [{0}] ", modulName);

                    // Create string recipients write
                    message = String.Format("{0}{1}{2} -> {3}", date.ToString("yyyy-MM-dd HH:mm:ss.fff"), traceType, modulName, message);

                    //zapiseme na konzolu
                    ConsoleLogger.WriteLine(message);

                    // Write string and close file
                    textWriter.WriteLine(message);

                    //zapis logu bol uspesny
                    return true;
                }
            }
            catch (Exception ex)
            {
                //zapis do trace, netusim kde recipients pojde
                ConsoleLogger.WriteLine("Chyba pri zapise log suboru. {0}", ex);
#if DEBUG
                Debug.WriteLine(ex);
#endif
                //ukoncime s chybou
                return false;
            }
            finally
            {
                //ukoncime zapisovac
                if (textWriter != null)
                {
                    textWriter.Close();
                    textWriter.Dispose();
                }
            }
        }
        #endregion
    }
}
