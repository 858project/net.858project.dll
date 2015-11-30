using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Project858.Drawing
{
    /// <summary>
    /// Nastavi TextRenderingHint.ClearTypeGridFit ktory sa zrusi pri _disposed.
    /// </summary>
    public class UseGraphicsClearTypeGridFit : IDisposable
    {
        #region - Constructor -
        /// <summary>
        /// Initialize a new instance of the UseClearTypeGridFit class.
        /// </summary>
        /// <param name="graphics">Graphics instance.</param>
        public UseGraphicsClearTypeGridFit(Graphics graphics)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");

            this._graphics = graphics;
            this._textRenderingHint = this._graphics.TextRenderingHint;
            this._graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
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

        #region - Variable -
        /// <summary>
        /// Graphic na vykreslovanie
        /// </summary>
        private Graphics _graphics = null;
        /// <summary>
        /// Renderer
        /// </summary>
        private TextRenderingHint _textRenderingHint = TextRenderingHint.SystemDefault;
        #endregion

        #region MethodsProtected
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be _disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                //Revert the TextRenderingHint back to original setting.
                this._graphics.TextRenderingHint = this._textRenderingHint;
            }
        }
        #endregion

        #region - Descructor -
        /// <summary>
        /// Deinitialize this class
        /// </summary>
        ~UseGraphicsClearTypeGridFit()
        {
            Dispose(false);
        }
        #endregion
    }
}
