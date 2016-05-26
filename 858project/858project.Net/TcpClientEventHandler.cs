using System;
using System.Collections.Generic;
using System.Text;

namespace Project858.Net
{
	/// <summary>
	/// Delegat ako predpis pre event oznamujuci uspesne akceptovanie klienta serverom
	/// </summary>
	/// <param name="sender">Odosielatel udalosti</param>
	/// <param name="e">TcpClientEventArgs</param>
	public delegate void TcpClientEventHandler(Object sender, TcpClientEventArgs e);
	
}
