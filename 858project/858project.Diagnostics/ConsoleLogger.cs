using System;
using System.Collections.Generic;
using System.Text;

namespace Project858.Diagnostics
{
    /// <summary>
    /// Zaloguje informacie do kontrolnej consoly v zvolenom formate
    /// </summary>
    public class ConsoleLogger
    {
        #region - Vriable -
        /// <summary>
        /// Pomocny synchronizacny objekt
        /// </summary>
        private static readonly Object _globalLock = new Object();
        #endregion

        #region - Public Method -
        /// <summary>
        /// Zapise data do konzoly v zvolenom formate
        /// </summary>
        /// <param name="format">String format</param>
        /// <param name="arg">Argumenty</param>
        public static void WriteLine(String format, params Object[] arg)
        {
            lock (_globalLock)
            {
                Console.WriteLine("[{0}]: {1}", DateTime.Now.ToString("HH:mm:ss.fff"), ((arg != null && arg.Length > 0) ? String.Format(format, arg) : format));
            }
        }
        /// <summary>
        /// Zapise data do konzoly v zvolenom formate
        /// </summary>
        /// <param name="format">String format</param>
        /// <param name="color">Color for text</param>
        /// <param name="arg">Argumenty</param>
        public static void WriteLine(String format, ConsoleColor color, params Object[] arg)
        {
            lock (_globalLock)
            {
                Console.ForegroundColor = color;
                Console.WriteLine("[{0}]: {1}", DateTime.Now.ToString("HH:mm:ss.fff"), ((arg != null && arg.Length > 0) ? String.Format(format, arg) : format));
                Console.ResetColor();
            }   
        }
        /// <summary>
        /// Zapise data do konzoly v zvolenom formate
        /// </summary>
        /// <param name="object">Objekty ktory chceme vypisat. (ToString())</param>
        public static void WriteLine(Object @object)
        {
            lock (_globalLock)
            {
                Console.WriteLine("[{0}]: {1}", DateTime.Now.ToString("HH:mm:ss.fff"), @object);
            }
        }
        #endregion
    }
}
