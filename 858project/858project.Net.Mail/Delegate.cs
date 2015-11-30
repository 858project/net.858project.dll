using System;
using System.Collections.Generic;
using System.Text;

namespace Project858.Net.Mail
{
    /// <summary>
    /// Delegat ako predpis pre event oznamujuci poziadavku na odoslanie emailovej spravy
    /// </summary>
    /// <param name="sender">Odosielatel udalosti</param>
    /// <param name="e">MailEventArgs</param>
    public delegate void MailEventHandler(Object sender, MailEventArgs e);
}
