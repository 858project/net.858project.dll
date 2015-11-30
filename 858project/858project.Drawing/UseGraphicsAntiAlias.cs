using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Project858.Drawing
{
    /// <summary>
    /// Nastavu SmoothingMode=AntiAlias ktory sa zrusi pri _disposed.
    /// </summary>
    public class UseGraphicsAntiAlias : IDisposable
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="graphics">Graphic na vykreslovanie</param>
        public UseGraphicsAntiAlias(Graphics graphics)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");

            this._graphics = graphics;
            this._smoothingMode = _graphics.SmoothingMode;
            this._graphics.SmoothingMode = SmoothingMode.AntiAlias;
        }
        #endregion

        #region - Variable -
        /// <summary>
        /// Graphic na vykreslovanie
        /// </summary>
        private Graphics _graphics = null;
        /// <summary>
        /// Povodny mod
        /// </summary>
        private SmoothingMode _smoothingMode = SmoothingMode.None;
        #endregion

        #region - Method -
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be _disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                //Revert the SmoothingMode back to original setting.
                this._graphics.SmoothingMode = this._smoothingMode;
            }
        }
        /// <summary>
        /// Releases all resources used by the class. 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region - Destructor -
        /// <summary>
        /// Deinitialize this class
        /// </summary>
        ~UseGraphicsAntiAlias()
        {
            Dispose(false);
        }
        #endregion
    }
}
