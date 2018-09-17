using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project858.Net
{
    /// <summary>
    /// Delegat na oznamenie vykonania operacie s datami
    /// </summary>
    /// <param name="sender">Odosielatel udalosti</param>
    /// <param name="e">FrameEventArgs</param>
    public delegate void FrameEventHandler(Object sender, FrameEventArgs e);
}
