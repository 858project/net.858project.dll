using System;
using System.Collections.Generic;
using System.Text;

namespace Project858.Diagnostics
{
    /// <summary>
    /// Delegat ako predpis pre event oznamujuci poziadavku na logovanie informacii
    /// </summary>
    /// <param name="sender">Odosielatel udalosti</param>
    /// <param name="e">TraceEventArgs</param>
    public delegate void TraceEventHandler(Object sender, TraceEventArgs e);
}
