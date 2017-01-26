using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project858.IO.Ports
{
    /// <summary>
    /// Delegat na oznamenie vykonania operacie s datami
    /// </summary>
    /// <param name="sender">Odosielatel udalosti</param>
    /// <param name="e">DataBytesEventArgs</param>
    public delegate void DataEventHandler(Object sender, DataEventArgs e);
}
